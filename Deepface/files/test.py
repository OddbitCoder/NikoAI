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
    "opencv",
    "ssd",
    "dlib",
    "mtcnn",
    "retinaface",
    "mediapipe",
    "yolov8",
    "yunet"
]

from deepface import DeepFace

# test all models & detectors

for model in models:
	veri = DeepFace.verify(img1_path = "/files/img1.jpg", 
	    img2_path = "/files/img2.jpg", 
	    model_name = model,
	    detector_backend = "retinaface"
	)
	print(veri)

for detector in detectors:
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