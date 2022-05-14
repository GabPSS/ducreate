from gtts import gTTS

text = open('assu.txt','r').read()
tts = gTTS(text, lang="pt")
tts.save("assu.mp3")