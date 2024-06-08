using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    public class Backend : INotifyPropertyChanged
    {
        private string chosenPic;
        private string chosenPic2;
        private string chosenAlgo;

        public event PropertyChangedEventHandler PropertyChanged;

        public Backend()
        {
            chosenPic = "";
            chosenPic2 = "";
            chosenAlgo = "";
        }

        public string ChosenPic
        {
            get { return chosenPic; }
            set
            {
                chosenPic = value;
                OnPropertyChanged();
            }
        }

        public string ChosenPic2
        {
            get { return chosenPic2; }
            set
            {
                chosenPic2 = value;
                OnPropertyChanged();
            }
        }

        public string ChosenAlgo
        {
            get { return chosenAlgo; }
            set
            {
                chosenAlgo = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void setPic(string pic)
        {
            ChosenPic = pic;
        }

        public void setPic2(string pic)
        {
            ChosenPic2 = pic;
        }

        public string getPic()
        {
            return ChosenPic;
        }

        public string getPic2()
        {
            return ChosenPic2;
        }

        public string getAlgo()
        {
            return ChosenAlgo;
        }

        public void setAlgo(string algo)
        {
            ChosenAlgo = algo;
        }
    }
}
