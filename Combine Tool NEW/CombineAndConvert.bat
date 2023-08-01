for /d %%i in (*) do type %%~nxi\*.mp4 > %%~nxi.dat
for /f "delims=" %%a in ('dir /B *.dat') do ffmpeg -i "%%a" -vcodec copy -acodec copy "%%~na.ts"
type *.ts > combined.raw
ffmpeg -i combined.raw -vcodec copy -acodec copy combined.mp4
