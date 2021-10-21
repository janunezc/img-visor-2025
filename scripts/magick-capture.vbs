Set WshShell = CreateObject("WScript.Shell") 
WshShell.Run chr(34) & "magick-capture.bat" & Chr(34), 0
Set WshShell = Nothing