using System;
using System.Data;
using System.Globalization;

class KMP
{
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
}

class Program
{
    static void Main(string[] args)
    {
        KMP kmp = new KMP();
        string katalengkap = "☼⌂ÿÿÿÿÿÿÿÿÿÿÿ@☼@☼@☼@☼@☼@↑♦☼@ ?o☼@0C♠☼@`♫↑ ☼@☺?↑`~♂?☼@A♦!??8↑☼@?↑♫☼@1?0?ÿ☼@8A1A♦d0?☼@a?@|<☼@Ç►Ä1?ÿ♠♠☼@☺?☺?g♠♥A☼@♥↑A1♀p8p@☼@♦!?f0Aü♫:0☼@Æ<Ic♣☺Ç☼@☺?D?I8q?A☼@♦►?óú♀á0☼@♀c2f!?☺F1?☼@↑ÆD?Çs?♀☼@É??⌂ÿ∟☻►☼@☻↓?'1d♥Æ1?☼@♠#&Nc?q?I☼Æl?IOo↑Ff☼@☺?U◄?¼▼?3??@#?373p@a???@F26ffAE<If☼@♀flIE???r2☼@↑I↓??↓üç??☼@3??3'?ÇY}?☼@‼36íß>Ií½U☼@♠fmU¼ÿÿv¶Ü☼@♫IÉU;ÿ{2ßì☼@∟IU·sÄ=?{l☼@↓??6÷??U}ö☼@337fî⌂YU-_☼@'w&íIóîI¿U☼@♀flYYÆöïÖU☼DIU»¿vîUU?@▲MUU¿⌂3oY|☼@▬É¶?>û»kmd☼@♦U'·⌂ÿ?me¶☼@♣÷M·om»oyo☼@     ¶ßöîY¿omì☼@♥??nßß?ímü☼@♥m»mY»{ím'☼@♠i,MUºûím'☼@[6U»¶xíe?☼@[fûwvxIfò☼@RlûæeöIFU☼@æM·æm¶EÖU☼@II·Iy¶U♫U☼@☺IY⌂ÜßvU_U☼@☺?UmùßnûöE☼@☺↓»y»?Yûù»¿mb☼@☺»·{3ïM`P☼@♥{öw3kUP☼@gnç÷{UD☼@♦îoîæû²?►?@?y_Iß'??@☺?ó¼Ix'?☼ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ";
        bool found = false;
        int idxkata = 0;
        int count = 0;
        int idxkatalengkap = 0;
        string kata = "Y}?☼@‼3";
        int[] borderfunction = new int[kata.Length - 1];
        for (int i = 1; i < kata.Length; i++)
        {
            Console.WriteLine("Index: " + (i - 1));
            borderfunction[i - 1] = kmp.getSize(kata, i - 1);
        }
        Console.WriteLine("Border Function: ");
        for (int i = 0; i < borderfunction.Length; i++)
        {
            Console.WriteLine(borderfunction[i]);
        }
        while (found == false && idxkatalengkap < katalengkap.Length)
        {
            count += 1;
            if (kata[idxkata] == katalengkap[idxkatalengkap])
            {
                if (idxkata == kata.Length - 1)
                {
                    /*Console.WriteLine("Huruf kata lengkap: " + katalengkap[idxkatalengkap]);
                    Console.WriteLine("Huruf kata: " + kata[idxkata]);
                    Console.WriteLine("Kata ditemukan di index: " + (idxkatalengkap));*/
                    found = true;
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
                    idxkata = borderfunction[idxkata - 1];
                }
            }
        }
        if (found == false)
        {
            Console.WriteLine("Kata tidak ditemukan");
        }
        else
        {
            Console.WriteLine("Ditemukan setelah terjadi: " + count + " pencocokan");
        }
    }
}
