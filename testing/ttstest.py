from gtts import gTTS

tts = gTTS("Testing TTS Functionality", lang="en")
tts.save("test.mp3")