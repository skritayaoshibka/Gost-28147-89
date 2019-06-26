using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gost_28147_89
{
    class Gost
    {
        private List<string> Blocks= new List<string>();
        private List<string> Keys = new List<string>();
        private string IV = "0101010101010101010101011111100011111100101010101010101111111000";

        public Gost(string text, string key)
        {
            TextTo64BitBlock(text);
            SymbolKeyTo256BitKey(key);
        }

        private void TextTo64BitBlock(string enter_text)
        {
            string text = "";
            for (int i = 0; i < enter_text.Length; i++)
            {
                string ab = Convert.ToString(enter_text[i], 2);
                while (ab.Length < 16)
                    ab = '0' + ab;
                Console.WriteLine(ab.Length + " " + enter_text[i]);
                text += ab;
            }

            while (text.Length % 64 != 0)
                text += "0";

            for (int i = 0; i < text.Length; i = i + 64)
                Blocks.Add(text.Substring(i, 64));
        }

        private void SymbolKeyTo256BitKey(string enter_key)
        {
            string key = "";
            for (int i = 0; i < enter_key.Length; i++)
            {
                string ab = Convert.ToString(enter_key[i], 2);
                while (ab.Length < 16)
                    ab = '0' + ab;
                key += ab;

            }
            while (key.Length != 256)
            {
                if (key.Length > 256)
                    key = key.Remove(key.Length - 1, 1);
                else if (key.Length < 256)
                    key += "0";
            }

            KeysGeneration(key);
        }

        private void KeysGeneration(string key)
        {
            for (int i = 0; i < key.Length; i += 32)
                Keys.Add(key.Substring(i, 32));

            for (int i = 0; i < 8; i++)
            {
                Keys.Add(Keys[i]);
                if (Keys.Count == 24)
                    break;

                if (i == 7)
                    i = 0;
            }

            for (int i = 7; i >= 0; i--)
                Keys.Add(Keys[i]);
        }


        private int f_function(string text, string key)
        {

            int[,] s_table = new int[,]
            {
                { 4, 10, 9, 2, 13, 8, 0, 14, 6, 11, 1, 12, 7, 15, 5, 3 },
                { 14, 11, 4, 12, 6, 13, 15, 10, 2, 3, 8, 1, 0, 7, 5, 9 },
                { 5, 8, 1, 13, 10, 3, 4, 2, 14, 15, 12, 7, 6, 0, 9, 11 },
                { 7, 13, 10, 1, 0, 8, 9, 15, 14, 4, 6, 12, 11, 2, 5, 3 },
                { 6, 12, 7, 1, 5, 15, 13, 8, 4, 10, 9, 14, 0, 3, 11, 2 },
                { 4, 11, 10, 0, 7, 2, 1, 13, 3, 6, 8, 5, 9, 12, 15, 14 },
                { 13, 11, 4, 1, 3, 15, 5, 9, 0, 10, 14, 7, 6, 8, 2, 12 },
                { 1, 15, 13, 0, 5, 7, 10, 4, 9, 2, 3, 14, 6, 11, 8, 12 }
            };

            int a = Convert.ToInt32(text.Substring(0, 32), 2);
            int k = Convert.ToInt32(key.Substring(0, 32), 2);

            int sum = a + k;
            text = Convert.ToString(sum, 2);
            while (text.Length < 32)
                text = '0' + text;

            string result = "";
            for (int i = 0; i < text.Length; i += 4)
            {
                int index = Convert.ToInt32(text.Substring(i, 4), 2);
                string heh = Convert.ToString(s_table[i / 4, index], 2);
                
                while (heh.Length < 4)
                    heh = '0' + heh;

                result += heh;
            }

            int output = Convert.ToInt32(result, 2);
            output = output << 11;

            return output;
        }

        public string Encrypt()
        {
            string cipher = "";

            for (int i = 0; i < Blocks.Count; i++)
            {

                string A = IV.Substring(32, 32);
                string B = IV.Substring(0, 32);

                for (int j = 0; j < 32; j++)
                {
                    int b = Convert.ToInt32(B, 2);
                    int a = b ^ f_function(A, Keys[i]);

                    B = A;
                    A = Convert.ToString(a, 2);
                    while (A.Length < 32)
                        A = '0' + A;
                }

                string result = A + B;
                long IV_int = Convert.ToInt64(result, 2);
                long res_int = IV_int ^ Convert.ToInt64(Blocks[i], 2);
                IV = Convert.ToString(res_int, 2);
                while (IV.Length < 64)
                    IV = '0' + IV;

                int start_index = 0;
                while (start_index < IV.Length)
                {
                    char c = (char)Convert.ToInt16(IV.Substring(start_index, 16), 2);
                    cipher += c;
                    start_index += 16;
                }
            }
            return cipher;
        }



        public string Decrypt()
        {
            string cipher = "";
            for (int i = 0; i < Blocks.Count; i++)
            {

                string A = IV.Substring(32, 32);
                string B = IV.Substring(0, 32);

                for (int j = 0; j < 32; j++)
                {
                    int b = Convert.ToInt32(B, 2);
                    
                    b = b ^ f_function(A, Keys[i]);
                    B = A;
                    A = Convert.ToString(b, 2);
                    while (A.Length < 32)
                        A = '0' + A;
                }

                string result = A + B;

                long IV_int = Convert.ToInt64(result, 2);
                IV_int = IV_int ^ Convert.ToInt64(Blocks[i], 2);
                IV = Convert.ToString(IV_int, 2);
                while (IV.Length < 64)
                    IV = '0' + IV;

                int start_index = 0;
                while (start_index < IV.Length)
                {
                    char c = (char)Convert.ToInt16(IV.Substring(start_index, 16), 2);
                    cipher += c;
                    start_index += 16;
                }

                IV = Blocks[i];
            }

            return cipher;
        }
    }
}
