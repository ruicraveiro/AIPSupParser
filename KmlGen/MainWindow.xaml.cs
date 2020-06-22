using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace KmlGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SectionTemplate = @"  
<Placemark>
    <name>{0}</name>
    <styleUrl>#default0</styleUrl>
    <Polygon>
      <extrude>1</extrude>
      <altitudeMode>relativeToGround</altitudeMode>
      <outerBoundaryIs>
        <LinearRing>
          <coordinates>
            {1}
          </coordinates>
        </LinearRing>
      </outerBoundaryIs>
    </Polygon>
  </Placemark>
";

        private readonly static string template =
"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine +
"<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:kml=\"http://www.opengis.net/kml/2.2\" xmlns:atom=\"http://www.w3.org/2005/Atom\">" + @"
<Document>
	<name>Areas.kml</name>
" + "	<Style id=\"default\">" + @"
		<LineStyle>
			<color>ff0000ff</color>
		</LineStyle>
		<PolyStyle>
			<color>330000ff</color>
		</PolyStyle>
	</Style>
" + "	<StyleMap id=\"default0\">" + @"
		<Pair>
			<key>normal</key>
			<styleUrl>#default</styleUrl>
		</Pair>
		<Pair>
			<key>highlight</key>
			<styleUrl>#hl</styleUrl>
		</Pair>
	</StyleMap>
" + "	<Style id=\"hl\">" + @"
		<IconStyle>
			<scale>1.2</scale>
		</IconStyle>
		<LineStyle>
			<color>ff0000ff</color>
		</LineStyle>
		<PolyStyle>
			<color>330000ff</color>
		</PolyStyle>
	</Style>

    {0}

</Document>
</kml>";



        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sectionsText = txtSections.Text;
            string st = string.Empty;
            try
            {
                var sectionTexts = GetSectionTexts(sectionsText);

                var builder = new StringBuilder();
                foreach (var sectionText in sectionTexts)
                {
                    st = sectionText;
                    var section = GetSection(sectionText);
                    var sectionKml = GetKmlSection(section);
                    builder.AppendLine(sectionKml);
                }

                var kmlDocument = string.Format(template, builder.ToString());

                txtResult.Text = kmlDocument;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + " for section " + st);
            }
        }



        private IEnumerable<string> GetSectionTexts(string sectionsText)
        {
            var sectionTexts = new List<string>();
            var pendingBlock = new StringBuilder();
            var reader = new StringReader(sectionsText);

            string line;
            while( (line = reader.ReadLine()) != null )
            {
                if (line.StartsWith("AREA ") && pendingBlock.Length != 0)
                {
                    sectionTexts.Add(pendingBlock.ToString());
                    pendingBlock.Clear();
                }
                pendingBlock.AppendLine(line);
            }
            if (pendingBlock.Length != 0)
                sectionTexts.Add(pendingBlock.ToString());
            return sectionTexts;

        }

        private Section GetSection(string sectionText)
        {
            var stringReader = new StringReader(sectionText);

            string pendingLatitudeWord = null;
            string lastAltitudeWord = null;

            List<Coordinate> coordinates = new List<Coordinate>();
            string name = null;
            string line;
            while ((line = stringReader.ReadLine()) != null && line != Environment.NewLine && !string.IsNullOrWhiteSpace(line))
            {
                var words = line.Split(new char[] { ' ' , ',' }).Select(w => w.Trim().ToUpper()).ToArray();
                foreach (var word in words)
                {
                    if (word == Environment.NewLine || string.IsNullOrWhiteSpace(word))
                        continue;
                    var isLatitutde = Coordinate.IsLatitude(word);
                    bool isLongitude;
                    if (isLatitutde)
                    {
                        if (pendingLatitudeWord != null)
                            throw new ArgumentException("Found a latitude when another latitude is still pending");
                        pendingLatitudeWord = word;
                    }
                    else if (isLongitude = Coordinate.IsLongitude(word))
                    {
                        if (pendingLatitudeWord == null)
                            throw new ArgumentException("Found a longitude without a corresponding pending latitude");
                        var coordinate = new Coordinate(word, pendingLatitudeWord);
                        pendingLatitudeWord = null;
                        coordinates.Add(coordinate);
                    }
                    else if (pendingLatitudeWord != null)
                        throw new ArgumentException("Found another non-latitude word after a pending latitude");
                    else if (word == "AREA")
                    {
                        name = line;
                        break;
                    }
                    if (Altitude.IsAltitude(word))

                        lastAltitudeWord = word;
                }
            }

            var altitude = lastAltitudeWord != null ? new Altitude(lastAltitudeWord) : null;

            var ret = new Section(name ?? "?", coordinates, altitude?.Feet ?? 0);
            return ret;
        }


        private string GetKmlSection(Section section)
        {
            var coordinateLines = section.Coordinates.Select(c => $"{ c.LongDecimal.ToString("0.0000000000000000", CultureInfo.InvariantCulture) },{ c.LatDecimal.ToString("0.0000000000000000", CultureInfo.InvariantCulture) },{ section.Altitude }");
            var coordinatesText = string.Join(Environment.NewLine, coordinateLines);

            var kml = string.Format(SectionTemplate, section.Name, coordinatesText);
            return kml;
        }
    }
}
