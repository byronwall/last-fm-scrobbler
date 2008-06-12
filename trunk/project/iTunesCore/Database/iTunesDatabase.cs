using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using iTunesLib;

namespace iTunesCore
{
    public class iTunesDatabase
    {
        private readonly IITunesDatabaseProvider provider;
        private readonly string xmlFileLocation;
        public Dictionary<string, int> Tracks = new Dictionary<string, int>();
        public DatabaseState State = DatabaseState.Unloaded;


        public iTunesDatabase(IITunesDatabaseProvider provider)
        {
            this.provider = provider;
        }

        public iTunesDatabase(string xmlFileLocation)
        {
            if (xmlFileLocation != null)
            {
                this.xmlFileLocation = xmlFileLocation;
                provider = new ITunesDatabaseProviderFromFile(this.xmlFileLocation);
            }
            else
            {
                throw new ArgumentException("XML file location cannot be null");
            }

            LoadDatabase();
        }

        /// <summary>
        /// Loads the database.  This method uses the local database location to get the information.  It will read the information and then create its internal representation.
        /// </summary>
        public void LoadDatabase()
        {
            XDocument doc = provider.DatabaseXMLReader;
            if (doc.Element("plist")==null)
            {
                throw new NullReferenceException("The XML file is not of the correct type.");
            }
            var query = from track in doc.Element("plist").Element("dict").Element("dict").Elements("dict") select track;
            foreach (var element in query)
            {
                string filename = string.Empty;
                int playcount = 0;
                foreach (var xElement in element.Elements())
                {

                    if (xElement.Value == "Location")
                    {
                        filename = Uri.UnescapeDataString(((string)((XElement)xElement.NextNode)).Replace("file://localhost/", "").Replace("/", @"\"));
                    }
                    else if (xElement.Value == "Play Count")
                    {
                        playcount = (int)(XElement)xElement.NextNode;
                    }


                }
                Tracks.Add(filename, playcount);
            }
            this.State = DatabaseState.Loaded;
        }

        /// <summary>
        /// Analyzes the internal database to see if the track that is being checked has a different play count than the track in the database.  If there is a difference, then the track is updated in the database and the difference in play count is returned.
        /// </summary>
        /// <param name="track">The track to be checked.</param>
        /// <returns>
        /// The difference in play count between the two songs.
        /// </returns>
        public int RetrieveOrAddPlayCount(ref DatabaseTrack track)
        {
            int databasePlayCount;
            if (Tracks.ContainsKey(track.Filename))
            {
                databasePlayCount = Tracks[track.Filename];
                Tracks[track.Filename] = track.PlayCount;
            }
            else
            {
                Tracks.Add(track.Filename, track.PlayCount);
                databasePlayCount = track.PlayCount;
            }
            track.NewPlays = track.PlayCount - databasePlayCount;
            return track.NewPlays;
        }

        public int GetPlayCount(ref DatabaseTrack track)
        {
            return Tracks[track.Filename];
        }
    }

    public enum DatabaseState
    {
        Unloaded, Loaded, Invalid
    }
}