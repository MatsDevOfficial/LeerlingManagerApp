[Setup]
AppName=Leerlingentool
AppVersion=1.0
DefaultDirName={pf}\Leerlingentool
DefaultGroupName=Leerlingentool
OutputDir=Output
OutputBaseFilename=LeerlingentoolInstaller
Compression=lzma
SolidCompression=yes

[Files]
Source: "bin\Release\net8.0-windows\Leerlingentool.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "leerlingen.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "Fotos\*"; DestDir: "{app}\Fotos"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Leerlingentool"; Filename: "{app}\Leerlingentool.exe"
Name: "{commondesktop}\Leerlingentool"; Filename: "{app}\Leerlingentool.exe"

[Run]
Filename: "{app}\Leerlingentool.exe"; Description: "Start Leerlingentool"; Flags: nowait postinstall skipifsilent

[Code]
var
  LicensePage: TInputQueryWizardPage;

const
  ValidLicenseKey = 'm7hjn99enndju10';

procedure InitializeWizard();
begin
  LicensePage := CreateInputQueryPage(wpWelcome,
    'Licentiecode invoeren',
    'Voer de licentiecode in om verder te gaan',
    'Voer hieronder uw licentiecode in:');
  LicensePage.Add('Licentiecode:', False);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if CurPageID = LicensePage.ID then
  begin
    if LicensePage.Values[0] <> ValidLicenseKey then
    begin
      MsgBox('De ingevoerde licentiecode is ongeldig. Probeer het opnieuw.', mbError, MB_OK);
      Result := False;
    end;
  end;
end;