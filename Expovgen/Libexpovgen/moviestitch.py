import sys
from moviepy.video.fx import resize
from moviepy.editor import *

# Read files and define variables and arrays

vid_width = sys.argv[1]
vid_height = sys.argv[2]

split_syncmap = []
with open('res\\splitmap.txt',mode="r",encoding="utf-8") as split_syncmap_file:
    split_syncmap = split_syncmap_file.readlines()

vclips_array = []
cc_clips_array = []

cc_texts = []
cc_map = []
cc_durations = []

with open('res\\ccmap.txt',mode="r",encoding="utf-8") as cc_map_file:
    cc_map = cc_map_file.readlines()

with open('res\\cc.txt',mode="r",encoding="utf-8") as cc_texts_file:
    cc_texts = cc_texts_file.readlines()


# Create image clips and add them respectively ------------------------------

# counter = Gets incremented each time a fragment is iterated through
#           At the end of the process, it will correspond to the timestamp of
#           the end of the video's slides
counter = 0 
for x in split_syncmap:
    data = x.split()
    # clipduration = The interval of an aligned fragment, corresponding to a slide in the final video
    clipduration = float(data[3]) - float(data[2])
    counter += clipduration
    imgclip = ImageClip("res\\imgs\\" + data[0] + ".jpg",duration=clipduration)
    imgclip = imgclip.resize( (vid_width,vid_height) )
    vclips_array.append(imgclip)

# Create speech clip
speech_aud = AudioFileClip("res\\speech.mp3")
speech_aud = speech_aud.subclip(0,counter);

# Create background music clip
music_aud = 0
if sys.argv[3] == "True":
    music_aud = AudioFileClip("res\\music.mp3")
    music_aud = music_aud.volumex(float(sys.argv[5]))
    if counter < music_aud.duration:
        music_aud = music_aud.subclip(0,counter)

# Creates caption clips and adds them
counter2 = 0
for text in cc_texts:
    cc_data = cc_map[counter2].split()
    cc_clip = TextClip(text,fontsize=float(sys.argv[4]),color='white',bg_color='black')
    cc_clip = cc_clip.set_position('bottom')
    cc_duration = float(cc_data[2]) - float(cc_data[1])
    cc_clip = cc_clip.set_duration(cc_duration)
    cc_clips_array.append(cc_clip)
    cc_durations.append(cc_duration)
    counter2 += 1

# Resplits video in order to add subtitles
allvideo = concatenate_videoclips(vclips_array)
vid_divisions = []

counter3 = 0
startat = 0
for x in cc_durations:
    endat = startat + x
    division = allvideo.subclip(startat,endat)
    division = CompositeVideoClip([division,cc_clips_array[counter3]])
    vid_divisions.append(division)
    counter3 += 1
    startat = endat

# Joins final audio # TODO: Move this over to the last part, with all clips.
final_aud = speech_aud
if music_aud != 0:
    final_aud = CompositeAudioClip([speech_aud,music_aud]);

# Generate end (credits) clip
creditsTxt = "";
ending_card_clip = 0;

with open('res\\card_credits.txt',mode="r",encoding="utf-8") as cc_map_file:
    creditsTxt = cc_map_file.read()

if creditsTxt != "":
    ending_card_clip = TextClip(creditsTxt, fontsize=40,color='white',bg_color='black',size=vid_divisions[0].size)
    ending_card_clip = ending_card_clip.set_position('center')
    ending_card_clip = ending_card_clip.set_duration(5)

# Gererate title clip
titleTxt = "";
title_card_clip = 0;

with open('res\\card_title.txt',mode="r",encoding="utf-8") as cc_map_file:
    titleTxt = cc_map_file.read()

if titleTxt != "":
    title_card_clip = TextClip(titleTxt, fontsize=60,color='white',bg_color='black',size=vid_divisions[0].size)
    title_card_clip = title_card_clip.set_position('center')
    title_card_clip = title_card_clip.set_duration(3)

# Join everything together
finalvid = concatenate_videoclips(vid_divisions);
finalvid = finalvid.set_audio(final_aud);
if ending_card_clip != 0:
    finalvid = concatenate_videoclips([finalvid,ending_card_clip]);
if title_card_clip != 0:
    finalvid = concatenate_videoclips([title_card_clip,finalvid]);
finalvid.write_videofile('res\\output.mp4', fps=30)




## --- EXTRA BITS OF CODE ---

#aud = aud.subclip(0,20)
#aud = aud.volumex(0.2)

#aud_speech = AudioFileClip("speech.mp3")
#aud_speech = aud_speech.subclip(0,20)

#clip_aud = CompositeAudioClip([aud,aud_speech])

#txt1 = TextClip("Primeira legenda", fontsize=70, color='white',bg_color='black')
#txt1 = txt1.set_position('bottom')
#txt1 = txt1.set_duration(7)
#txt2 = TextClip("Segunda legenda", fontsize=70, color='white',bg_color='black')
#txt2 = txt2.set_position('bottom')
#txt2 = txt2.set_duration(4)
#txt3 = TextClip("Terceira legenda", fontsize=70, color='white',bg_color='black')
#txt3 = txt3.set_position('bottom')
#txt3 = txt3.set_duration(4)
#txt4 = TextClip("Quarta legenda", fontsize=70, color='white',bg_color='black')
#txt4 = txt4.set_position('bottom')
#txt4 = txt4.set_duration(5)

## generates video divisions

#video = concatenate_videoclips([img1,img2,img3,img4])
#videodiv1 = video.subclip(0,7)
#videodiv2 = video.subclip(7,11)
#videodiv3 = video.subclip(11,15)
#videodiv4 = video.subclip(15,20)

## adds captions

#videodiv1 = CompositeVideoClip([videodiv1,txt1])
#videodiv2 = CompositeVideoClip([videodiv2,txt2])
#videodiv3 = CompositeVideoClip([videodiv3,txt3])
#videodiv4 = CompositeVideoClip([videodiv4,txt4])

## brings everything together with audio
#finalvid = concatenate_videoclips([videodiv1,videodiv2,videodiv3,videodiv4])
#finalvid = finalvid.set_audio(clip_aud)
#finalvid.write_videofile('concat.mp4',fps=30)

#### img_clip1 = ImageClip("002.png", duration=2)
#### img_clip1 = resize.resize(img_clip1,width=vid_width,height=vid_height)

#### #img_clip1 = img_clip1.resize(1920,1080)
#### img_clip2 = ImageClip("003.png",duration=2)
#### img_clip2 = resize.resize(img_clip2,width=vid_width,height=vid_height)


#### aud_bg = AudioFileClip("aud.mp3")
#### aud_bg = aud_bg.volumex(0.2)
#### aud_bg = aud_bg.subclip(0,4)

#### aud_speech = AudioFileClip("speech.mp3")
#### aud_speech = aud_speech.subclip(0,4)


#### txt_clip1 = TextClip("This one was made by MoviePy!!", fontsize=70, color='white',bg_color='black')
#### txt_clip1 = txt_clip1.set_position('bottom')
#### txt_clip1 = txt_clip1.set_duration(2)

#### txt_clip2 = TextClip("This is a second text!!", fontsize=70, color='white', bg_color='black')
#### txt_clip2 = txt_clip2.set_position('bottom')
#### txt_clip2 = txt_clip2.set_duration(2)

#### clip1 = CompositeVideoClip([img_clip1,txt_clip1])
#### clip2 = CompositeVideoClip([img_clip2,txt_clip2])
#### clip_aud = CompositeAudioClip([aud_bg,aud_speech])

#### AnArray = [clip1,clip2]

#### video = concatenate_videoclips(AnArray)
#### video = video.set_audio(clip_aud)
# video.write_videofile('moviepy.mp4', fps=30)