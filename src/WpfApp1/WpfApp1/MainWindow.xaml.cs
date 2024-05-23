using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Backend backendState;

        private Dictionary<string, (int Position, int HammingDistance, double ClosenessPercentage)> bestMatchesDict = new Dictionary<string, (int, int, double)>();

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

                    string midDigs = FingerPrintConverter.GetMiddleDigits(binaryRepresentation, 128);

                    string asciiRepresentation = FingerPrintConverter.BinaryToAscii(midDigs);
                    Console.WriteLine($"ASCII representation: {asciiRepresentation}");

                    if (!string.IsNullOrEmpty(asciiRepresentation))
                    {
                        // Reset the dictionary before starting the search
                        bestMatchesDict.Clear();

                        // Fetch database data
                        List<string> databaseData = FetchDatabaseData();

                        // Perform search for the current ASCII representation
                        BoyerMoore bm = new BoyerMoore();
                        List<(int Position, int HammingDistance, double ClosenessPercentage)> bestMatches = new List<(int Position, int HammingDistance, double ClosenessPercentage)>();

                        Stopwatch stopwatch = Stopwatch.StartNew();

                        foreach (string data in databaseData)
                        {
                            var matches = bm.Search(data, asciiRepresentation).First();
                            bestMatches.Add(matches);
                        }

                        stopwatch.Stop();

                        
                        if (bestMatches.Count > 0)
                        {
                            int nnn = 0;
                            foreach (var match in bestMatches)
                            {
                                nnn += 1;
                                // MessageBox.Show($"Position: {match.Position}, Hamming Distance: {match.HammingDistance}, Closeness Percentage: {match.ClosenessPercentage}");
                            }

                            MessageBox.Show($"{nnn}");

                            // Get the best match for the current ASCII representation
                            var bestMatch = bestMatches.OrderBy(m => m.HammingDistance).First();

                            // Store the best match in the dictionary with the ASCII representation as key
                            // bestMatchesDict[asciiRepresentation] = bestMatch;

                            // Display the best match found so far
                            // MessageBox.Show($"Best match found at position {bestMatch.Position} with closeness {bestMatch.ClosenessPercentage}%", "Match Found");

                            // Now, find the overall best match from the dictionary
                            // var finalBestMatch = bestMatchesDict.OrderBy(kv => kv.Value.HammingDistance).FirstOrDefault().Value;

                            // Display the final best match found
                            MessageBox.Show($"Final best match found at position {bestMatch.Position} with closeness {bestMatch.ClosenessPercentage}%", "Final Best Match Found");

                            MessageBox.Show($"Time taken: {stopwatch.ElapsedMilliseconds} milliseconds");
                        }
                        else
                        {
                            MessageBox.Show("No matches found for this image.", "No Match", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string connectionString = "Server=localhost;port=1234;Database=trystima2;Uid=root;Pwd=Ma17urungh3bat;Max Pool Size=100;Connect Timeout=1000;";
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
                                string asciiRepresentation = reader["berkas_citra"].ToString();
                                // string asciiRepresentation = ConvertFileToAscii(fileName);
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
                string binaryRepresentation = FingerPrintConverter.ProcessImage(filePath);
                Console.WriteLine($"Binary representation: {binaryRepresentation}");

                // MessageBox.Show($"{binaryRepresentation}");

                string asciiRepresentation = FingerPrintConverter.BinaryToAscii(binaryRepresentation);
                Console.WriteLine($"ASCII representation: {asciiRepresentation}");

                // Fetch database data
                // List<string> databaseData = FetchDatabaseData();

                // // Perform search for the current ASCII representation
                // BoyerMoore bm = new BoyerMoore();
                // List<(int Position, int HammingDistance, double ClosenessPercentage)> bestMatches = new List<(int Position, int HammingDistance, double ClosenessPercentage)>();

                // foreach (string data in databaseData)
                // {
                //     var matches = bm.Search(data, asciiRepresentation);
                //     bestMatches.AddRange(matches);
                // }

                // if (bestMatches.Count > 0)
                // {
                //     // Get the best match for the current ASCII representation
                //     var bestMatch = bestMatches.OrderBy(m => m.HammingDistance).First();

                //     // Store the best match in the dictionary with the ASCII representation as key
                //     bestMatchesDict[asciiRepresentation] = bestMatch;
                // }
                // else
                // {
                //     MessageBox.Show("No matches found for this file.", "No Match", MessageBoxButton.OK, MessageBoxImage.Information);
                // }

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
