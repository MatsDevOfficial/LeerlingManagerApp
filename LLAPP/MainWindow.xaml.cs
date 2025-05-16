using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace LeerlingTool
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> leerlingen = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            LoadLeerlingen();
            SearchPlaceholder.Visibility = Visibility.Visible;
        }

        private void LoadLeerlingen()
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "leerlingen.json");
                if (File.Exists(jsonPath))
                {
                    string jsonText = File.ReadAllText(jsonPath);
                    leerlingen = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonText);

                    UpdateNamenLijst();
                }
                else
                {
                    File.WriteAllText(jsonPath, "{}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij laden leerlingen: " + ex.Message);
            }
        }

        private void UpdateNamenLijst()
        {
            NamenLijst.ItemsSource = null;
            NamenLijst.ItemsSource = new List<string>(leerlingen.Keys);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string zoekterm = SearchBox.Text.Trim();
            if (leerlingen.ContainsKey(zoekterm))
            {
                ToonFoto(zoekterm);
                NamenLijst.SelectedItem = zoekterm;
            }
            else
            {
                MessageBox.Show("Leerling niet gevonden.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void NamenLijst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NamenLijst.SelectedItem != null)
            {
                string naam = NamenLijst.SelectedItem.ToString();
                SearchBox.Text = naam;
                ToonFoto(naam);
            }
        }

        private void ToonFoto(string naam)
        {
            if (leerlingen.TryGetValue(naam, out string bestandsnaam))
            {
                string fotoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fotos", bestandsnaam);
                if (File.Exists(fotoPath))
                {
                    LeerlingFoto.Source = new BitmapImage(new Uri(fotoPath));
                    NaamLabel.Text = naam;
                }
                else
                {
                    LeerlingFoto.Source = null;
                    NaamLabel.Text = "Foto niet gevonden";
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string naam = NieuwNaamBox.Text.Trim();
            if (string.IsNullOrEmpty(naam))
            {
                MessageBox.Show("Vul een naam in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Afbeeldingen (*.png;*.jpg)|*.png;*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                string fotosDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fotos");
                if (!Directory.Exists(fotosDir))
                    Directory.CreateDirectory(fotosDir);

                string bestandsnaam = Path.GetFileName(openFileDialog.FileName);
                string doelPad = Path.Combine(fotosDir, bestandsnaam);

                try
                {
                    File.Copy(openFileDialog.FileName, doelPad, true);
                    leerlingen[naam] = bestandsnaam;

                    string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "leerlingen.json");
                    string jsonText = JsonSerializer.Serialize(leerlingen, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(jsonPath, jsonText);

                    UpdateNamenLijst();
                    NieuwNaamBox.Text = "";

                    MessageBox.Show("Leerling toegevoegd!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij toevoegen: " + ex.Message);
                }
            }
        }
    }
}
