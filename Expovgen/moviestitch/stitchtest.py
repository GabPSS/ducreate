import ffmpeg
import os

print(os.getcwd())

in_file = ffmpeg.input("exstack/img%03d.jpg",framerate=30,pix_fmt='yuv420p')
#in_file = ffmpeg.input("input.mp4")

in_file = in_file.filter('scale',1920,1080)


in_mus = ffmpeg.input("speech.mp3")
a1 = in_mus.audio

in_file2 = in_file.drawbox(50,50,120,120, color='Blue', thickness='5')
in_file2 = in_file2.drawtext('Este e um video gerado em python!',x='(w-tw)/2',y=986, fontsize='98',box=1,boxcolor='black',fontcolor='white')

conc = ffmpeg.concat(in_file2, a1, v=1, a=1)
out = ffmpeg.output(conc, 'movie.mov',pix_fmt='yuv420p',framerate=30, **{'c:v':'libx264'})
out.run();
