using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Service.Helper
{
    public class RandomStringGenerator
    {
        private static Random random = new Random();
        public static string RandomString()
        {
            int length = 2;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomCharacters()
        {
            int length = 2;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}