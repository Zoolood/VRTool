Combiner.exe

for /f "delims=" %%a in ('dir /B *.dat') do ffmpeg -i "%%a" -vcodec copy -acodec copy "%%~na0.ts"

Combiner.exe combine

ffmpeg -i combined.ts -vcodec copy -acodec copy combined.mp4