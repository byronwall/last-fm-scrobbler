using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Threading;

namespace LastFM
{
    public partial class UserInfoControl
    {
        public string UserName;
        private ObservableCollection<ListEntry> userWeeklyArtists = new ObservableCollection<ListEntry>();
        private ObservableCollection<ListEntry> userWeeklyTracks = new ObservableCollection<ListEntry>();
        private ObservableCollection<ListEntry> userRecommendations = new ObservableCollection<ListEntry>();

        static readonly DependencyProperty UserUserNameProperty = DependencyProperty.Register("UserUserName", typeof(string), typeof(UserInfoControl));
        static readonly DependencyProperty UserPlayCountProperty = DependencyProperty.Register("UserPlayCount", typeof(int), typeof(UserInfoControl));
        static readonly DependencyProperty UserRealNameProperty = DependencyProperty.Register("UserRealName", typeof(string), typeof(UserInfoControl));
        static readonly DependencyProperty UserImageURLProperty = DependencyProperty.Register("UserImageURL", typeof(string), typeof(UserInfoControl));

        public string UserUserName
        {
            get
            {
                return (string)GetValue(UserUserNameProperty);
            }
            set
            {
                SetValue(UserUserNameProperty, value);
            }
        }
        public string UserRealName
        {
            get
            {
                return (string)GetValue(UserRealNameProperty);
            }
            set
            {
                SetValue(UserRealNameProperty, value);
            }
        }
        public int UserPlayCount
        {
            get
            {
                return (int)GetValue(UserPlayCountProperty);
            }
            set
            {
                SetValue(UserPlayCountProperty, value);
            }
        }
        public string UserImageURL
        {
            get
            {
                return (string)GetValue(UserImageURLProperty);
            }
            set
            {
                SetValue(UserImageURLProperty, value);
            }
        }

        public UserInfoControl()
        {
            this.InitializeComponent();

            listRecommendations.ItemsSource = userRecommendations;
            listWeeklyArtist.ItemsSource = userWeeklyArtists;
            listWeeklyTracks.ItemsSource = userWeeklyTracks;

            // Insert code required on object creation below this point.
        }
        private UserInfoObject GetUserDetails()
        {
            UserInfoObject output = new UserInfoObject();
            string url = string.Format("http://ws.audioscrobbler.com/1.0/user/{0}/profile.xml", System.Web.HttpUtility.UrlEncode(UserName, System.Text.Encoding.UTF8));

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
                            //readStream.ReadStartElement();
                            readStream.ReadToDescendant("profile");
                            XElement x = (XElement)XNode.ReadFrom(readStream);

                            string userURL = (string)(x.Element("url"));
                            string userRealName = (string)(x.Element("realname"));
                            int userPlayCount = (int)(x.Element("playcount"));
                            string userImageURL = (string)(x.Element("avatar"));

                            output.ImageURL = userImageURL;
                            output.PlayCount = userPlayCount;
                            output.RealName = userRealName;
                            output.UserName = UserName;

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
        private List<ListEntry> GetWeeklyArtists()
        {
            List<ListEntry> output = new List<ListEntry>();
            string url = string.Format("http://ws.audioscrobbler.com/1.0/user/{0}/weeklyartistchart.xml", System.Web.HttpUtility.UrlEncode(UserName, System.Text.Encoding.UTF8));

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


                            /*
                             * <weeklyartistchart user="RJ" from="1203249600" to="1203854400">
−
    <artist>
<name>Stanley Jordan</name>
<mbid>6ba7dd48-a5fe-46a3-947a-057919dbe989</mbid>
<chartposition>1</chartposition>
<playcount>18</playcount>
<url>http://www.last.fm/music/Stanley+Jordan</url>
</artist>
                             * 
                             */
                            for (int i = 0; i < 10; i++)
                            {
                                XElement x = (XElement)XNode.ReadFrom(readStream);
                                string artistName = (string)(x.Element("name"));
                                int artistPlayCount = (int)(x.Element("playcount"));
                                string artistURL = (string)(x.Element("url"));

                                output.Add(new ListEntry() { Artist = artistName, Url = artistURL });
                                //output.Add(new ListEntry(artistName, artistURL));
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
        private List<ListEntry> GetWeeklyTracks()
        {
            List<ListEntry> output = new List<ListEntry>();
            string url = string.Format("http://ws.audioscrobbler.com/1.0/user/{0}/weeklytrackchart.xml", System.Web.HttpUtility.UrlEncode(UserName, System.Text.Encoding.UTF8));

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


                            /*
                             * 	<weeklytrackchart user="gexman87" from="1203249600" to="1203854400">
−
	<track>
<artist mbid="">Ronnie Day</artist>
<name>Ever and After</name>
<mbid/>
<chartposition>1</chartposition>
<playcount>6</playcount>
−
	<url>
http://www.last.fm/music/Ronnie+Day/_/Ever+and+After
</url>
</track>
                             * 
                             */
                            for (int i = 0; i < 10; i++)
                            {
                                XElement x = (XElement)XNode.ReadFrom(readStream);
                                string trackArtist = (string)(x.Element("artist"));
                                string trackName = (string)(x.Element("name"));
                                int trackPlayCount = (int)(x.Element("playcount"));
                                string trackURL = (string)(x.Element("url"));
                                output.Add(new ListEntry() { Artist = trackArtist, Track = trackName, Url = trackURL });
                                //output.Add(new ListEntry(string.Format("{0} - {1}", trackArtist, trackName), trackURL));
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
        private List<ListEntry> GetRecommendations()
        {
            List<ListEntry> output = new List<ListEntry>();
            string url = string.Format("http://ws.audioscrobbler.com/1.0/user/{0}/systemrecs.xml", System.Web.HttpUtility.UrlEncode(UserName, System.Text.Encoding.UTF8));

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


                            /*
                             * 		<recommendations user="gexman87">
−
	<artist>
<name>Ben Harper</name>
<mbid>1582a5b8-538e-45e7-9ae4-4099439a0e79</mbid>
<url>http://www.last.fm/music/Ben+Harper</url>
</artist>
                             * 
                             */
                            for (int i = 0; i < 10; i++)
                            {
                                XElement x = (XElement)XNode.ReadFrom(readStream);
                                string recommendationName = (string)(x.Element("name"));
                                string recommendationURL = (string)(x.Element("url"));
                                output.Add(new ListEntry() { Artist = recommendationName, Url = recommendationURL });
                                //output.Add(new ListEntry(recommendationName, recommendationURL));
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

        public void UpdateCurrentUser(string username)
        {
            UserName = username;
            ThreadPool.QueueUserWorkItem(BeginUpdateRecommendations);
            ThreadPool.QueueUserWorkItem(BeginUpdateWeeklyArtists);
            ThreadPool.QueueUserWorkItem(BeginUpdateWeeklyTracks);
            ThreadPool.QueueUserWorkItem(BeginUpdateUserDetails);
        }

        private void BeginUpdateWeeklyArtists(object state)
        {
            UpdateWeeklyArtists(GetWeeklyArtists());
        }
        private void BeginUpdateWeeklyTracks(object state)
        {
            //insert code here
            UpdateWeeklyTracks(GetWeeklyTracks());
        }
        private void BeginUpdateRecommendations(object state)
        {
            //insert code here
            UpdateRecommendations(GetRecommendations());
        }
        private void BeginUpdateUserDetails(object state)
        {
            //insert code here
            UpdateUserDetails(GetUserDetails());
        }

        private void UpdateUserDetails(UserInfoObject userInfo)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<UserInfoObject>(UpdateUserDetails), userInfo);
                return;
            }
            //code to be executed
            UserUserName = userInfo.UserName;
            UserRealName = userInfo.RealName;
            UserPlayCount = userInfo.PlayCount;
            UserImageURL = userInfo.ImageURL;
        }
        private void UpdateWeeklyArtists(List<ListEntry> artists)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<List<ListEntry>>(UpdateWeeklyArtists), artists);
                return;
            }
            //code to be executed
            userWeeklyArtists.Clear();
            foreach (ListEntry item in artists)
            {
                userWeeklyArtists.Add(item);
            }
        }
        private void UpdateWeeklyTracks(List<ListEntry> tracks)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<List<ListEntry>>(UpdateWeeklyTracks), tracks);
                return;
            }
            //code to be executed
            userWeeklyTracks.Clear();
            foreach (ListEntry item in tracks)
            {
                userWeeklyTracks.Add(item);
            }
        }
        private void UpdateRecommendations(List<ListEntry> recommendations)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<List<ListEntry>>(UpdateRecommendations), recommendations);
                return;
            }
            //code to be executed
            userRecommendations.Clear();
            foreach (ListEntry item in recommendations)
            {
                userRecommendations.Add(item);
            }
        }


        private void PreviewMouseWheelVerticalScroll(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer target = (ScrollViewer)sender;
            if (e.Delta > 0)
            {
                if (target.VerticalOffset >= 1)
                {
                    target.ScrollToVerticalOffset(target.VerticalOffset - 1);
                }
            }
            else
            {
                if (target.VerticalOffset < target.ViewportHeight + target.ExtentHeight)
                {
                    target.ScrollToVerticalOffset(target.VerticalOffset + 1);
                }
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
                    InterfaceHelper.ShowArtistInfo(temp.Artist);
                }
            }
        }
    }
    struct UserInfoObject
    {
        public int PlayCount;
        public string UserName;
        public string RealName;
        public string ImageURL;
    }

}