type data\*.mp4 > combined.raw
ffmpeg -i combined.raw -vcodec copy -acodec copy combined.mp4