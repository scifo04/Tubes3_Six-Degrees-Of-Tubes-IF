using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NameCleanser
{
    private static Dictionary<char, char> numberToCharMap = new Dictionary<char, char>()
    {
        {'1', 'i'},
        {'4', 'a'},
        {'6', 'g'},
        {'0', 'o'},
        {'3', 'e'},
        {'5', 's'},
        {'7', 't'}
    };

    public static string CorrectWord(string corrupted)
    {
        // Step 1: Normalize case
        string normalized = corrupted.ToLower();

        // Step 2: Replace numbers with corresponding letters
        foreach (var pair in numberToCharMap)
        {
            normalized = normalized.Replace(pair.Key, pair.Value);
        }

        // Step 3: Remove shorteners using regex
        string pattern = "[aeiou]";
        string shortened = Regex.Replace(normalized, pattern, "");

        // Step 4: Find the closest match using basic correction
        string bestMatch = null;
        int bestMatchScore = int.MaxValue;

        foreach (var word in CommonEnglishWords.Words)
        {
            int score = LevenshteinDistance(word, shortened);
            if (score < bestMatchScore)
            {
                bestMatchScore = score;
                bestMatch = word;
            }
        }

        return bestMatch ?? corrupted;
    }

    private static int LevenshteinDistance(string s1, string s2)
    {
        int[,] d = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
            d[i, 0] = i;
        for (int j = 0; j <= s2.Length; j++)
            d[0, j] = j;

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[s1.Length, s2.Length];
    }

    public static void Main(string[] args)
    {
        // Example usage:
        string corruptedWord = "b1nt4n6 dw1 m4rthen";

        string correctedWord = CorrectWord(corruptedWord);
        Console.WriteLine($"Original: {corruptedWord}");
        Console.WriteLine($"Corrected: {correctedWord}");
    }
}

public static class CommonEnglishWords
{
    public static List<string> Words = new List<string>
    {
        "the", "be", "to", "of", "and", "a", "in", "that", "have", "I", "it",
        "for", "not", "on", "with", "he", "as", "you", "do", "at", "this",
        "but", "his", "by", "from", "they", "we", "say", "her", "she", "or",
        "an", "will", "my", "one", "all", "would", "there", "their", "what",
        "so", "up", "out", "if", "about", "who", "get", "which", "go", "me",
        "when", "make", "can", "like", "time", "no", "just", "him", "know",
        "take", "people", "into", "year", "your", "good", "some", "could",
        "them", "see", "other", "than", "then", "now", "look", "only", "come",
        "its", "over", "think", "also", "back", "after", "use", "two", "how",
        "our", "work", "first", "well", "way", "even", "new", "want", "because",
        "any", "these", "give", "day", "most", "us"
        // This is a basic list; you may want to expand it with more words
    };
}
