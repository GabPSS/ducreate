import ffmpeg
import os

print(os.getcwd())

in_file = ffmpeg.input("exstack/*.jpg", pattern_type='glob',framerate=1)
#in_file = ffmpeg.input("input.mp4")

fps = 60

in_mus = ffmpeg.input("aud.mp3")
a1 = in_mus.audio

in_file2 = in_file.drawbox(50,50,120,120, color='Blue', thickness='5')
in_file2 = in_file2.drawtext('Este e um video gerado em python!',x='(w-tw)/2',y=986, fontsize='98',box=1,boxcolor='black',fontcolor='white')

conc = ffmpeg.concat(in_file2, a1, v=1, a=1)
out = ffmpeg.output(conc, 'movie.mp4')
out.run();
