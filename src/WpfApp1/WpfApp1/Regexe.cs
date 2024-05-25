using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace test {
    public class Regexe {
        private static Dictionary<char,string> dictionary= new Dictionary<char, string>();
        
        static Regexe() {
            dictionary.Add('A',"[Aa4]?");
            dictionary.Add('B',"[Bb]");
            dictionary.Add('C',"[Cc]");
            dictionary.Add('D',"[Dd]");
            dictionary.Add('E',"[Ee3]?");
            dictionary.Add('F',"[Ff]");
            dictionary.Add('G',"[Gg6]");
            dictionary.Add('H',"[Hh]");
            dictionary.Add('I',"[Ii1]?");
            dictionary.Add('J',"[Jj]");
            dictionary.Add('K',"[Kk]");
            dictionary.Add('L',"[Ll1]");
            dictionary.Add('M',"[Mm]");
            dictionary.Add('N',"[Nn]");
            dictionary.Add('O',"[Oo0]?");
            dictionary.Add('P',"[Pp]");
            dictionary.Add('Q',"[Qq]");
            dictionary.Add('R',"[Rr]");
            dictionary.Add('S',"[Ss5]");
            dictionary.Add('T',"[Tt7]");
            dictionary.Add('U',"[Uu]?");
            dictionary.Add('V',"[Vv]");
            dictionary.Add('W',"[Ww]");
            dictionary.Add('X',"[Xx]");
            dictionary.Add('Y',"[Yy]");
            dictionary.Add('Z',"[Zz]");
            dictionary.Add('a',"[Aa4]?");
            dictionary.Add('b',"[Bb]");
            dictionary.Add('c',"[Cc]");
            dictionary.Add('d',"[Dd]");
            dictionary.Add('e',"[Ee3]?");
            dictionary.Add('f',"[Ff]");
            dictionary.Add('g',"[Gg6]");
            dictionary.Add('h',"[Hh]");
            dictionary.Add('i',"[Ii1]?");
            dictionary.Add('j',"[Jj]");
            dictionary.Add('k',"[Kk]");
            dictionary.Add('l',"[Ll1]");
            dictionary.Add('m',"[Mm]");
            dictionary.Add('n',"[Nn]");
            dictionary.Add('o',"[Oo0]?");
            dictionary.Add('p',"[Pp]");
            dictionary.Add('q',"[Qq]");
            dictionary.Add('r',"[Rr]");
            dictionary.Add('s',"[Ss5]");
            dictionary.Add('t',"[Tt7]");
            dictionary.Add('u',"[Uu]?");
            dictionary.Add('v',"[Vv]");
            dictionary.Add('w',"[Ww]");
            dictionary.Add('x',"[Xx]");
            dictionary.Add('y',"[Yy]");
            dictionary.Add('z',"[Zz]");
        }
        public static bool checkAlay(string word, string bener) {
            string[] splitted = word.Split(' ');
            string[] benered = bener.Split(' ');
            bool res = true;
            for (int i = 0; i < splitted.Length; i++) {
                res = res && regexAlay(splitted[i],benered[i]);
            }
            return res;
        }

        public static bool regexAlay(string word, string bener) {
            Regex re = new Regex(turnToRegex(bener));
            if (re.IsMatch(word)) {
                Console.WriteLine(re);
                Console.WriteLine(re.IsMatch(word));
                return true;
            }
            Console.WriteLine(re);
            Console.WriteLine(re.IsMatch(word));
            return false;
        }

        public static string turnToRegex(string bener) {
            string original = @"^";
            for (int i = 0; i < bener.Length; i++) {
                original += dictionary[bener[i]];
            }
            original += "$";
            return original;
        }
        
    }
}