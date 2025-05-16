using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LLAPP
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> leerlingen = new();

        public MainWindow()
        {
            InitializeComponent();
            LaadLeerlingen();
        }

        private void LaadLeerlingen()
        {
            if (File.Exists("leerlingen.json"))
            {
                string json = File.ReadAllText("leerlingen.json");
                leerlingen = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
        }

        private void OpslaanLeerlingen()
        {
            string json = JsonSerializer.Serialize(leerlingen, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("leerlingen.json", json);
        }

        private void ZoekButton_Click(object sender, RoutedEventArgs e)
        {
            string naam = SearchBox.Text.Trim();

            if (leerlingen.ContainsKey(naam))
            {
                string fotoPad = leerlingen[naam];
                if (File.Exists(fotoPad))
                {
                    BitmapImage image = new();
                    image.BeginInit();
                    image.UriSource = new Uri(Path.GetFullPath(fotoPad));
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    LeerlingFoto.Source = image;
                }
                else
                {
                    MessageBox.Show("Foto niet gevonden.");
                }
            }
            else
            {
                MessageBox.Show("Leerling niet gevonden.");
            }
        }

        private void ToevoegenButton_Click(object sender, RoutedEventArgs e)
        {
            string naam = Microsoft.VisualBasic.Interaction.InputBox("Vul de naam van de leerling in:", "Leerling Toevoegen");

            if (string.IsNullOrWhiteSpace(naam))
            {
                MessageBox.Show("Geen naam ingevuld.");
                return;
            }

            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Afbeeldingen (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                string fotosMap = "Fotos";
                if (!Directory.Exists(fotosMap))
                {
                    Directory.CreateDirectory(fotosMap);
                }

                string bestandsNaam = Path.GetFileName(openFileDialog.FileName);
                string doelPad = Path.Combine(fotosMap, bestandsNaam);

                File.Copy(openFileDialog.FileName, doelPad, true);

                leerlingen[naam] = doelPad;
                OpslaanLeerlingen();

                MessageBox.Show("Leerling toegevoegd!");
            }
        }
    }
}
