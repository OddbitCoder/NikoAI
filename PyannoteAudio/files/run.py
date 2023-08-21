# 1. visit hf.co/pyannote/speaker-diarization and hf.co/pyannote/segmentation and accept user conditions (only if requested)
# 2. visit hf.co/settings/tokens to create an access token (only if you had to go through 1.)
# 3. instantiate pretrained speaker diarization pipeline
from pyannote.audio import Pipeline
import torchaudio
import torch

if torch.cuda.is_available():
    device = torch.device("cuda")
    print(f"GPU {torch.cuda.get_device_name(0)} is available")
else:
    device = torch.device("cpu")
    print("No GPU available, using CPU instead")

waveform, sample_rate = torchaudio.load("/files/dont_erase_me.wav")
audio_in_memory = {"waveform": waveform, "sample_rate": sample_rate}

pipeline = Pipeline.from_pretrained("/files/config.yaml")
pipeline.to(device)

# 4. apply pretrained pipeline
diarization = pipeline(audio_in_memory)

# 5. print the result
for turn, _, speaker in diarization.itertracks(yield_label=True):
    print(f"start={turn.start:.1f}s stop={turn.end:.1f}s speaker_{speaker}")
