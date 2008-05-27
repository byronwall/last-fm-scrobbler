using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace LastFM
{
    public partial class WindowExtraInfo
    {
        public WindowExtraInfo(string artist)
        {
            this.InitializeComponent();
            lstSimilar.ItemsSource = ArtistsCollection;
            ArtistsCollection.Add(new SimilarArtists(artist));

        }
        public ObservableCollection<SimilarArtists> ArtistsCollection = new ObservableCollection<SimilarArtists>();

        private void TextBlock_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock _sender = sender as TextBlock;
            ArtistsCollection.Add(new SimilarArtists(_sender.Text));
        }

    }
    public class SimilarArtists
    {
        private string artist;

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }
        private List<ArtistEntry> similars = new List<ArtistEntry>();

        public List<ArtistEntry> Similars
        {
            get { return similars; }
            set { similars = value; }
        }

        public SimilarArtists(string artist)
        {
            Artist = artist;
            BuildSimilarList();
        }
        private void BuildSimilarList()
        {
            string url = string.Format("http://ws.audioscrobbler.com/1.0/artist/{0}/similar.xml", System.Web.HttpUtility.UrlEncode(Artist, System.Text.Encoding.GetEncoding("ISO-8859-1")));



            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.Proxy = null;
            try
            {
                using (HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse())
                {
                    using (Stream receiveStream = myHttpWebResponse.GetResponseStream())
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
                        {
                            readStream.ReadStartElement();
                            for (int i = 0; i < 5; i++)
                            {
                                XElement x = (XElement)XNode.ReadFrom(readStream);
                                string newartist = (string)(x.Element("name"));
                                similars.Add(new ArtistEntry(newartist));
                            }
                        }
                    }
                }
            }
            catch
            {

            }

        }

    }
    public class ArtistEntry
    {
        private string artist;

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }
        public ArtistEntry(string artist)
        {
            Artist = artist;
        }
    }
}