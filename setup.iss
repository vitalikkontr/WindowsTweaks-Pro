[Setup]
AppId={{B7E3F8A2-C941-4D5E-9B12-8FA6E2D1C3E7}}
AppName=WindowsTweaks
AppVersion=2.3
AppVerName=WindowsTweaks Professional 2.3
AppPublisher=Виталий Николаевич (vitalikkontr)
AppPublisherURL=https://github.com/vitalikkontr/WindowsTweaks-Pro
AppSupportURL=https://github.com/vitalikkontr/WindowsTweaks-Pro/issues
AppUpdatesURL=https://github.com/vitalikkontr/WindowsTweaks-Pro/releases
AppCopyright=Copyright © 2026 vitalikkontr

DefaultDirName={autopf}\WindowsTweaksProfessional
DefaultGroupName=WindowsTweaks
DisableProgramGroupPage=yes
OutputDir=C:\Release\Setup
OutputBaseFilename=WindowsTweaks-Professional-Setup-v2.3
Compression=lzma2
SolidCompression=yes
WizardStyle=modern

PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
LanguageDetectionMethod=locale
ShowLanguageDialog=no

UninstallDisplayIcon={app}\WindowsTweaks.exe
UninstallDisplayName=WindowsTweaks Professional

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "autostart"; Description: "Запускать при старте Windows"; GroupDescription: "Дополнительные опции:"; Flags: unchecked

[Files]
Source: "C:\Users\vital\source\repos\WindowsTweaks_Pro\bin\Release\net8.0-windows8.0\*.*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\WindowsTweaks"; Filename: "{app}\WindowsTweaks.exe"
Name: "{group}\{cm:UninstallProgram,WindowsTweaks}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\WindowsTweaks"; Filename: "{app}\WindowsTweaks.exe"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\WindowsTweaksProfessional"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\WindowsTweaksProfessional\Settings"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "WindowsTweaks"; ValueData: "{app}\WindowsTweaks.exe /autostart"; Flags: uninsdeletevalue; Tasks: autostart

[Run]
Filename: "{app}\WindowsTweaks.exe"; Description: "{cm:LaunchProgram,WindowsTweaks}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: files; Name: "{app}\config.ini"
Type: files; Name: "{app}\*.log"
Type: dirifempty; Name: "{app}"

[Code]
const
  AppMutexName = 'WindowsTweaksMutex';

function IsAppRunning(): Boolean;
begin
  Result := CheckForMutexes(AppMutexName);
end;

function CloseRunningApp(): Boolean;
var
  ResultCode: Integer;
begin
  Result := True;
  if IsAppRunning() then
  begin
    if MsgBox('Приложение WindowsTweaks запущено. Закрыть его для продолжения?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      Exec('taskkill.exe', '/F /IM WindowsTweaks.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    end
    else
      Result := False;
  end;
end;

function InitializeSetup(): Boolean;
begin
  Result := CloseRunningApp();
end;

function InitializeUninstall(): Boolean;
begin
  Result := CloseRunningApp();
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Здесь можно добавить дополнительные действия после установки
  end;
end;
