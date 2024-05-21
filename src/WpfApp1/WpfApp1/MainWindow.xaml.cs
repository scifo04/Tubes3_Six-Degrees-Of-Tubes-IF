using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            Button clicked = (Button)sender;
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
                img.UriSource = new Uri(backendState.getPic(),UriKind.Absolute);
                img.EndInit();
                selectedImage.Source = img;
            }
        }

        private void comboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            backendState.setAlgo(selectedItem.Content.ToString());
            MessageBox.Show($"Selected algorithm: {backendState.getAlgo()}"); ;
        }

        private void searchGo(object sender, RoutedEventArgs e)
        {
            backendState.run();
        }
    }
}