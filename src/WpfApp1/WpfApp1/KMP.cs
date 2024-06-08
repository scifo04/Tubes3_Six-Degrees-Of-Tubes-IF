using System;
using System.Data;
using System.Globalization;

public class KMP : IStringSearchAlgorithm
{

    private int[] borderfunction;

    public KMP() {
        borderfunction = new int[0];
    }
    public string getPrefix(string kata, int length, int idx)
    {
        int count = 0;
        string result = "";
        while (result.Length < length)
        {
            result = result + kata[count];
            count += 1;
        }
        /*Console.WriteLine("Result: " + result);*/
        return result;
    }

    public string getSuffix(string kata, int length, int idx)
    {
        int count = idx;
        string result = "";
        while (result.Length < length)
        {
            result = kata[count] + result;
            count -= 1;
        }
        /*Console.WriteLine("Result: " + result);*/
        return result;
    }

    public int getSize(string kata, int idx)
    {
        for (int i = idx; i > 0; i--)
        {
            string prefix = getPrefix(kata, i, idx);
            string suffix = getSuffix(kata, i, idx);
            Console.WriteLine("Prefix: " + prefix);
            Console.WriteLine("Suffix: " + suffix);
            if (prefix == suffix)
            {
                return prefix.Length;
            }
        }

        return 0;
    }

    public void getborderfunction(string pattern) {
        borderfunction = new int[pattern.Length - 1];
        for (int i = 1; i < pattern.Length; i++)
        {
            Console.WriteLine("Index: " + (i - 1));
            borderfunction[i - 1] = getSize(pattern, i - 1);
        }
    }

    public IEnumerable<(int Position, int HammingDistance, double ClosenessPercentage)> Search(string text, string pattern)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty.");
        if (string.IsNullOrEmpty(pattern))
            throw new ArgumentException("Pattern cannot be null or empty.");

        getborderfunction(pattern);

        string katalengkap = text;
        string kata = pattern;

        List<(int Position, int HammingDistance, double ClosenessPercentage)> results = new List<(int Position, int HammingDistance, double ClosenessPercentage)>();
        int i = 0;
        int idxkata = 0;
        int idxkatalengkap = 0;
        int position = 0;
        int hammming = kata.Length - 1;
        bool found = false;
        while (found == false && idxkatalengkap < katalengkap.Length)
        {
            if (kata[idxkata] == katalengkap[idxkatalengkap])
            {
                if (idxkata == kata.Length - 1)
                {
                    /*Console.WriteLine("Huruf kata lengkap: " + katalengkap[idxkatalengkap]);
                    Console.WriteLine("Huruf kata: " + kata[idxkata]);
                    Console.WriteLine("Kata ditemukan di index: " + (idxkatalengkap));*/
                    position = idxkatalengkap - idxkata;
                    found = true;
                    results.Add((position, 0, 100.0));
                    return results;
                }
                else
                {
                    /*Console.WriteLine("Huruf kata lengkap: " + katalengkap[idxkatalengkap]);
                    Console.WriteLine("Huruf kata: " + kata[idxkata])*/;
                    idxkata += 1;
                    idxkatalengkap += 1;
                }
            }
            else
            {
                /*Console.WriteLine("Huruf kata lengkapa: " + katalengkap[idxkatalengkap]);
                Console.WriteLine("Huruf kataa: " + kata[idxkata]);*/
                if (idxkata == 0)
                {
                    idxkatalengkap += 1;
                }
                else
                {
                    int gethammming = kata.Length - idxkata;
                    if (gethammming < hammming) {
                        hammming = gethammming;
                        position = idxkatalengkap - idxkata;
                    }
                    idxkata = borderfunction[idxkata - 1];
                }
            }
        }
        if (found == false)
        {
            results.Add((position, hammming, 100 - (double)hammming / kata.Length * 100));
        }
        return results;
    }
}