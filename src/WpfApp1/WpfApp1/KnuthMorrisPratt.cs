using System;
using System.Collections.Generic;
using System.Linq;

public class KnuthMorrisPratt : IStringSearchAlgorithm
{
    private int[] ComputePrefixFunction(string pattern)
    {
        int[] pi = new int[pattern.Length];
        int k = 0;
        for (int q = 1; q < pattern.Length; q++)
        {
            while (k > 0 && pattern[k] != pattern[q])
            {
                k = pi[k - 1];
            }
            if (pattern[k] == pattern[q])
            {
                k++;
            }
            pi[q] = k;
        }
        return pi;
    }

    public IEnumerable<(int Position, int HammingDistance, double ClosenessPercentage)> Search(string text, string pattern)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty.");
        if (string.IsNullOrEmpty(pattern))
            throw new ArgumentException("Pattern cannot be null or empty.");

        List<(int Position, int HammingDistance, double ClosenessPercentage)> results = new List<(int Position, int HammingDistance, double ClosenessPercentage)>();

        int[] pi = ComputePrefixFunction(pattern);
        int m = pattern.Length;
        int n = text.Length;
        int q = 0; // Number of characters matched

        for (int i = 0; i < n; i++)
        {
            while (q > 0 && pattern[q] != text[i])
            {
                q = pi[q - 1];
            }
            if (pattern[q] == text[i])
            {
                q++;
            }
            if (q == m)
            {
                int start = i - m + 1;
                int hammingDistance = CalculateHammingDistance(pattern, text.Substring(start, m));
                double closenessPercentage = CalculateClosenessPercentage(hammingDistance, m);
                results.Add((start, hammingDistance, closenessPercentage));
                q = pi[q - 1];
            }
        }

        // Filter results to keep only those with the best Hamming distance
        int minHammingDistance = results.Min(r => r.HammingDistance);
        results = results.Where(r => r.HammingDistance == minHammingDistance).ToList();

        return results;
    }

    private int CalculateHammingDistance(string str1, string str2)
    {
        if (str1.Length != str2.Length)
            throw new ArgumentException("Strings must be of the same length");

        int distance = 0;
        for (int i = 0; i < str1.Length; i++)
        {
            if (str1[i] != str2[i])
                distance++;
        }
        return distance;
    }

    private double CalculateClosenessPercentage(int hammingDistance, int length)
    {
        return (1 - (double)hammingDistance / length) * 100;
    }
}
