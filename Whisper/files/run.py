import torch
import whisper
from whisper.tokenizer import get_tokenizer

if torch.cuda.is_available():
    device = torch.device("cuda")
    print(f"GPU {torch.cuda.get_device_name(0)} is available")
else:
    device = torch.device("cpu")
    print("No GPU available, using CPU instead")

model_name = "medium"
model = whisper.load_model(model_name).to(device)
audio_path = "/files/jfk.flac"

language = "en" if model_name.endswith(".en") else None
print("model loded")
result = model.transcribe(
    audio_path, language=language, temperature=0.0, word_timestamps=True
)
assert result["language"] == "en"
assert result["text"] == "".join([s["text"] for s in result["segments"]])

transcription = result["text"].lower()
print(transcription)
assert "my fellow americans" in transcription
assert "your country" in transcription
assert "do for you" in transcription

tokenizer = get_tokenizer(model.is_multilingual)
all_tokens = [t for s in result["segments"] for t in s["tokens"]]
assert tokenizer.decode(all_tokens) == result["text"]
assert tokenizer.decode_with_timestamps(all_tokens).startswith("<|0.00|>")

timing_checked = False
for segment in result["segments"]:
    for timing in segment["words"]:
        assert timing["start"] < timing["end"]
        if timing["word"].strip(" ,") == "Americans":
            assert timing["start"] <= 1.8
            assert timing["end"] >= 1.8
            timing_checked = True

assert timing_checked
