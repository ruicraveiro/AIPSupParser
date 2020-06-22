using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace KmlGen
{
    public class Coordinate
    {
        public Coordinate(string longDegMins, string latDegMins)
        {
            if (!longDegMins.EndsWith('W') && !longDegMins.EndsWith('E'))
                throw new ArgumentException("LongDegMins must end with either W or E");

            if (!latDegMins.EndsWith('N') && !latDegMins.EndsWith('S'))
                throw new ArgumentException("LongDegMins must end with either W or E");

            this.LongDegMins = longDegMins;
            this.LatDegMins = latDegMins;
            this.LongDecimal = DegMinsToDecimal(longDegMins);
            this.LatDecimal = DegMinsToDecimal(LatDegMins);
        }

        public string LongDegMins { get; }
        public string LatDegMins { get; }

        public decimal LongDecimal { get; }
        public decimal LatDecimal { get; }

        private decimal DegMinsToDecimal(string degMins)
        {
            var sign = degMins.EndsWith('S') || degMins.EndsWith('W') ? -1M : 1M;
            var strDegrees = degMins.Substring(0, degMins.Length - 5);
            var strMinutes = degMins.Substring(strDegrees.Length, 2);
            var strSeconds = degMins.Substring(strDegrees.Length + 2, 2);

            var intDegrees = int.Parse(strDegrees);
            var intMinutes = int.Parse(strMinutes);
            var intSeconds = int.Parse(strSeconds);

            var decimalDegrees = (intDegrees + (intMinutes / 60M) + (intSeconds / 6000M)) * sign;
            return decimalDegrees;
        }

        private static bool IsCoordinateComponent(string word, int expectedLength, char hemisphere1, char hemisphere2)
        {
            if (!word.EndsWith(hemisphere1) && !word.EndsWith(hemisphere2))
                return false;

            if (word.Length != expectedLength)
                return false;
            word = word.Substring(0, word.Length - 1);
            var ret = int.TryParse(word, out _);
            return ret;
        }

        public static bool IsLatitude(string word) => IsCoordinateComponent(word, 7, 'N', 'S');

        public static bool IsLongitude(string word) => IsCoordinateComponent(word, 8, 'E', 'W');



    }
}
