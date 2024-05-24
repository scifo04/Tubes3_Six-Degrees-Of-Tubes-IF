using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
                        List<(int Id, string AsciiRepresentation)> databaseData = FetchDatabaseData();

                        MessageBox.Show("cock");
        
                        // Perform search for the current ASCII representation
                        BoyerMoore bm = new BoyerMoore();
                        List<(int Id, int Position, int HammingDistance, double ClosenessPercentage)> bestMatches = new List<(int Id, int Position, int HammingDistance, double ClosenessPercentage)>();
        
                        Stopwatch stopwatch = Stopwatch.StartNew();
        
                        foreach (var data in databaseData)
                        {
                            var matches = bm.Search(data.AsciiRepresentation, asciiRepresentation).First();
                            bestMatches.Add((data.Id, matches.Position, matches.HammingDistance, matches.ClosenessPercentage));
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
        
                            // Display the final best match found
                            MessageBox.Show($"Final best match found at position {bestMatch.Position} with closeness {bestMatch.ClosenessPercentage}% at id {bestMatch.Id}", "Final Best Match Found");
        
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

        private List<(int Id, string AsciiRepresentation)> FetchDatabaseData()
        {
            List<(int Id, string AsciiRepresentation)> data = new List<(int Id, string AsciiRepresentation)>();
            // string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example.db");
            string connectionString = $"Data Source=example.db;Version=3;";
            string query = "SELECT id, berkas_citra FROM sidik_jari";

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
                                // int id = reader.GetInt32("id");
                                int id = Convert.ToInt32(reader["id"]);
                                string tempBC = reader["berkas_citra"].ToString();
                                string asciiRepresentation = ConvertFileToAscii(tempBC);
                                if (asciiRepresentation != null)
                                {
                                    data.Add((id, asciiRepresentation));
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
            // string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataset", fileName);
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
    }
}
