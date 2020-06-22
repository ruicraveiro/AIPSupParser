using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KmlGen
{
    public class Altitude
    {
        public Altitude(string word)
        {
            if (!IsAltitude(word))
                throw new ArgumentException("Word is not an altitude");
            word = word.Substring(0, word.Length - 2);
            this.Feet = int.Parse(word);
        }

        public int Feet { get; }

        public static bool IsAltitude(string word)
        {
            if (!word.EndsWith("FT"))
                return false;
            word = word.Substring(0, word.Length - 2);
            var ret = int.TryParse(word, out _);
            return ret;
        }

    }
}
