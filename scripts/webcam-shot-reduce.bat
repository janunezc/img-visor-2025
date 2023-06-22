@echo off

set log_file="webcam-shot-reduce.log"
set images_path=images\

echo VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV >>%log_file%
echo ------------------------------------------------------------------------- >>%log_file%
@echo MAGICK REDUCE: %1 >>%log_file%
echo ------------------------------------------------------------------------- >>%log_file%
echo ------------------------------------------------------------------------- >>%log_file%
set file_name=%~1
set target_file=%images_path%%file_name%
echo TARGET RESULT FILE >>%log_file%

echo Reducing file %file_name% into /images using mogrify... >>%log_file%
echo magick mogrify -path images/ -resize 256 %file_name% >>%log_file%

"magick.exe" mogrify -path images/ -resize 256 "%file_name%"

echo deleting the local file... >>%log_file%

echo del %file_name% >>%log_file%

del "%file_name%"

ECHO COMPLETE! >>%log_file%
echo ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ >>%log_file%
