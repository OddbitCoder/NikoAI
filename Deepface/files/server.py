import argparse
import signal

from concurrent import futures
import os
import os.path

import grpc

import contracts_pb2
import contracts_pb2_grpc

import json

import cv2
import numpy as np

import tensorflow as tf

from deepface.detectors import FaceDetector
from deepface import DeepFace
from deepface.commons import functions, distance as dst

tf_version = tf.__version__
tf_major_version = int(tf_version.split(".", maxsplit=1)[0])
tf_minor_version = int(tf_version.split(".")[1])

if tf_major_version == 1:
    from keras.preprocessing import image
elif tf_major_version == 2:
    from tensorflow.keras.preprocessing import image


def handler(signum, frame):
    exit(1)

signal.signal(signal.SIGINT, handler)


class Service(contracts_pb2_grpc.ModelServiceServicer):
  def Process(self, request, context):
    #print(len(request.data))
    img = cv2.imdecode(np.frombuffer(request.data, dtype=np.uint8), cv2.IMREAD_COLOR)
    # detect faces
    try:
        img_objs = extract_faces(img) 
    except:
        return contracts_pb2.ProcessReply(reply=json.dumps({ "objects": [] })) 
    #print(img_objs)
    # embed faces
    results = []
    tabu = set()
    for img_content, img_region, _ in img_objs:
        vec = represent(
            img_data=img_content
        )
        vec = vec[0]["embedding"]
        min_dist = 1
        best_match = None
        # find matching faces
        for person_fn in os.listdir(db_dir):
            if person_fn in tabu:
                continue # for person_fn
            for img_fn in os.listdir(os.path.join(db_dir, person_fn)):
                if not img_fn.lower().endswith(".vec") and not img_fn.lower().endswith(".err"):
                    vec2 = represent_known(os.path.join(db_dir, person_fn, img_fn))
                    if (vec2 != None):
                        print(f"Comparing with {person_fn} ({img_fn})...")
                        if distance_metric == "cosine":
                            distance = dst.findCosineDistance(vec, vec2)
                        elif distance_metric == "euclidean_l2":
                            distance = dst.findEuclideanDistance(dst.l2_normalize(vec), dst.l2_normalize(vec2))
                        else: # euclidean
                            distance = dst.findEuclideanDistance(vec, vec2)
                        threshold = dst.findThreshold(model_name, distance_metric)
                        if distance < min_dist:
                            min_dist = distance
                            best_match = {
                                "x": img_region['x'],
                                "y": img_region['y'],
                                "width": img_region['w'],
                                "height": img_region['h'],
                                "name": person_fn,
                                "distance": distance,
                                "threshold": threshold,
                                "verified": bool(distance <= threshold)
                            }
                            if distance <= threshold and greedy_search:
                                break # for img_fn 
            # for each person_fn continue here...
        # for each embedding continue here...
        if best_match != None:
            results.append(best_match)
            if greedy_search and best_match["verified"]:
                # put best_match["name"] to tabu list
                tabu.add(best_match["name"])

    return contracts_pb2.ProcessReply(reply=json.dumps({ "objects": results })) 


def init():
    global detector_name
    global model_name
    global db_dir
    global distance_metric
    global greedy_search
    gpu_enable, gpu_mem, detector_name, model_name, db_dir, distance_metric, greedy_search = opt.gpu, opt.gpu_mem, opt.detector, opt.model, opt.db, opt.dist, opt.greedy
    if not gpu_enable:
        os.environ["CUDA_VISIBLE_DEVICES"] = ""
    else:
        gpus = tf.config.list_physical_devices('GPU')
        if gpus: 
            if gpu_mem <= 0:
                for gpu in gpus:
                    tf.config.experimental.set_memory_growth(gpu, True)
            else: # WARNME: this only affects the first physical device
                tf.config.set_logical_device_configuration(
                    gpus[0],
                    [tf.config.LogicalDeviceConfiguration(memory_limit=gpu_mem)]
                )
    global face_detector
    face_detector = FaceDetector.build_model(detector_name)
    global model
    model = DeepFace.build_model(model_name)


def represent_known(
    img_fn
):
    vec_fn = img_fn + f".{model_name}.vec"

    # read from cache (if it exists)
    if os.path.isfile(vec_fn):
        with open(vec_fn, 'r') as f:
            vec = json.load(f)
            return vec

    # read image
    with open(img_fn, "rb") as img_f:
        chunk = img_f.read()
        img = cv2.imdecode(np.frombuffer(chunk, dtype=np.uint8), cv2.IMREAD_COLOR)
    
    # detect and extract faces
    try:
        img_objs = extract_faces(img) # WARNME: this sometimes fails
    except:
        os.rename(img_fn, img_fn + ".err");
        return None

    # create and write embedding to file
    for img_content, img_region, _ in img_objs:
        vec = represent(
            img_data=img_content
        )
        vec = vec[0]["embedding"]
        with open(vec_fn, 'w') as f:
            f.write(json.dumps(vec))
        return vec

    return None # we failed to create or read embedding


def represent( # WARNME: Taken from the original code. It is somewhat buggy.
    img_data,
    normalization="base",
):
    resp_objs = []

    target_size = functions.find_target_size(model_name=model_name)
    img = img_data.copy()
    # --------------------------------
    if len(img.shape) == 4:
        img = img[0]  # e.g. (1, 224, 224, 3) to (224, 224, 3)
    if len(img.shape) == 3:
        img = cv2.resize(img, target_size)
        img = np.expand_dims(img, axis=0)
    # --------------------------------
    img_region = [0, 0, img.shape[1], img.shape[0]] # WARNME: I think this should be [0, 0, img.shape[2], img.shape[1]]
    img_objs = [(img, img_region, 0)] # WARNME: these are image data, region, confidence, respectively
    # ---------------------------------

    for img, region, confidence in img_objs:
        # custom normalization
        img = functions.normalize_input(img=img, normalization=normalization)

        # represent
        if "keras" in str(type(model)):
            # new tf versions show progress bar and it is annoying
            embedding = model.predict(img, verbose=0)[0].tolist()
        else:
            # SFace and Dlib are not keras models and no verbose arguments
            embedding = model.predict(img)[0].tolist()

        resp_obj = {}
        resp_obj["embedding"] = embedding
        resp_obj["facial_area"] = region
        resp_obj["face_confidence"] = confidence
        resp_objs.append(resp_obj)

    return resp_objs


def extract_faces(
    img,
    target_size=(224, 224),
    grayscale=False,
    enforce_detection=True,
    align=True,
):
    # this is going to store a list of img itself (numpy), its region and confidence
    extracted_faces = []

    img_region = [0, 0, img.shape[1], img.shape[0]]

    face_objs = FaceDetector.detect_faces(face_detector, detector_name, img, align)

    # in case of no face found
    # if len(face_objs) == 0 and enforce_detection is True:
    #     print("handle issue")

    if len(face_objs) == 0 and enforce_detection is False:
        face_objs = [(img, img_region, 0)]

    for current_img, current_region, confidence in face_objs:
        if current_img.shape[0] > 0 and current_img.shape[1] > 0:
            if grayscale is True:
                current_img = cv2.cvtColor(current_img, cv2.COLOR_BGR2GRAY)

            # resize and padding
            if current_img.shape[0] > 0 and current_img.shape[1] > 0:
                factor_0 = target_size[0] / current_img.shape[0]
                factor_1 = target_size[1] / current_img.shape[1]
                factor = min(factor_0, factor_1)

                dsize = (
                    int(current_img.shape[1] * factor),
                    int(current_img.shape[0] * factor),
                )
                current_img = cv2.resize(current_img, dsize)

                diff_0 = target_size[0] - current_img.shape[0]
                diff_1 = target_size[1] - current_img.shape[1]
                if grayscale is False:
                    # Put the base image in the middle of the padded image
                    current_img = np.pad(
                        current_img,
                        (
                            (diff_0 // 2, diff_0 - diff_0 // 2),
                            (diff_1 // 2, diff_1 - diff_1 // 2),
                            (0, 0),
                        ),
                        "constant",
                    )
                else:
                    current_img = np.pad(
                        current_img,
                        (
                            (diff_0 // 2, diff_0 - diff_0 // 2),
                            (diff_1 // 2, diff_1 - diff_1 // 2),
                        ),
                        "constant",
                    )

            # double check: if target image is not still the same size with target.
            if current_img.shape[0:2] != target_size:
                current_img = cv2.resize(current_img, target_size)

            # normalizing the image pixels
            # what this line doing? must?
            img_pixels = image.img_to_array(current_img)
            img_pixels = np.expand_dims(img_pixels, axis=0)
            img_pixels /= 255  # normalize input in [0, 1]

            # int cast is for the exception - object of type 'float32' is not JSON serializable
            region_obj = {
                "x": int(current_region[0]),
                "y": int(current_region[1]),
                "w": int(current_region[2]),
                "h": int(current_region[3]),
            }

            extracted_face = [img_pixels, region_obj, confidence]
            extracted_faces.append(extracted_face)

    # if len(extracted_faces) == 0 and enforce_detection == True:
    #     print("handle issue")

    return extracted_faces    


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--port', type=int, default=9012, help='server port')
    parser.add_argument('--gpu', action='store_true', help='enable GPU support')
    parser.add_argument('--gpu-mem', type=int, default=0, help='GPU memory limit (0 or negative = auto-grow)')
    parser.add_argument('--detector', type=str, default='ssd', help='face detection model (ssd, dlib, mtcnn, retinaface, mediapipe, yolov8, yunet)') 
    parser.add_argument("--model", type=str, default='ArcFace', help='face recognition model (VGG-Face, Facenet, Facenet512, OpenFace, DeepFace, DeepID, ArcFace, Dlib, SFace)') 
    parser.add_argument("--db", type=str, default='/files/db', help='known faces database location') 
    parser.add_argument("--dist", type=str, default='cosine', help='distance metric (cosine, euclidean, euclidean_l2)') 
    parser.add_argument("--greedy", action='store_true', help='greedy database search')

    opt = parser.parse_args()
    print(opt)	

    init()

    # start RPC service
    svc = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    contracts_pb2_grpc.add_ModelServiceServicer_to_server(Service(), svc)
    svc.add_insecure_port(f"[::]:{opt.port}")
    svc.start()
    print(f"Service listening on port {opt.port}.")
    svc.wait_for_termination()