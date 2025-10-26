; img-visor-installer.iss
; Inno Setup script - starter for img-visor (no ImageMagick dependency)
; Tested pattern: copy files, create ProgramData folders, ACLs, HKLM Run entry, scheduled task.

#define AppName "Img-Visor"
#define AppVersion "1.0.0"
#define AppPublisher "YourOrg"
#define AppExeName "img-visor-capture.exe"
#define OrchestratorExe "imgvisor-winfrm.exe"

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={pf}\{#AppName}
DisableProgramGroupPage=yes
OutputBaseFilename=img-visor-installer
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin   ; installer requires admin elevation

; --- Files to install ---
[Files]
; Main executables (assume you compile them into a `publish` folder before packaging)
Source: "publish\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\{#OrchestratorExe}"; DestDir: "{app}"; Flags: ignoreversion

; VBScript launcher (keeps the UI hidden). We'll create a tiny runner
Source: "scripts\magick-capture.vbs"; DestDir: "{app}\scripts"; Flags: ignoreversion

; winscp script (keep in place) - note: contains credentials; ensure secure handling
Source: "scripts\winscp-sync.txt"; DestDir: "{app}\scripts"; Flags: ignoreversion

; Other helpers (node scripts, README, license)
Source: "scripts\webcam-shot-take.js"; DestDir: "{app}\scripts"; Flags: ignoreversion
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion

; Create ProgramData folders by writing files (we'll do in [Dirs] and [Code] ACL)
[Dirs]
Name: "{commonappdata}\{#AppName}\images"; Flags: uninsclear
Name: "{commonappdata}\{#AppName}\Logs"; Flags: uninsclear

; --- Shortcuts ---
[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#OrchestratorExe}"; WorkingDir: "{app}"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#OrchestratorExe}"; WorkingDir: "{app}"; Tasks: desktopicon

; --- Registry (HKLM Run for all users) ---
[Registry]
; Add a startup entry in HKLM so the capture CLI runs at every user logon
; The command uses a small VBS wrapper to run hidden; change paths as needed
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; \
  ValueType: string; ValueName: "{#AppName}Capture"; \
  ValueData: """{app}\scripts\magick-capture.vbs"" ""{app}\{#AppExeName}"""; \
  Flags: uninsdeletevalue

; --- Run commands after install (we'll set ACLs and register scheduled task) ---
[Run]
; Set ACLs so regular Users can write to ProgramData image/logs folder
Filename: "cmd.exe"; Parameters: "/C icacls ""{commonappdata}\{#AppName}"" /grant Users:(OI)(CI)M /T"; \
  StatusMsg: "Configuring folder permissions..."; Flags: runhidden waituntilterminated

; Create scheduled task for periodic sync/maintenance
; We will create a task that runs as SYSTEM at logon and daily at 02:00 (example)
; Replace schedule or commands as needed
Filename: "schtasks.exe"; Parameters: "/Create /TN ""Img-Visor\imgvisor-maintenance"" /TR ""\"{app}\{#AppExeName}\"" --maintenance """"{commonappdata}\{#AppName}\images"""" 7"" /SC DAILY /ST 02:00 /RL HIGHEST /RU SYSTEM /F"; \
  Flags: runhidden runascurrentuser

; Optionally add a Task that runs at each user logon to ensure service for already-logged-in users
; NB: Creating per-user tasks from an elevated installer is delicate; HKLM Run above will already launch the runner at logon.

; --- UninstallRemove (cleanup scheduled tasks and Run key) ---
[UninstallRun]
; Delete scheduled task
Filename: "schtasks.exe"; Parameters: "/Delete /TN ""Img-Visor\imgvisor-maintenance"" /F"; Flags: runhidden

; Other cleanup is handled by uninstaller registry/uninsdeletevalue flags
