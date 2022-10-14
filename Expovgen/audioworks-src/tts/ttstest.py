from gtts import gTTS
f = open('article.txt','r')
text = f.read()
f.close()
i = 0
tts = gTTS(text, lang="pt")
tts.save("article.mp3")