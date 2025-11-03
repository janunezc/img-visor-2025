; img-visor-installer.iss
; Installer for img-visor (.NET 8 Windows app)
; Requires Inno Setup 6.x

#define AppName "img-visor"
#define AppVersion "1.0.20251101"
#define AppPublisher "NUNEZ TECHNOLOGIES"
#define AppExeName "img-visor-25.exe"

; --- Customize these two paths for your environment before compiling ---
; VS Publish output (contains exe, deps, config, etc.)
#define PublishDir "C:\Users\josea\git\img-visor-2025\img-visor-25\bin\Release\net8.0-windows\publish"
; Project scripts folder (repo root \scripts)
#define ScriptsDir "C:\Users\josea\git\img-visor-2025\img-visor-25\scripts"
#define ImageDir "C:\Users\josea\git\img-visor-2025\img-visor-25\image"

[Setup]
AppId={{A47C8A5C-32E0-4D0C-9E9F-IMGVISOR2025}}  ; new GUID if you change identity
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputBaseFilename=img-visor-installer
Compression=lzma
SolidCompression=yes

; Require admin and install into Program Files (x64-appropriate)
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes

; Show "Ready to Install" as last step
DisableReadyMemo=no

; Don't allow multiple instances of installer running
AllowRootDirectory=no

; --- Files to install ---
[Files]
; Everything from publish (recommended: include all; the app controls what it needs)
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

; Scripts folder copied as-is to Program Files\img-visor\scripts
Source: "{#ScriptsDir}\*"; DestDir: "{app}\scripts"; Flags: recursesubdirs createallsubdirs ignoreversion
Source: "{#ImageDir}\*"; DestDir: "{app}\image"; Flags: recursesubdirs createallsubdirs ignoreversion

; --- Create local data directories ---
[Dirs]
; Program Files base directories (Windows default ACLs already restrict write access)
Name: "{app}"
Name: "{app}\scripts"
Name: "{app}\image"

; LocalAppData working folders for the app
Name: "{localappdata}\img-visor"; Flags: uninsalwaysuninstall
Name: "{localappdata}\img-visor\backlog"; Flags: uninsalwaysuninstall
Name: "{localappdata}\img-visor\logs"; Flags: uninsalwaysuninstall


; --- Registry (optional) ---

; If you want a Run key for convenience (not needed once the Task Scheduler is in place), leave commented:
; [Registry]
; Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; \
;   ValueType: string; ValueName: "img-visor"; ValueData: """{app}\{#AppExeName}"""; Flags: uninsdeletevalue


; --- Optional: kill running process during uninstall to release files ---
[UninstallDelete]
; Best-effort clean logs (leave if you prefer to keep)
; Type: filesandordirs; Names: "{localappdata}\img-visor\logs"
; Type: filesandordirs; Name: "{localappdata}\img-visor\backlog"

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
begin
  { You can add logging or validation here if needed }
end;
