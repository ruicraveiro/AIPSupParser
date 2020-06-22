using System;
using System.Collections.Generic;
using System.Text;

namespace KmlGen
{
    public class Section
    {
        public Section(string name,  IEnumerable<Coordinate> coordinates, int altitude)
        {
            this.Name = name;
            this.Coordinates = new List<Coordinate>(coordinates);
            this.Altitude = altitude;
        }

        public string Name { get; set; }
        public int Altitude { get; set; }
        public List<Coordinate> Coordinates { get; }
    }
}
