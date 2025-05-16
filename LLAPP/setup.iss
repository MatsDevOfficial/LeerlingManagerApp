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
  PrivacyPage: TWizardPage;
  LicensePage: TInputQueryWizardPage;
  PrivacyTextBox: TMemo;
  PrivacyCheckBox: TCheckBox;

const
  ValidLicenseKey = 'm7hjn99enndju10';
  PrivacyPolicyText =
    'Privacybeleid:' + #13#10 +
    'Wij respecteren uw privacy en verzamelen alleen noodzakelijke gegevens voor het functioneren van deze app.' + #13#10 +
    'Uw gegevens worden niet gedeeld met derden.' + #13#10#13#10 +
    'Algemene Voorwaarden:' + #13#10 +
    'Door deze software te installeren, gaat u akkoord met het gebruik voor persoonlijk educatieve doeleinden.' + #13#10 +
    'De ontwikkelaar is niet aansprakelijk voor gegevensverlies of schade veroorzaakt door het gebruik van deze software.' + #13#10;

procedure InitializeWizard();
begin
  // Privacy & voorwaarden pagina
  PrivacyPage := CreateCustomPage(wpWelcome,
    'Privacybeleid en Algemene Voorwaarden',
    'Lees onderstaande voorwaarden aandachtig door en ga akkoord om verder te gaan.');

  PrivacyTextBox := TMemo.Create(PrivacyPage);
  PrivacyTextBox.Parent := PrivacyPage.Surface;
  PrivacyTextBox.Left := 0;
  PrivacyTextBox.Top := 0;
  PrivacyTextBox.Width := PrivacyPage.SurfaceWidth;
  PrivacyTextBox.Height := PrivacyPage.SurfaceHeight - 50;
  PrivacyTextBox.ScrollBars := ssVertical;
  PrivacyTextBox.ReadOnly := True;
  PrivacyTextBox.Text := PrivacyPolicyText;

  PrivacyCheckBox := TCheckBox.Create(PrivacyPage);
  PrivacyCheckBox.Parent := PrivacyPage.Surface;
  PrivacyCheckBox.Width := PrivacyPage.SurfaceWidth;
  PrivacyCheckBox.Height := 40;
  PrivacyCheckBox.Caption := 'Ik ga akkoord met het Privacybeleid en de Algemene Voorwaarden.';
  PrivacyCheckBox.Top := PrivacyTextBox.Top + PrivacyTextBox.Height + 5;
  PrivacyCheckBox.Left := 0;

  // Licentiecode pagina
  LicensePage := CreateInputQueryPage(PrivacyPage.ID,
    'Licentiecode invoeren',
    'Voer de licentiecode in om verder te gaan',
    'Licentiecode:');
  LicensePage.Add('Licentiecode:', False);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;

  if CurPageID = PrivacyPage.ID then
  begin
    if not PrivacyCheckBox.Checked then
    begin
      MsgBox('Je moet akkoord gaan met het Privacybeleid en de Algemene Voorwaarden om verder te gaan.', mbError, MB_OK);
      Result := False;
    end;
  end
  else if CurPageID = LicensePage.ID then
  begin
    if LicensePage.Values[0] <> ValidLicenseKey then
    begin
      MsgBox('De ingevoerde licentiecode is ongeldig. Probeer het opnieuw.', mbError, MB_OK);
      Result := False;
    end;
  end;
end;
