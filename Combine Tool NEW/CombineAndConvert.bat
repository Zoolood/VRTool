@echo off

echo Wait...

for /f %%i in ('dir /B /AD *') do dir /b /s /o:d  %%~nxi\* > %%~nxi.txt 
for /f %%i in ('dir /B /AD *') do (for /F "tokens=*" %%A in (%%~nxi.txt) do type "%%A") > %%~nxi.dat
for /f "delims=" %%a in ('dir /B *.dat') do ffmpeg -i "%%a" -vcodec copy -acodec copy "%%~na.ts"
type *.ts > combined.raw
ffmpeg -i combined.raw -vcodec copy -acodec copy combined.mp4
