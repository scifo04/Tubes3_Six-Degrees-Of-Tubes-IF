using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AlaiRegex
{
    private List<string> listRegex = new List<string>
    {
        "[Aa4@]",
        "[Bb8]",
        "[Cc\\(]",
        "[Dd]",
        "[Ee3]",
        "[Ff]",
        "[Gg6]",
        "[Hh]",
        "[Ii1!]",
        "[Jj]",
        "[Kk]",
        "[Ll1!]",
        "[Mm]",
        "[Nn]",
        "[Oo0]",
        "[Pp]",
        "[Qq]",
        "[Rr]",
        "[Ss5$]",
        "[Tt7]",
        "[Uu]",
        "[Vv]",
        "[Ww]",
        "[Xx]",
        "[Yy]",
        "[Zz2]",
        "[\\s]*"
    };

    public string StringToRegex(string input)
    {
        string result = "[a-zA-Z0-9]*?";
        int i = 0;
        foreach (char c in input)
        {
            if (c == ' ')
            {
                result += listRegex[26];
            }
            else if (c == '4' || c == '@')
            {
                result += listRegex[0]; // A
            }
            else if (c == '8')
            {
                result += listRegex[1]; // B
            }
            else if (c == '(')
            {
                result += listRegex[2]; // C
            }
            else if (c == '3')
            {
                result += listRegex[4]; // E
            }
            else if (c == '6' || c == '9')
            {
                result += listRegex[6]; // G
            }
            else if (c == '1' || c == '!')
            {
                result += listRegex[8]; // I
            }
            else if (c == '0')
            {
                result += listRegex[14]; // O
            }
            else if (c == '5' || c == '$')
            {
                result += listRegex[18]; // S
            }
            else if (c == '7')
            {
                result += listRegex[19]; // T
            }
            else if (char.IsLetter(c))
            {
                result += listRegex[char.ToUpper(c) - 65];
            }
            else
            {
                result += Regex.Escape(c.ToString());
            }

            if (i != input.Length - 1)
            {
                result += "[a-zA-Z0-9]*?";
            }
            i++;
        }
        result += "[a-zA-Z0-9]*?";
        return result;
    }

    public void TestStrings(List<string> testStrings)
    {
        foreach (string testString in testStrings)
        {
            Regex regex = new Regex(StringToRegex(testString));
            Match match = regex.Match("daniel");
            Console.WriteLine($"'{testString}': {(match.Success ? "Match" : "No Match")}");
        }
    }

    public bool TestString2(string input, string pattern)
    {
        Regex regex = new Regex(StringToRegex(pattern));
        Match match = regex.Match(input);
        return match.Success;
    }
}
