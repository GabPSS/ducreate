import sys;
import perform_tts;
import perform_align;

if sys.argv[1] == 'tts':
    perform_tts.PerformTTS(sys.argv[2],sys.argv[3])
if sys.argv[1] == 'align':
    perform_align.PerformAlign(sys.argv[2],sys.argv[3],sys.argv[4])