using System;
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
using System.Windows.Input;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Backend backendState;
        private Results res;
        private Dictionary<string, (int Position, int HammingDistance, double ClosenessPercentage)> bestMatchesDict = new Dictionary<string, (int, int, double)>();
        private AlaiRegex alaiRegex = new AlaiRegex(); // Added instance of AlaiRegex
        private InfoWindow iW;

        public MainWindow()
        {
            backendState = new Backend();
            res = new Results();
            InitializeComponent();

            string sqlFilePath = "./sql/try1.sql";
            string dbFilePath = "./demo/try1.db";

            bool success = CreateDatabaseFromSqlFile(sqlFilePath, dbFilePath);

            if (success)
            {
                MessageBox.Show("Database created successfully.");
            }
            else
            {
                MessageBox.Show("Failed to create database.");
            }
        }

        static bool CreateDatabaseFromSqlFile(string sqlFilePath, string dbFilePath)
        {
            try
            {
                // Ensure the SQL file exists
                if (!File.Exists(sqlFilePath))
                {
                    MessageBox.Show("SQL file does not exist.");
                    return false;
                }

                // Read the SQL file
                string sqlCommands = File.ReadAllText(sqlFilePath);

                // Create a new SQLite database file
                SQLiteConnection.CreateFile(dbFilePath);

                // Open a connection to the new database
                using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    connection.Open();

                    // Execute the SQL commands
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sqlCommands;
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                return true; // Indicate success
            }
            catch (SQLiteException sqlex)
            {
                MessageBox.Show($"SQLite error: {sqlex.Message}");
                return false; // Indicate failure
            }
            catch (FileNotFoundException fnfex)
            {
                MessageBox.Show($"File not found: {fnfex.Message}");
                return false; // Indicate failure
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false; // Indicate failure
            }
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
                // MessageBox.Show(backendState.getPic());
                selectedImage.Source = img;

            }
        }

        private void comboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            backendState.setAlgo(selectedItem.Content.ToString());
            // MessageBox.Show($"Selected algorithm: {backendState.getAlgo()}");
        }

        private async void searchGo(object sender, RoutedEventArgs e)
        {
            if (backendState.getPic() != null)
            {
                string imagePath = backendState.getPic();
        
                try
                {
                    // Show the loading overlay
                    loadingOverlay.Visibility = Visibility.Visible;
        
                    await Task.Run(() =>
                    {
                        // Process the image
                        string binaryRepresentation = FingerPrintConverter.ProcessImage(imagePath);
                        // Console.WriteLine($"Binary representation: {binaryRepresentation}");
        
                        string midDigs = FingerPrintConverter.GetMiddleDigits(binaryRepresentation, 128);
        
                        string asciiRepresentation = FingerPrintConverter.BinaryToAscii(midDigs);
                        // Console.WriteLine($"ASCII representation: {asciiRepresentation}");
        
                        if (!string.IsNullOrEmpty(asciiRepresentation))
                        {

                            Stopwatch stopwatch2 = Stopwatch.StartNew();

                            // Reset the dictionary before starting the search
                            bestMatchesDict.Clear();
        
                            // Fetch database data
                            List<(string TempBC, string AsciiRepresentation, string Nama)> databaseData = FetchDatabaseData();
                            List<(string PK, string Nama, string TempatLahir, string JenisKelamin, string GolonganDarah, string Alamat, string StatusPerkawinan, string Pekerjaan, string Kewarganegaraan)> BiodataData = FetchBiodata();
        
                            // Perform search for the current ASCII representation
                            IStringSearchAlgorithm algorithm = GetSearchAlgorithm(backendState.getAlgo());
                            if (algorithm != null)
                            {
                                List<(int Position, int HammingDistance, double ClosenessPercentage, string Nama, string TempBC)> bestMatches = new List<(int, int, double, string, string)>();
        
                                Stopwatch stopwatch = Stopwatch.StartNew();
        
                                foreach (var data in databaseData)
                                {
                                    var matches = algorithm.Search(data.AsciiRepresentation, asciiRepresentation).First();
                                    bestMatches.Add((matches.Position, matches.HammingDistance, matches.ClosenessPercentage, data.Nama, data.TempBC));
                                }
        
                                stopwatch.Stop();
                                stopwatch2.Stop();
        
                                if (bestMatches.Count > 0)
                                {
                                    var bestMatch = bestMatches.OrderBy(m => m.HammingDistance).First();
                                    List<string> matchedNames = new List<string>();
        
                                    foreach (var item in BiodataData)
                                    {
                                        if (alaiRegex.TestString2(bestMatch.Nama, item.Nama))
                                        {
                                            matchedNames.Add(item.Nama);
                                            res.setName(bestMatch.Nama + " FROM " + item.Nama);
                                            res.setNik(item.PK);
                                            res.setBirthLoc(item.TempatLahir);
                                            res.setGender(item.JenisKelamin);
                                            res.setBloodType(item.GolonganDarah);
                                            res.setAddress(item.Alamat);
                                            res.setMarriageStatus(item.StatusPerkawinan);
                                            res.setJob(item.Pekerjaan);
                                            res.setCitizenship(item.Kewarganegaraan);
                                            res.setExecTime(stopwatch.ElapsedMilliseconds);
                                            res.setMatchPercentage(bestMatch.ClosenessPercentage);
                                        }
                                    }
        
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        // MessageBox.Show($"Final best match found at position {bestMatch.Position} with closeness {bestMatch.ClosenessPercentage}% at id {bestMatch.Id} with nama {bestMatch.Nama} and Image {bestMatch.TempBC}", "Final Best Match Found");
        
                                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                        int srcIndex = baseDirectory.LastIndexOf("src");
        
                                        if (srcIndex != -1)
                                        {
                                            string rootPath = baseDirectory.Substring(0, srcIndex);
                                            string relativePath = Path.Combine(rootPath, "test", bestMatch.TempBC);
                                            string combinedPath = Path.GetFullPath(relativePath);
        
                                            if (File.Exists(combinedPath))
                                            {
                                                BitmapImage matchedImg = new BitmapImage();
                                                matchedImg.BeginInit();
                                                matchedImg.UriSource = new Uri(combinedPath, UriKind.Absolute);
                                                matchedImg.EndInit();
                                                selectedImageGay.Source = matchedImg;
                                                TimeRun.Text = res.getExecTime().ToString() + " ms";
                                                Percentage.Text = res.getMatchPercentage().ToString() + " %";
                                            }
                                            else
                                            {
                                                MessageBox.Show("Matched image file not found.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("The path does not contain 'src'.", "Path Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                        MessageBox.Show($"Time taken (including converting database image): {stopwatch2.ElapsedMilliseconds / 1000} seconds");
                                        // MessageBox.Show($"Time taken: {stopwatch.ElapsedMilliseconds} milliseconds");
                                    });
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        MessageBox.Show("No matches found for this image.", "No Match", MessageBoxButton.OK, MessageBoxImage.Information);
                                    });
                                }
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show("Please select an algorithm.", "Algorithm Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                });
                            }
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Failed to convert to ASCII.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing image: {ex.Message}", "Processing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // Hide the loading overlay
                    loadingOverlay.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MessageBox.Show("Please select an image first.", "No Image Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private List<(string TempBC, string AsciiRepresentation, string Nama)> FetchDatabaseData()
        {
            List<(string TempBC, string AsciiRepresentation, string Nama)> data = new List<(string TempBC, string AsciiRepresentation, string Nama)>();
            string connectionString = $"Data Source=./demo/try1.db;Version=3;";
            string query = "SELECT berkas_citra, nama FROM sidik_jari";

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
                                string tempBC = reader["berkas_citra"].ToString();
                                string asciiRepresentation = ConvertFileToAscii(tempBC);
                                if (asciiRepresentation != null)
                                {
                                    string nama = reader["nama"].ToString();
                                    data.Add((tempBC, asciiRepresentation, nama));
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


        private List<(string PK, string Nama, string TempatLahir, string JenisKelamin, string GolonganDarah, string Alamat, string StatusPerkawinan, string Pekerjaan, string Kewarganegaraan)> FetchBiodata()
        {
            List<(string PK, string Nama, string TempatLahir, string JenisKelamin, string GolonganDarah, string Alamat, string StatusPerkawinan, string Pekerjaan, string Kewarganegaraan)> data = new List<(string, string, string, string, string, string, string, string, string)>();
            // string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example.db");
            string connectionString = $"Data Source=./demo/try1.db;Version=3;";
            string query = "SELECT NIK, nama, tempat_lahir, jenis_kelamin, golongan_darah, alamat, status_perkawinan, pekerjaan, kewarganegaraan FROM biodata";

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

                                data.Add((PK, nama, tempat_lahir, jenis_kelamin, golongan_darah, alamat, status_perkawinan, pekerjaan, kewarganegaraan));
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
            string filePath = Path.Combine("../../test/", fileName);

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

        private void GoToInfo(object sender, MouseButtonEventArgs e)
        {
            if (iW != null)
            {
                iW.Close();
            }
            iW = new InfoWindow(res);
            iW.Show();
        }
    }
}
