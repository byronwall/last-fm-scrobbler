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
    public partial class ArtistInfoControl
    {
        public ArtistInfoControl()
        {
            this.InitializeComponent();
            ArtistInfoManager.Instance.SimilarArtistsReference = this;
            listArtistAlbums.ItemsSource = ArtistInfoManager.Instance.Albums;
            listArtistSongs.ItemsSource = ArtistInfoManager.Instance.Songs;
        }

        public string ArtistURL;
        static readonly DependencyProperty ArtistNameProperty = DependencyProperty.Register("ArtistName", typeof(string), typeof(ArtistInfoControl));
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
        static readonly DependencyProperty ImageURLProperty = DependencyProperty.Register("ImageURL", typeof(string), typeof(ArtistInfoControl));
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

        private void txtArtistName_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ArtistURL != null)
            {
                System.Diagnostics.Process.Start(ArtistURL);
            }
        }

    }
    //public class ArtistInfoManager
    //{
    //    private ObservableCollection<ListEntry> albums = new ObservableCollection<ListEntry>();
    //    internal ObservableCollection<ListEntry> Albums
    //    {
    //        get { return albums; }
    //        set { albums = value; }
    //    }
    //    private ObservableCollection<ListEntry> songs = new ObservableCollection<ListEntry>();
    //    internal ObservableCollection<ListEntry> Songs
    //    {
    //        get { return songs; }
    //        set { songs = value; }
    //    }
    //    private static ArtistInfoManager _instance = null;
    //    internal static ArtistInfoManager Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //            {
    //                _instance = new ArtistInfoManager();
    //            }
    //            return _instance;
    //        }
    //    }
    //    private ArtistInfoControl artistInfoReference = null;
    //    public ArtistInfoControl ArtistInfoReference
    //    {
    //        get
    //        {
    //            if (artistInfoReference == null)
    //            {
    //                throw new Exception("Interface changes are happening before the piping is in place.");
    //            }
    //            return artistInfoReference;
    //        }
    //        set { artistInfoReference = value; }
    //    }
    //    private string ArtistName;
    //    private void GetAlbums()
    //    {
    //        Albums.Clear();

    //        string url = string.Format("http://ws.audioscrobbler.com/1.0/artist/{0}/topalbums.xml", System.Web.HttpUtility.UrlEncode(ArtistName, System.Text.Encoding.UTF8));

    //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    //        request.Proxy = null;
    //        request.Timeout = 15 * 1000;
    //        request.KeepAlive = false;
    //        try
    //        {
    //            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
    //            {
    //                using (Stream receiveStream = response.GetResponseStream())
    //                {
    //                    XmlReaderSettings settings = new XmlReaderSettings();
    //                    settings.IgnoreWhitespace = true;
    //                    using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
    //                    {
    //                        readStream.ReadStartElement();
    //                        for (int i = 0; i < 3; i++)
    //                        {
    //                            XElement x = (XElement)XNode.ReadFrom(readStream);
    //                            string artistName = (string)(x.Element("name"));
    //                            string artistURL = (string)(x.Element("url"));
    //                            string imageURL = (string)(x.Element("image").Element("small"));

    //                            Albums.Add(new ListEntry(artistName, artistURL, imageURL));
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Log.Instance.AddEvent(e);
    //        }
    //    }
    //    private void GetSongs()
    //    {
    //        Songs.Clear();

    //        string url = string.Format("http://ws.audioscrobbler.com/1.0/artist/{0}/toptracks.xml", System.Web.HttpUtility.UrlEncode(ArtistName, System.Text.Encoding.UTF8));

    //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    //        request.Proxy = null;
    //        request.Timeout = 15 * 1000;
    //        request.KeepAlive = false;
    //        try
    //        {
    //            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
    //            {
    //                using (Stream receiveStream = response.GetResponseStream())
    //                {
    //                    XmlReaderSettings settings = new XmlReaderSettings();
    //                    settings.IgnoreWhitespace = true;
    //                    using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
    //                    {
    //                        readStream.ReadStartElement();
    //                        for (int i = 0; i < 10; i++)
    //                        {
    //                            try
    //                            {
    //                                XElement x = (XElement)XNode.ReadFrom(readStream);
    //                                string songName = (string)(x.Element("name"));
    //                                string songURL = (string)(x.Element("url"));

    //                                Songs.Add(new ListEntry(songName, songURL));
    //                            }
    //                            catch { }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Log.Instance.AddEvent(e);
    //        }
    //    }
    //    public void SetArtist(string artist, string imageURL)
    //    {
    //        ArtistName = artist;
    //        ArtistInfoReference.ArtistName = artist;
    //        ArtistInfoReference.ImageURL = imageURL;
    //        ArtistInfoReference.ArtistURL = string.Format("http://www.last.fm/music/{0}", System.Web.HttpUtility.UrlEncode(ArtistName, System.Text.Encoding.UTF8));
    //        GetAlbums();
    //        GetSongs();
    //    }
    //}
    //class ListEntry
    //{
    //    private string content, url, imageURL;

    //    public string ImageURL
    //    {
    //        get { return imageURL; }
    //        set { imageURL = value; }
    //    }

    //    public string Url
    //    {
    //        get { return url; }
    //        set { url = value; }
    //    }

    //    public string Content
    //    {
    //        get { return content; }
    //        set { content = value; }
    //    }
    //    public ListEntry(string content, string url, string imageURL)
    //    {
    //        Content = content;
    //        Url = url;
    //        ImageURL = imageURL;
    //    }
    //    public ListEntry(string content, string url)
    //    {
    //        Content = content;
    //        Url = url;
    //    }

    //}

}