from moviepy.video.fx import resize
from moviepy.editor import *

vid_height = 1366
vid_width = 768

img_clip1 = ImageClip("002.png", duration=2)
img_clip1 = resize.resize(img_clip1,width=vid_width,height=vid_height)

#img_clip1 = img_clip1.resize(1920,1080)
img_clip2 = ImageClip("003.png",duration=2)
img_clip2 = resize.resize(img_clip2,width=vid_width,height=vid_height)


aud_bg = AudioFileClip("aud.mp3")
aud_bg = aud_bg.volumex(0.2)
aud_bg = aud_bg.subclip(0,4)

aud_speech = AudioFileClip("speech.mp3")
aud_speech = aud_speech.subclip(0,4)


txt_clip1 = TextClip("This one was made by MoviePy!!", fontsize=70, color='white',bg_color='black')
txt_clip1 = txt_clip1.set_position('bottom')
txt_clip1 = txt_clip1.set_duration(2)

txt_clip2 = TextClip("This is a second text!!", fontsize=70, color='white', bg_color='black')
txt_clip2 = txt_clip2.set_position('bottom')
txt_clip2 = txt_clip2.set_duration(2)

clip1 = CompositeVideoClip([img_clip1,txt_clip1])
clip2 = CompositeVideoClip([img_clip2,txt_clip2])
clip_aud = CompositeAudioClip([aud_bg,aud_speech])

AnArray = [clip1,clip2]

video = concatenate_videoclips(AnArray)
video = video.set_audio(clip_aud)
video.write_videofile('moviepy.mp4', fps=30)