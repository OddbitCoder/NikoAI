models = [
	"VGG-Face", 
	"Facenet", 
	"Facenet512", 
	"OpenFace", 
	"DeepFace", 
	"DeepID", 
	"ArcFace", 
	"Dlib", 
	"SFace"
]

detectors = [
    #"opencv", # does not detect any faces
    "ssd",
    "dlib",
    "mtcnn",
    "retinaface",
    "mediapipe",
    "yolov8",
    "yunet"
]

import tensorflow as tf

from deepface import DeepFace

import os

# disable GPU
os.environ["CUDA_VISIBLE_DEVICES"]=""

# configure GPU memory usage
# WARNME: This allocates as much mem as needed. However, it somehow fails to see how much memory is available in total (~5 GB instead of 8 GB). 
# If memory limit is set explicitly on the logical device, the memory growth setting is ignored.
gpus = tf.config.list_physical_devices('GPU')
if gpus: 
    for gpu in gpus:
        tf.config.experimental.set_memory_growth(gpu, True)
    # tf.config.set_logical_device_configuration(
    #     gpus[0],
    #     [tf.config.LogicalDeviceConfiguration(memory_limit=8*1024)]
    # )

# test all models & detectors

for model in models:
	print("*** model: " + model)
	veri = DeepFace.verify(img1_path = "/files/img1.jpg", 
	    img2_path = "/files/img2.jpg", 
	    model_name = model,
	    detector_backend = "retinaface"
	)
	print(veri)

for detector in detectors:
	print("*** detector: " + detector)
	veri = DeepFace.verify(img1_path = "/files/img1.jpg", 
	    img2_path = "/files/img2.jpg", 
	    model_name = "VGG-Face",
	    detector_backend = detector
	)
	print(veri)

# other tests

face_objs = DeepFace.extract_faces(img_path = "/files/img2.jpg", 
    target_size = (224, 224), 
    detector_backend = "retinaface"
)

print(face_objs)