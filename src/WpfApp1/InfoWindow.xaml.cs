using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private Results shows;
        public InfoWindow(Results shows)
        {
            InitializeComponent();
            if (shows != null)
            {
                this.shows = shows;
                NIKRes.Text = shows.getNik();
                NamaRes.Text = shows.getName();
                BornRes.Text = shows.getBirthLoc();
                GenderRes.Text = shows.getGender();
                BloodRes.Text = shows.getBloodType();
                AddressRes.Text = shows.getAddress();
                StatusRes.Text = shows.getMarriageStatus();
                JobRes.Text = shows.getJob();
                CitiRes.Text = shows.getCitizenship();
            }
        }
    }
}
