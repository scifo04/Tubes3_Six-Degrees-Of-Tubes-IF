using System;
using System.Drawing;
using System.Text;

namespace WpfApp1
{
    public static class FingerPrintConverter
    {
        public static string ProcessImage(string imagePath)
        {
            string binaryString = null;

            try
            {
                using (Bitmap grayscaleImage = new Bitmap(imagePath))
                {
                    Bitmap binaryImage = ToBinary(grayscaleImage);
                    binaryString = BinaryToAsciiBit(binaryImage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {imagePath}: {ex.Message}");
            }

            return binaryString;
        }

        private static Bitmap ToBinary(Bitmap grayscaleImage)
        {
            Bitmap binaryImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);

            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    Color pixel = grayscaleImage.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);

                    Color binaryColor = grayValue < 128 ? Color.Black : Color.White;
                    binaryImage.SetPixel(x, y, binaryColor);
                }
            }

            return binaryImage;
        }

        private static string BinaryToAsciiBit(Bitmap binaryImage)
        {
            StringBuilder asciiBuilder = new StringBuilder();

            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    Color pixel = binaryImage.GetPixel(x, y);

                    // Convert black/white to ASCII character
                    if (pixel.R == 0)
                        asciiBuilder.Append("1");
                    else
                        asciiBuilder.Append("0");
                }
            }

            return asciiBuilder.ToString();
        }

        public static string GetMiddleDigits(string binaryString, int count)
        {
            int startIdx = (binaryString.Length / 2 - count / 2) / 8 * 8 + 8;
            string middleDigits = binaryString.Substring(startIdx, count);
            return middleDigits;
        }

        public static string BinaryToAscii(string binaryString)
        {
            if (string.IsNullOrEmpty(binaryString))
                throw new ArgumentException("Binary string cannot be null or empty.");

            StringBuilder textBuilder = new StringBuilder();

            for (int i = 0; i < binaryString.Length; i += 8)
            {
                // Ensure there are at least 8 bits remaining to convert
                if (i + 8 <= binaryString.Length)
                {
                    string binary = binaryString.Substring(i, 8);
                    int intValue = Convert.ToInt32(binary, 2);
                    textBuilder.Append((char)intValue);
                }
            }

            return textBuilder.ToString();
        }
    }
}
