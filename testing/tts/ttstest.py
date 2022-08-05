from gtts import gTTS
f = open('assu.txt','r')
text = f.read()
f.close()
i = 0
tts = gTTS(text, lang="pt")
tts.save("assu.mp3")