using System;
using System.Collections.Generic;
using System.Linq;

class BoyerMoore
{
    private readonly Dictionary<char, int> _badCharacterShift;
    private int[] _goodSuffixShift;
    private int[] _suffixes;

    public BoyerMoore()
    {
        _badCharacterShift = new Dictionary<char, int>();
    }

    private void PreprocessBadCharacter(string pattern)
    {
        _badCharacterShift.Clear();
        for (int i = 0; i < pattern.Length; i++)
        {
            char currentChar = pattern[i];
            if (!_badCharacterShift.ContainsKey(currentChar))
            {
                _badCharacterShift[currentChar] = pattern.Length - i - 1;
            }
        }
    }

    private void PreprocessGoodSuffix(string pattern)
    {
        _goodSuffixShift = new int[pattern.Length + 1];
        _suffixes = new int[pattern.Length + 1];

        int i = pattern.Length;
        int j = pattern.Length + 1;
        _suffixes[i] = j;

        while (i > 0)
        {
            while (j <= pattern.Length && pattern[i - 1] != pattern[j - 1])
            {
                if (_goodSuffixShift[j] == 0)
                    _goodSuffixShift[j] = j - i;
                j = _suffixes[j];
            }
            i--;
            j--;
            _suffixes[i] = j;
        }

        j = _suffixes[0];
        for (i = 0; i <= pattern.Length; i++)
        {
            if (_goodSuffixShift[i] == 0)
                _goodSuffixShift[i] = j;
            if (i == j)
                j = _suffixes[j];
        }
    }

    public List<(int Position, int HammingDistance, double ClosenessPercentage)> Search(string text, string pattern)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty.");
        if (string.IsNullOrEmpty(pattern))
            throw new ArgumentException("Pattern cannot be null or empty.");

        PreprocessBadCharacter(pattern);
        PreprocessGoodSuffix(pattern);

        List<(int Position, int HammingDistance, double ClosenessPercentage)> results = new List<(int Position, int HammingDistance, double ClosenessPercentage)>();
        int i = 0;
        while (i <= text.Length - pattern.Length)
        {
            int j = pattern.Length - 1;
            while (j >= 0 && pattern[j] == text[i + j])
                j--;
            if (j < 0)
            {
                results.Add((i, 0, 100.0)); // Exact match
                i += _goodSuffixShift[0];
            }
            else
            {
                char badChar = text[i + j];
                int badCharShift = _badCharacterShift.ContainsKey(badChar) ? _badCharacterShift[badChar] : pattern.Length;

                int hammingDistance = CalculateHammingDistance(pattern, text.Substring(i, pattern.Length));
                double closenessPercentage = CalculateClosenessPercentage(hammingDistance, pattern.Length);
                results.Add((i, hammingDistance, closenessPercentage));

                i += Math.Max(_goodSuffixShift[j + 1], badCharShift - pattern.Length + 1 + j);
            }
        }

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