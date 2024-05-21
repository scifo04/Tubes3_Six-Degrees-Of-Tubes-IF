// using System;
// using System.Drawing;
// using System.IO;
// using System.Text;

// class FingerPrintConverter
// {
//     static void Main(string[] args)
//     {
//         // Specify the directory containing the .dib files
//         string directoryPath = @"assets";

//         if (!Directory.Exists(directoryPath))
//         {
//             Console.WriteLine("Directory not found: " + directoryPath);
//             return;
//         }

//         string[] imageFiles = Directory.GetFiles(directoryPath, "*.dib");

//         if (imageFiles.Length < 2)
//         {
//             Console.WriteLine("At least two .dib files are required in the directory: " + directoryPath);
//             return;
//         }

//         string firstImagePath = imageFiles[0];
//         string secondImagePath = imageFiles[1];

//         try
//         {
//             // Process the first image
//             string firstAsciiString = ProcessImage(firstImagePath);
//             if (!string.IsNullOrEmpty(firstAsciiString))
//             {
//                 Console.WriteLine($"ASCII representation of {firstImagePath}:");
//                 Console.WriteLine(firstAsciiString);
//                 Console.WriteLine();

//                 string ASCII_1 = BinaryToAscii(firstAsciiString);
//                 Console.WriteLine("\n");
//                 Console.WriteLine(ASCII_1);
//                 Console.WriteLine();
//             }

//             // Process the second image
//             string secondAsciiString = ProcessImage(secondImagePath);
//             if (!string.IsNullOrEmpty(secondAsciiString))
//             {
//                 Console.WriteLine($"ASCII representation of {secondImagePath}:");
//                 Console.WriteLine(secondAsciiString);
//                 Console.WriteLine();

//                 // Take the middle 50 digits of the binary image and convert to ASCII
//                 string middleDigits = GetMiddleDigits(secondAsciiString, 128);
//                 string middleAscii = BinaryToAscii(middleDigits);

//                 // Display the middle digits in binary and ASCII
//                 Console.WriteLine($"Middle 50 digits (binary) of {secondImagePath}:");
//                 Console.WriteLine(middleDigits);
//                 Console.WriteLine();

//                 Console.WriteLine($"Middle 50 digits (ASCII) of {secondImagePath}:");
//                 Console.WriteLine(middleAscii);
//                 Console.WriteLine();
//             }
//         }
//         catch (Exception ex) when (ex is FileNotFoundException || ex is IOException)
//         {
//             Console.WriteLine($"Error processing files: {ex.Message}");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Unexpected error: {ex.Message}");
//         }
//     }

//     static string ProcessImage(string imagePath)
//     {
//         string asciiString = null;

//         try
//         {
//             using (Bitmap grayscaleImage = new Bitmap(imagePath))
//             {
//                 Bitmap binaryImage = ToBinary(grayscaleImage);
//                 asciiString = BinaryToAscii(binaryImage);
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error processing {imagePath}: {ex.Message}");
//         }

//         return asciiString;
//     }

//     static Bitmap ToBinary(Bitmap grayscaleImage)
//     {
//         Bitmap binaryImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);

//         for (int x = 0; x < grayscaleImage.Width; x++)
//         {
//             for (int y = 0; y < grayscaleImage.Height; y++)
//             {
//                 Color pixel = grayscaleImage.GetPixel(x, y);
//                 int grayValue = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);

//                 Color binaryColor = grayValue < 128 ? Color.Black : Color.White;
//                 binaryImage.SetPixel(x, y, binaryColor);
//             }
//         }

//         return binaryImage;
//     }

//     static string BinaryToAscii(Bitmap binaryImage)
//     {
//         StringBuilder asciiBuilder = new StringBuilder();

//         for (int y = 0; y < binaryImage.Height; y++)
//         {
//             for (int x = 0; x < binaryImage.Width; x++)
//             {
//                 Color pixel = binaryImage.GetPixel(x, y);

//                 // Convert black/white to ASCII character
//                 if (pixel.R == 0)
//                     asciiBuilder.Append("1");
//                 else
//                     asciiBuilder.Append("0");
//             }
//         }

//         return asciiBuilder.ToString();
//     }

//     static string GetMiddleDigits(string asciiString, int count)
//     {
//         int startIdx = (asciiString.Length / 2 - count / 2) / 8 * 8 + 8;
//         string middleDigits = asciiString.Substring(startIdx, count);
//         return middleDigits;
//     }


//     static string BinaryToAscii(string binaryString)
//     {
//         if (string.IsNullOrEmpty(binaryString))
//             throw new ArgumentException("Binary string cannot be null or empty.");

//         StringBuilder textBuilder = new StringBuilder();

//         for (int i = 0; i < binaryString.Length; i += 8)
//         {
//             // Ensure there are at least 8 bits remaining to convert
//             if (i + 8 <= binaryString.Length)
//             {
//                 string binary = binaryString.Substring(i, 8);
//                 int intValue = Convert.ToInt32(binary, 2);
//                 textBuilder.Append((char)intValue);
//             }
//         }

//         return textBuilder.ToString();
//     }
// }
