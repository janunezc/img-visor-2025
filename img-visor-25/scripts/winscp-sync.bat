@echo off	
set "LOGPATH=%LOCALAPPDATA%\img-visor\logs"
if not exist "%LOGPATH%" mkdir "%LOGPATH%"

"C:\Program Files (x86)\WinSCP\winscp.com" /script=./winscp-sync.txt > "%LOGPATH%\winscp-sync.log"