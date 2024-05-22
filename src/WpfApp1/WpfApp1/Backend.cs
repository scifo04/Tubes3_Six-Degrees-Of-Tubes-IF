using System;
using System.Windows;
using System.IO;

namespace WpfApp1
{
    public class Backend
    {
        private string chosenPic;
        private string chosenAlgo;

        public Backend()
        {
            chosenPic = "";
            chosenAlgo = "";
        }
        public void run()
        {
            MessageBox.Show($"Searching matches for {Path.GetFileName(getPic())} with {getAlgo()} algorithm");
        }

        public string getPic()
        {
            return chosenPic;
        }

        public void setPic(string pic)
        {
            chosenPic = pic;
        }

        public string getAlgo()
        {
            return chosenAlgo;
        }

        public void setAlgo(string algo) { 
            chosenAlgo = algo;
        }
    }
}