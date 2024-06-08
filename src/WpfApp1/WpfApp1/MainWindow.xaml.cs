﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Backend backendState;
        private Dictionary<string, (int Position, int HammingDistance, double ClosenessPercentage)> bestMatchesDict = new Dictionary<string, (int, int, double)>();
        private AlaiRegex alaiRegex = new AlaiRegex(); // Added instance of AlaiRegex

        public MainWindow()
        {
            backendState = new Backend();
            InitializeComponent();
        }

        private void searchClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "d:\\";
            openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
            openFileDialog.Title = "Choose file";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                backendState.setPic(openFileDialog.FileName);
                // MessageBox.Show(backendState.getPic());
                MessageBox.Show($"Selected file: {System.IO.Path.GetFileName(backendState.getPic())}");
                // MessageBox.Show(backendState.getPic());

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(backendState.getPic(), UriKind.Absolute);
                img.EndInit();
                // MessageBox.Show(backendState.getPic());
                selectedImage.Source = img;

            }
        }

        private void comboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            backendState.setAlgo(selectedItem.Content.ToString());
            MessageBox.Show($"Selected algorithm: {backendState.getAlgo()}");
        }

        private void searchGo(object sender, RoutedEventArgs e)
        {
            if (backendState.getPic() != null)
            {
                string imagePath = backendState.getPic();

                try
                {
                    // Process the image
                    string binaryRepresentation = FingerPrintConverter.ProcessImage(imagePath);
                    Console.WriteLine($"Binary representation: {binaryRepresentation}");

                    string midDigs = FingerPrintConverter.GetMiddleDigits(binaryRepresentation, 128);

                    string asciiRepresentation = FingerPrintConverter.BinaryToAscii(midDigs);
                    Console.WriteLine($"ASCII representation: {asciiRepresentation}");

                    if (!string.IsNullOrEmpty(asciiRepresentation))
                    {
                        // Reset the dictionary before starting the search
                        bestMatchesDict.Clear();

                        // Fetch database data
                        List<(int Id, string TempBC, string AsciiRepresentation, string Nama)> databaseData = FetchDatabaseData();
                        List<(string Nama, string TempatLahir, string JenisKelamin, string GolonganDarah, string Alamat, string StatusPerkawinan, string Pekerjaan, string Kewarganegaraan)> BiodataData = FetchBiodata();

                        // Perform search for the current ASCII representation
                        IStringSearchAlgorithm algorithm = GetSearchAlgorithm(backendState.getAlgo());
                        if (algorithm != null)
                        {
                            // List<(int Id, int Position, int HammingDistance, double ClosenessPercentage, string Nama)> bestMatches = new List<(int, int, int, double, string)>();
                            List<(int Id, int Position, int HammingDistance, double ClosenessPercentage, string Nama, string TempBC)> bestMatches = new List<(int, int, int, double, string, string)>();


                            Stopwatch stopwatch = Stopwatch.StartNew();

                            foreach (var data in databaseData)
                            {
                                var matches = algorithm.Search(data.AsciiRepresentation, asciiRepresentation).First();
                                bestMatches.Add((data.Id, matches.Position, matches.HammingDistance, matches.ClosenessPercentage, data.Nama, data.TempBC));
                            }


                            stopwatch.Stop();

                            if (bestMatches.Count > 0)
                            {
                                int nnn = bestMatches.Count;
                                MessageBox.Show($"{nnn}");

                                // Get the best match for the current ASCII representation
                                var bestMatch = bestMatches.OrderBy(m => m.HammingDistance).First();

                                List<string> matchedNames = new List<string>();
                                foreach (var item in BiodataData)
                                {
                                    if (alaiRegex.TestString2(bestMatch.Nama, item.Nama))
                                    { 
                                        matchedNames.Add(item.Nama);
                                    }
                                }                                

                                // MessageBox.Show($"Final best match found at position {bestMatch.Position} with closeness {bestMatch.ClosenessPercentage}% at id {bestMatch.Id} with nama {bestMatch.Nama}", "Final Best Match Found");
                                MessageBox.Show($"Final best match found at position {bestMatch.Position} with closeness {bestMatch.ClosenessPercentage}% at id {bestMatch.Id} with nama {bestMatch.Nama} and Image {bestMatch.TempBC}", "Final Best Match Found");

                                string imageFilePath = Path.Combine("../../../test/", bestMatch.TempBC);
                                // backendState.setPic2(imageFilePath);

                                // MessageBox.Show($"Result file: {System.IO.Path.GetFileName(backendState.getPic2())}");
                                // MessageBox.Show(backendState.getPic());
                                BitmapImage img = new BitmapImage();
                                img.BeginInit();
                                MessageBox.Show(imageFilePath);
                                img.UriSource = new Uri(imageFilePath, UriKind.Absolute);
                                img.EndInit();
                                selectedImageGay.Source = img;

                                if (matchedNames.Count > 0)
                                {
                                    MessageBox.Show($"First matched name: {matchedNames[0]}", "Matched Name", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("No matches found in biodata.", "No Match", MessageBoxButton.OK, MessageBoxImage.Information);
                                }

                                MessageBox.Show($"Time taken: {stopwatch.ElapsedMilliseconds} milliseconds");
                            }
                            else
                            {
                                MessageBox.Show("No matches found for this image.", "No Match", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please select an algorithm.", "Algorithm Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to convert to ASCII.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing image: {ex.Message}", "Processing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an image first.", "No Image Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private List<(int Id, string TempBC, string AsciiRepresentation, string Nama)> FetchDatabaseData()
        {
            List<(int Id, string TempBC, string AsciiRepresentation, string Nama)> data = new List<(int Id, string TempBC, string AsciiRepresentation, string Nama)>();
            string connectionString = $"Data Source=example2.db;Version=3;";
            string query = "SELECT id, berkas_citra, nama FROM sidik_jari";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["id"]);
                                string tempBC = reader["berkas_citra"].ToString();
                                string asciiRepresentation = ConvertFileToAscii(tempBC);
                                if (asciiRepresentation != null)
                                {
                                    string nama = reader["nama"].ToString();
                                    data.Add((id, tempBC, asciiRepresentation, nama));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return data;
        }


        private List<(string Nama, string TempatLahir, string JenisKelamin, string GolonganDarah, string Alamat, string StatusPerkawinan, string Pekerjaan, string Kewarganegaraan)> FetchBiodata()
        {
            List<(string Nama, string TempatLahir, string JenisKelamin, string GolonganDarah, string Alamat, string StatusPerkawinan, string Pekerjaan, string Kewarganegaraan)> data = new List<(string, string, string, string, string, string, string, string)>();
            // string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example.db");
            string connectionString = $"Data Source=example2.db;Version=3;";
            string query = "SELECT PK, nama, tempat_lahir, jenis_kelamin, golongan_darah, alamat, status_perkawinan, pekerjaan, kewarganegaraan FROM biodata";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string PK = reader["PK"].ToString();
                                string nama = reader["nama"].ToString();
                                string tempat_lahir = reader["tempat_lahir"].ToString();
                                string jenis_kelamin = reader["jenis_kelamin"].ToString();
                                string golongan_darah = reader["golongan_darah"].ToString();
                                string alamat = reader["alamat"].ToString();
                                string status_perkawinan = reader["status_perkawinan"].ToString();
                                string pekerjaan = reader["pekerjaan"].ToString();
                                string kewarganegaraan = reader["kewarganegaraan"].ToString();

                                data.Add((nama, tempat_lahir, jenis_kelamin, golongan_darah, alamat, status_perkawinan, pekerjaan, kewarganegaraan));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return data;
        }

        private string ConvertFileToAscii(string fileName)
        {
            // Assuming your files are in a "test" folder in the current directory
            // string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test", fileName);
            string filePath = Path.Combine("../../../test/", fileName);

            try
            {
                // Process the image
                string binaryRepresentation = FingerPrintConverter.ProcessImage(filePath);
                Console.WriteLine($"Binary representation: {binaryRepresentation}");

                string asciiRepresentation = FingerPrintConverter.BinaryToAscii(binaryRepresentation);
                Console.WriteLine($"ASCII representation: {asciiRepresentation}");

                return asciiRepresentation;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error converting file to ASCII: {ex.Message}", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private IStringSearchAlgorithm GetSearchAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "Knuth-Morris-Pratt":
                    return new KMP();
                case "Boyer-Moore":
                    return new BoyerMoore();
                default:
                    throw new ArgumentException($"Unsupported algorithm: {algorithm}");
            }
        }
    }
}
