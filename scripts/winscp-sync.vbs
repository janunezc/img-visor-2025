Set WshShell = CreateObject("WScript.Shell") 
WshShell.Run chr(34) & "winscp-sync.bat" & Chr(34), 0
Set WshShell = Nothing