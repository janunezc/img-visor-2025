@echo off

set log_file="magick-capture.log"
set images_path=images\

echo VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV >>%log_file%
echo ------------------------------------------------------------------------- >>%log_file%
@echo MAGICK CAPTURE BAT: %1 >>magic-capture-exec.log
echo ------------------------------------------------------------------------- >>%log_file%
echo ------------------------------------------------------------------------- >>%log_file%
set file_name=%~1
set target_file=%images_path%%file_name%
echo TARGET FILE >>%log_file%
 
echo ------------------------------------------------------------------------- >>%log_file%
echo STEP 1: IDENTIFYING PREVIOUS FILE >>%log_file%

pushd images
for /f "tokens=*" %%a in ('dir /b /od') do set newest=%%a
popd

echo IDENTIFIED: "%newest%" >>%log_file%

echo ------------------------------------------------------------------------- >>%log_file%
echo STEP 2: TAKING SCREENSHOT AS %target_file% >>%log_file% 

"magick.exe" convert "screenshot:[0]" -quality 40%% "%target_file%" >>%log_file%

echo ------------------------------------------------------------------------- >>%log_file%
echo STEP 3: COMPARING PREVIOUS FILE >>%log_file%

echo NEWEST: %newest%
echo CURRENT: %target_file%
"magick.exe" compare -metric RMSE "images\%newest%" "%target_file%" diff.jpg 2>magick-compare-result.txt

echo ------------------------------------------------------------------------- >>%log_file%
echo STEP 4: DIFF DECISION >>%log_file%


FOR /F %%i in (magick-compare-result.txt) do set difference=%%i

set /a difference+=1

set criteria=1550

ECHO STEP 5: DIFFERENCE: %difference% vs %criteria% >>%log_file%

IF %difference% LSS %criteria% (
   ECHO STEP 5: DELETING TOO SIMILAR SCREENSHOT "%target_file%" >>%log_file%
   del "%target_file%"
)

IF %difference% GEQ %criteria% (
	ECHO STEP 5: FILE IS DIFFERENT ENOUGH TO KEEP >>%log_file%
)

echo ------------------------------------------------------------------------- >>%log_file%
ECHO STEP 5: DELETING OLDER FILES AT COMPUTER SIDE... >>%log_file%
forfiles -p "images" -s -m *.* -d -7 -c "cmd /c del @path" >>%log_file%

ECHO COMPLETE! >>%log_file%
echo ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ >>%log_file%
