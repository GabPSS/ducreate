from gtts import gTTS

def PerformTTS(file_path: str, output_path: str):
    f = open(file_path,'r')
    text = f.read()
    f.close()
    i = 0
    tts = gTTS(text, lang="pt")
    tts.save(output_path)
