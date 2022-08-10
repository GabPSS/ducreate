import cv2
import numpy as np
import glob

size = (1366,768)

writer = cv2.VideoWriter('output.avi',cv2.VideoWriter_fourcc(*'DIVX'), 60, size)

for file in glob.glob('*.png'):
    img = cv2.imread(file)
    writer.write(img)

writer.release()