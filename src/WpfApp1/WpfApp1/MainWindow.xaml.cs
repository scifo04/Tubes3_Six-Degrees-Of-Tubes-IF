using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Backend backendState;

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
                MessageBox.Show($"Selected file: {System.IO.Path.GetFileName(backendState.getPic())}");

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(backendState.getPic(), UriKind.Absolute);
                img.EndInit();
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

                    string asciiRepresentation = FingerPrintConverter.BinaryToAscii(binaryRepresentation);
                    Console.WriteLine($"ASCII representation: {asciiRepresentation}");

                    if (!string.IsNullOrEmpty(asciiRepresentation))
                    {
                        List<string> databaseData = FetchDatabaseData();

                        BoyerMoore bm = new BoyerMoore();
                        List<(int Position, int HammingDistance, double ClosenessPercentage)> bestMatches = new List<(int Position, int HammingDistance, double ClosenessPercentage)>();

                        foreach (string data in databaseData)
                        {
                            var matches = bm.Search(data, asciiRepresentation);
                            bestMatches.AddRange(matches);
                        }

                        if (bestMatches.Count > 0)
                        {
                            var bestMatch = bestMatches.OrderBy(m => m.HammingDistance).First();
                            MessageBox.Show($"Best match found at position {bestMatch.Position} with closeness {bestMatch.ClosenessPercentage}%", "Match Found");

                            BitmapImage imgay = new BitmapImage();
                            imgay.BeginInit();
                            imgay.UriSource = new Uri(backendState.getPic(), UriKind.Absolute);
                            imgay.EndInit();
                            selectedImageGay.Source = imgay;
                            TimeRun.Text = "40ms";  // Example value, replace with actual time if needed
                            Percentage.Text = $"{bestMatch.ClosenessPercentage}%";
                        }
                        else
                        {
                            MessageBox.Show("No close matches found.", "No Match", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private List<string> FetchDatabaseData()
        {
            List<string> data = new List<string>();

            // Replace with your MySQL connection string
            string connectionString = "Server=localhost;port=1234;Database=tubes3_stima24;Uid=root;Pwd=Ma17urungh3bat;";
            string query = "SELECT berkas_citra FROM sidik_jari";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string fileName = reader["berkas_citra"].ToString();
                                string asciiRepresentation = ConvertFileToAscii(fileName);
                                if (asciiRepresentation != null)
                                {
                                    data.Add(asciiRepresentation);
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

        private string ConvertFileToAscii(string fileName)
        {
            string filePath = Path.Combine("../../../test/dataset", fileName);

            try
            {
                // Process the image
                // MessageBox.Show($"{filePath}");

                string binaryRepresentation = FingerPrintConverter.ProcessImage(filePath);
                Console.WriteLine($"Binary representation: {binaryRepresentation}");

                // MessageBox.Show($"{binaryRepresentation}");

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
    }
}
