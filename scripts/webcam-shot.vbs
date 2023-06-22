Set WshShell = CreateObject("WScript.Shell") 
WshShell.Run chr(34) & "webcam-shot.bat" & Chr(34), 0
Set WshShell = Nothing