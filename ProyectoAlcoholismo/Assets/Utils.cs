using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    public static class ListUtils
    {
        private static Random rand = new Random();        

        public static void Shuffle<T>(this IList<T> values)
        {
            for (int i = values.Count - 1; i > 0; i--)
            {
                int k = rand.Next(i + 1);
                (values[k], values[i]) = (values[i], values[k]);
            }
        }
    }

    public static class StringUtils
    {
        public static string generateRandomString(int length = 16)
        {
            Random rand = new Random();
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ123456789".ToCharArray();

            var str = "";
            while(str.Length < 16)
            {
                str += chars[rand.Next(0, chars.Length)];
            }
            
            return str;
        }
    }
}