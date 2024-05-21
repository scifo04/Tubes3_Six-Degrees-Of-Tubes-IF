using System;
using System.Collections.Generic;
using System.Linq;

class BoyerMoore
{
    private readonly Dictionary<char, int> _badCharacterShift;
    private readonly int[] _goodSuffixShift;
    private readonly int[] _suffixes;
    private readonly string _pattern;

    public BoyerMoore(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            throw new ArgumentException("Pattern cannot be null or empty.");

        _pattern = pattern;
        _badCharacterShift = new Dictionary<char, int>();
        _goodSuffixShift = new int[pattern.Length + 1]; // increased by 1 to handle edge cases
        _suffixes = new int[pattern.Length + 1]; // increased by 1 to handle edge cases

        PreprocessBadCharacter();
        PreprocessGoodSuffix();
    }

    private void PreprocessBadCharacter()
    {
        for (int i = 0; i < _pattern.Length; i++)
        {
            char currentChar = _pattern[i];
            if (!_badCharacterShift.ContainsKey(currentChar))
            {
                _badCharacterShift[currentChar] = _pattern.Length - i - 1;
            }
        }
    }

    private void PreprocessGoodSuffix()
    {
        int i = _pattern.Length;
        int j = _pattern.Length + 1;
        _suffixes[i] = j;

        while (i > 0)
        {
            while (j <= _pattern.Length && _pattern[i - 1] != _pattern[j - 1])
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
        for (i = 0; i <= _pattern.Length; i++)
        {
            if (_goodSuffixShift[i] == 0)
                _goodSuffixShift[i] = j;
            if (i == j)
                j = _suffixes[j];
        }
    }

    public List<(int Position, int HammingDistance, double ClosenessPercentage)> Search(string text)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty.");

        List<(int Position, int HammingDistance, double ClosenessPercentage)> results = new List<(int Position, int HammingDistance, double ClosenessPercentage)>();
        int i = 0;
        while (i <= text.Length - _pattern.Length)
        {
            int j = _pattern.Length - 1;
            while (j >= 0 && _pattern[j] == text[i + j])
                j--;
            if (j < 0)
            {
                results.Add((i, 0, 100.0)); // Exact match
                i += _goodSuffixShift[0];
            }
            else
            {
                char badChar = text[i + j];
                int badCharShift = _badCharacterShift.ContainsKey(badChar) ? _badCharacterShift[badChar] : _pattern.Length;
                
                int hammingDistance = CalculateHammingDistance(_pattern, text.Substring(i, _pattern.Length));
                double closenessPercentage = CalculateClosenessPercentage(hammingDistance, _pattern.Length);
                results.Add((i, hammingDistance, closenessPercentage));
                
                i += Math.Max(_goodSuffixShift[j + 1], badCharShift - _pattern.Length + 1 + j);
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

class MainProgram
{
    static void Main(string[] args)
    {
        try
        {
            string text = "☼⌂ÿÿÿÿÿÿÿÿÿÿÿ@☼@☼@☼@☼@☼@↑♦☼@ ?o☼@0C♠☼@`♫↑ ☼@☺?↑`~♂?☼@A♦!??8↑☼@?↑♫☼@1?0?ÿ☼@8A1A♦d0?☼@a?@|<☼@Ç►Ä1?ÿ♠♠☼@☺?☺?g♠♥A☼@♥↑A1♀p8p@☼@♦!?f0Aü♫:0☼@Æ<Ic♣☺Ç☼@☺?D?I8q?A☼@♦►?óú♀á0☼@♀c2f!?☺F1?☼@↑ÆD?Çs?♀☼@É??⌂ÿ∟☻►☼@☻↓?'1d♥Æ1?☼@♠#&Nc?q?I☼Æl?IOo↑Ff☼@☺?U◄?¼▼?3??@#?373p@a???@F26ffAE<If☼@♀flIE???r2☼@↑I↓??↓üç??☼@3??3'?ÇY}?☼@‼36íß>Ií½U☼@♠fmU¼ÿÿv¶Ü☼@♫IÉU;ÿ{2ßì☼@∟IU·sÄ=?{l☼@↓??6÷??U}ö☼@337fî⌂YU-_☼@'w&íIóîI¿U☼@♀flYYÆöïÖU☼DIU»¿vîUU?@▲MUU¿⌂3oY|☼@▬É¶?>û»kmd☼@♦U'·⌂ÿ?me¶☼@♣÷M·om»oyo☼@     ¶ßöîY¿omì☼@♥??nßß?ímü☼@♥m»mY»{ím'☼@♠i,MUºûím'☼@[6U»¶xíe?☼@[fûwvxIfò☼@RlûæeöIFU☼@æM·æm¶EÖU☼@II·Iy¶U♫U☼@☺IY⌂ÜßvU_U☼@☺?UmùßnûöE☼@☺↓»y»?Yûù»¿mb☼@☺»·{3ïM`P☼@♥{öw3kUP☼@gnç÷{UD☼@♦îoîæû²?►?@?y_Iß'??@☺?ó¼Ix'?☼ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ";
            string pattern = "Y}?☼@‼3";
            BoyerMoore bm = new BoyerMoore(pattern);
            List<(int Position, int HammingDistance, double ClosenessPercentage)> results = bm.Search(text);

            if (results.Count == 0)
                Console.WriteLine("Pattern not found");
            else
            {
                Console.WriteLine("Pattern found at positions with closest Hamming distances:");
                foreach (var result in results)
                {
                    Console.WriteLine($"Position: {result.Position}, Hamming Distance: {result.HammingDistance}, Closeness Percentage: {result.ClosenessPercentage:F2}%");
                }
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
