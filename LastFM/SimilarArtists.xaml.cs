using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;

namespace LastFM
{
    public partial class SimilarArtists
    {
        private ArtistInfoManager ArtistManager;
        public string ArtistURL;
        static readonly DependencyProperty ArtistNameProperty = DependencyProperty.Register("ArtistName", typeof(string), typeof(SimilarArtists));
        public string ArtistName
        {
            get
            {
                return (string)GetValue(ArtistNameProperty);
            }
            set
            {
                SetValue(ArtistNameProperty, value);
            }
        }
        static readonly DependencyProperty ImageURLProperty = DependencyProperty.Register("ImageURL", typeof(string), typeof(SimilarArtists));
        public string ImageURL
        {
            get
            {
                return (string)GetValue(ImageURLProperty);
            }
            set
            {
                SetValue(ImageURLProperty, value);
            }
        }

        public SimilarArtists()
        {
            this.InitializeComponent();
            ArtistManager = new ArtistInfoManager(this);
            listSimilar.ItemsSource = ArtistManager.Artists;
            listArtistSongs.ItemsSource = ArtistManager.Songs;
        }

        public void InitializeArtist(string artist)
        {
            txtArtistName.Text = artist;
            if (artist == "" || artist == null)
            {
                return;
            }
            if (artist != ArtistManager.ArtistName)
            {
                ArtistManager.SetArtist(artist);
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtArtistName.Text != "" || txtArtistName.Text != "artist")
            {
                InitializeArtist(txtArtistName.Text);
            }
        }

        private void Border_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement)
            {
                FrameworkElement clicked = e.OriginalSource as FrameworkElement;
                if (clicked.Tag == null) return;
                string action = clicked.Tag.ToString();
                if (action == "info")
                {
                    ListEntry temp = (ListEntry)clicked.DataContext;
                    ArtistManager.SetArtist(temp.Artist);
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer target = (ScrollViewer)sender;
            if (e.Delta > 0)
            {
                if (target.HorizontalOffset >= 1)
                {
                    target.ScrollToHorizontalOffset(target.HorizontalOffset - 1);
                }
            }
            else
            {
                if (target.HorizontalOffset < target.ViewportWidth + target.ExtentWidth)
                {
                    target.ScrollToHorizontalOffset(target.HorizontalOffset + 1);
                }
            }
        }

    }
    public sealed class NullImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value == null)
            {
                return "http://userserve-ak.last.fm/serve/160/4147853.jpg";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ArtistInfoManager
    {
        private ObservableCollection<ListEntry> albums = new ObservableCollection<ListEntry>();
        internal ObservableCollection<ListEntry> Albums
        {
            get { return albums; }
            set { albums = value; }
        }
        private ObservableCollection<ListEntry> songs = new ObservableCollection<ListEntry>();
        internal ObservableCollection<ListEntry> Songs
        {
            get { return songs; }
            set { songs = value; }
        }
        private SimilarArtists similarArtistsReference = null;
        public SimilarArtists SimilarArtistsReference
        {
            get
            {
                if (similarArtistsReference == null)
                {
                    throw new Exception("Interface changes are happening before the piping is in place.");
                }
                return similarArtistsReference;
            }
            set { similarArtistsReference = value; }
        }

        private ObservableCollection<ListEntry> artists = new ObservableCollection<ListEntry>();
        internal ObservableCollection<ListEntry> Artists
        {
            get { return artists; }
            set { artists = value; }
        }

        public string ArtistName;
        private List<ListEntry> GetAlbums()
        {
            //Albums.Clear();

            List<ListEntry> output = new List<ListEntry>();

            string url = string.Format("http://ws.audioscrobbler.com/1.0/artist/{0}/topalbums.xml", System.Web.HttpUtility.UrlEncode(ArtistName, System.Text.Encoding.UTF8));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            request.Timeout = 15 * 1000;
            request.KeepAlive = false;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
                        {
                            readStream.ReadStartElement();
                            for (int i = 0; i < 3; i++)
                            {
                                XElement x = (XElement)XNode.ReadFrom(readStream);
                                string artistName = (string)(x.Element("name"));
                                string artistURL = (string)(x.Element("url"));
                                string imageURL = (string)(x.Element("image").Element("small"));

                                output.Add(new ListEntry() { Artist = artistName, Url = artistURL, ImageURL = imageURL });
                                //output.Add(new ListEntry(artistName, artistURL, imageURL));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }
            return output;
        }
        private List<ListEntry> GetSongs()
        {
            //Songs.Clear();
            List<ListEntry> output = new List<ListEntry>();
            string url = string.Format("http://ws.audioscrobbler.com/1.0/artist/{0}/toptracks.xml", System.Web.HttpUtility.UrlEncode(ArtistName, System.Text.Encoding.UTF8));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            request.Timeout = 15 * 1000;
            request.KeepAlive = false;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
                        {
                            readStream.ReadStartElement();
                            int maxPlayCount = 1;
                            for (int i = 0; i < 10; i++)
                            {
                                try
                                {
                                    XElement x = (XElement)XNode.ReadFrom(readStream);
                                    string songName = (string)(x.Element("name"));
                                    string songURL = (string)(x.Element("url"));
                                    int songPlayCount = (int)(x.Element("reach"));
                                    double playRatio;
                                    if (i == 0)
                                    {
                                        playRatio = 100;
                                        maxPlayCount = songPlayCount;
                                    }
                                    else
                                    {
                                        playRatio = (double)songPlayCount / maxPlayCount * 100;
                                    }

                                    output.Add(new ListEntry() { Track = songName, SimilarScore = playRatio, Url = songURL });
                                    //output.Add(new ListEntry(songName, songURL));
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }
            return output;
        }
        private List<ListEntry> GetSimilarArtists()
        {
            //Artists.Clear();

            List<ListEntry> output = new List<ListEntry>();

            string url = string.Format("http://ws.audioscrobbler.com/1.0/artist/{0}/similar.xml", System.Web.HttpUtility.UrlEncode(ArtistName, System.Text.Encoding.UTF8));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            request.Timeout = 15 * 1000;
            request.KeepAlive = false;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        settings.IgnoreProcessingInstructions = true;
                        using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
                        {
                            //insert processing logic here
                            readStream.ReadToFollowing("similarartists");
                            UpdateArtistImageURL(readStream.GetAttribute("picture"));
                            readStream.ReadStartElement();

                            for (int i = 0; i < 20; i++)
                            {
                                XElement x = (XElement)XNode.ReadFrom(readStream);
                                string artistName = (string)(x.Element("name"));
                                string artistImageURLSmall = (string)(x.Element("image_small"));
                                string artistImageURLLarge = (string)(x.Element("image"));
                                string artistURL = (string)(x.Element("url"));
                                double artistMatch = (double)(x.Element("match"));
                                output.Add(new ListEntry() { Artist = artistName, ImageURL = artistImageURLSmall, Url = artistURL, SimilarScore = artistMatch });
                                //output.Add(new ListEntry(artistName, artistURL, artistImageURLSmall, artistMatch));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }
            return output;
        }
        //public void BeginGetArtists(object state)
        //{

        //}
        public void BeginGetAlbums(object state)
        {
            UpdateAlbums(GetAlbums());
        }
        public void BeginGetSongs(object state)
        {
            UpdateSongs(GetSongs());
        }
        public void BeginGetArtists(object state)
        {
            UpdateArtists(GetSimilarArtists());
        }

        public void SetArtist(string artist)
        {
            ArtistName = artist;
            SimilarArtistsReference.ArtistName = artist;
            //SimilarArtistsReference.ImageURL = imageURL;
            SimilarArtistsReference.ArtistURL = string.Format("http://www.last.fm/music/{0}", System.Web.HttpUtility.UrlEncode(ArtistName, System.Text.Encoding.UTF8));
            //System.Threading.ThreadPool.QueueUserWorkItem(BeginGetArtists);

            //SimilarArtistsReference.lstSimilar.SelectedIndex = 0;
            if (SimilarArtistsReference.listSimilar.Items.Count > 0)
            {
                SimilarArtistsReference.listSimilar.ScrollIntoView(SimilarArtistsReference.listSimilar.Items[0]);
            }
            //ThreadPool.QueueUserWorkItem(BeginGetAlbums);
            ThreadPool.QueueUserWorkItem(BeginGetSongs);
            ThreadPool.QueueUserWorkItem(BeginGetArtists);

        }

        private void UpdateSongs(List<ListEntry> songs)
        {
            if (!SimilarArtistsReference.Dispatcher.CheckAccess())
            {
                SimilarArtistsReference.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<List<ListEntry>>(UpdateSongs), songs);
                return;
            }
            //code to be executed
            Songs.Clear();
            foreach (ListEntry item in songs)
            {
                Songs.Add(item);
            }
        }
        private void UpdateAlbums(List<ListEntry> albums)
        {
            if (!SimilarArtistsReference.Dispatcher.CheckAccess())
            {
                SimilarArtistsReference.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<List<ListEntry>>(UpdateAlbums), albums);
                return;
            }
            //code to be executed
            Albums.Clear();
            foreach (ListEntry item in albums)
            {
                Albums.Add(item);
            }
        }
        private void UpdateArtists(List<ListEntry> artists)
        {
            if (!SimilarArtistsReference.Dispatcher.CheckAccess())
            {
                SimilarArtistsReference.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<List<ListEntry>>(UpdateArtists), artists);
                return;
            }
            //code to be executed
            Artists.Clear();
            foreach (ListEntry item in artists)
            {
                Artists.Add(item);
            }
        }
        public void UpdateArtistImageURL(string artistImageURL)
        {
            if (!SimilarArtistsReference.Dispatcher.CheckAccess())
            {
                SimilarArtistsReference.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<string>(UpdateArtistImageURL), artistImageURL);
                return;
            }
            //code to be executed
            SimilarArtistsReference.ImageURL = artistImageURL;
        }

        public ArtistInfoManager(SimilarArtists parent)
        {
            SimilarArtistsReference = parent;
        }
    }
    class ListEntry
    {
        public string ImageURL { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public string Artist { get; set; }
        public string Track { get; set; }
        public double SimilarScore { get; set; }
        public ListEntry(string content, string url, string imageURL, double similarScore)
        {
            SimilarScore = similarScore;
            ImageURL = imageURL;
            Content = content;
            Url = url;
        }
        public ListEntry(string content, string url, string imageURL)
        {
            ImageURL = imageURL;
            Content = content;
            Url = url;
        }
        public ListEntry(string content, string url)
        {
            Content = content;
            Url = url;
        }
        public ListEntry() { }
    }
    public class BackgroundFromScoreConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double score = (double)value;

            //EF2706
            int redStart = 0xef;
            int greenStart = 0x27;
            int blueStart = 0x06;

            //54E518
            int redEnd = 0x54;
            int greenEnd = 0xe5;
            int blueEnd = 0x18;

            int redOut = (int)(redStart + (redEnd - redStart) * (score / 100));
            int blueOut = (int)(blueStart + (blueEnd - blueStart) * (score / 100));
            int greenOut = (int)(greenStart + (greenEnd - greenStart) * (score / 100));

            return new SolidColorBrush(Color.FromRgb((byte)redOut, (byte)greenOut, (byte)blueOut));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BackgroundWidthLeftConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double input = (double)value;
            string output = input.ToString() + "*";
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BackgroundWidthRightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double input = (double)value;
            string output = (100 - input).ToString() + "*";
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }




}