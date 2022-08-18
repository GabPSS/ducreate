from moviepy.editor import *

clip = VideoFileClip('input.mp4')
txtclp = TextClip('Testing out moviepy', fontsize=70, color='blue')
txtclp = txtclp.set_position('center').set_duration(3)

video = CompositeVideoClip([clip, txtclp])
clip.write_videofile('moviepy.mp4')