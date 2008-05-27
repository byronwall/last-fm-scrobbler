using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using LastFM.TrackInfo;
using System.Runtime.Serialization.Formatters.Binary;

namespace LastFM
{
    class LastFMHelper
    {
        static Encoding subEncoding = Encoding.GetEncoding("ISO-8859-1");
        static string submitURI, playingURI, sessionID, username, passwordHash;

        public static string PasswordHash
        {
            get { return LastFMHelper.passwordHash; }
            set { LastFMHelper.passwordHash = value; }
        }
        public static string Username
        {
            get { return LastFMHelper.username; }
            set { LastFMHelper.username = value; }
        }
        public static string SubmitURI
        {
            get { return LastFMHelper.submitURI; }
            set { LastFMHelper.submitURI = value; }
        }
        public static string PlayingURI
        {
            get { return LastFMHelper.playingURI; }
            set { LastFMHelper.playingURI = value; }
        }
        public static string SessionID
        {
            get { return LastFMHelper.sessionID; }
            set { LastFMHelper.sessionID = value; }
        }

        static int lastSubmission = 0;
        public static int LastSubmission
        {
            get { return LastFMHelper.lastSubmission; }
            set { LastFMHelper.lastSubmission = value; }
        }

        static HandshakeResponse connectionState = HandshakeResponse.FAILED;
        private static HandshakeResponse ConnectionState
        {
            get { return LastFMHelper.connectionState; }
            set { LastFMHelper.connectionState = value; }
        }

        public static void SaveState(string filename)
        {
            //if (CachedSubmissions.Count == 0) return;

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (Submission item in CachedSubmissions)
                    {
                        sw.WriteLine(item.Serialize());
                    }
                }
            }
        }
        const int MaxSub = 30;
        internal static void ProcessCache()
        {
            if (CachedSubmissions.Count == 0)
            {
                Log.Instance.AddEvent("Cache is empty and the process is done.");
                return;
            }
            Log.Instance.AddEvent(LogEvent.LogEventSender.LastFM, "The cache is beginning to be processed.", LogEvent.LogEventStatus.Neutral);

            int counter = 0;
            int availableTime = UnixFromNow() - LastSubmission;
            int startTime = LastSubmission;
            string _URI = "";

            foreach (Submission item in CachedSubmissions)
            {
                Track track = item.Content;
                if (track.Length >= 30)
                {
                    for (int i = 0; i < track.NewPlays; i++)
                    {
                        availableTime -= track.Length;
                        startTime += track.Length;
                        if (availableTime < 0 || counter > MaxSub - 1) break;

                        if (counter == 0)
                        {
                            _URI = string.Format("{0}?s={1}&a[0]={2}&t[0]={3}&i[0]={4}&o[0]={5}&r[0]={6}&l[0]={7}&b[0]={8}&n[0]={9}&m[0]={10}", SubmitURI, SessionID, HttpUtility.UrlEncode(track.Artist, subEncoding), HttpUtility.UrlEncode(track.Title, subEncoding), startTime, "P", "", track.Length, HttpUtility.UrlEncode(track.Album, subEncoding), track.TrackNo, "");
                        }
                        else
                        {
                            //Encoding.GetEncoding("ISO-8859-1")
                            _URI = string.Format("{0}&a[{11}]={2}&t[{11}]={3}&i[{11}]={4}&o[{11}]={5}&r[{11}]={6}&l[{11}]={7}&b[{11}]={8}&n[{11}]={9}&m[{11}]={10}", _URI, SessionID, HttpUtility.UrlEncode(track.Artist, subEncoding), HttpUtility.UrlEncode(track.Title, subEncoding), startTime, "P", "", track.Length, HttpUtility.UrlEncode(track.Album, subEncoding), track.TrackNo, "", counter);
                        }
                        counter++;
                        item.HandledPlays++;

                    }
                }
                else
                {
                    track.NewPlays = 0;
                }
                if (availableTime < 0 || counter > MaxSub - 1) break;
            }
            //SaveState(filenames);
            if (counter == 0) return;
            int attempts = 1;
            while (attempts <= 3)
            {
                SubmissionResponse result = Submit(_URI);
                if (result == SubmissionResponse.OK)
                {
                    foreach (Submission item in CachedSubmissions)
                    {
                        item.Content.NewPlays -= item.HandledPlays;
                        //Database.SetPlays(item.Content);
                    }
                    IEnumerable<Submission> _deadTrack = CachedSubmissions.Where(c => c.Content.NewPlays <= 0);
                    int removed = 0;

                    List<Submission> removeRef = new List<Submission>();
                    removeRef.AddRange(_deadTrack);

                    foreach (Submission track in removeRef)
                    {
                        CachedSubmissions.Remove(track);
                        removed++;
                    }
                    Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("Successfully removed {0} items from the cache", removed), LogEvent.LogEventStatus.Neutral);//change
                    //SaveState(filenames);
                    LastSubmission = startTime;
                    break;
                }
                else
                {
                    if (attempts == 3)
                    {
                        foreach (Submission item in CachedSubmissions)
                        {
                            item.HandledPlays = 0;
                        }

                        Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The cached submission did not work.", LogEvent.LogEventStatus.Neutral);//change
                    }
                }
                attempts++;
            }
            //Console.WriteLine(_URI);
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "Finished with ProcessCache()", LogEvent.LogEventStatus.Neutral);//change

        }

        public static void AddTracksToSubmission(List<Track> tracks)
        {
            foreach (Track track in tracks)
            {
                CachedSubmissions.Add(new Submission(track, 0));
            }
            //SaveState(DateTime.Now.Ticks.ToString() + "BACKUP");
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The cache was saved and the thread was started to process it.", LogEvent.LogEventStatus.Neutral);//change
            Thread t = new Thread(delegate() { LastFMHelper.ProcessCache(); });
            t.Start();
        }
        public static void SubmitSingleTrack(Track track)
        {
            int startTime = UnixFromNow();

            string _URI = string.Format("{0}?s={1}&a[0]={2}&t[0]={3}&i[0]={4}&o[0]={5}&r[0]={6}&l[0]={7}&b[0]={8}&n[0]={9}&m[0]={10}", SubmitURI, SessionID, HttpUtility.UrlEncode(track.Artist, subEncoding), HttpUtility.UrlEncode(track.Title, subEncoding), startTime, "P", "", track.Length, HttpUtility.UrlEncode(track.Album, subEncoding), track.TrackNo, "");
            int attempts = 1;
            while (attempts <= 3)
            {
                SubmissionResponse result = Submit(_URI);
                if (result == SubmissionResponse.OK)
                {
                    Log.Instance.AddEvent(LogEvent.LogEventSender.LastFM, "Successfully added the last played track.", LogEvent.LogEventStatus.Success);//change
                    LastSubmission = startTime;
                    break;
                }
                else
                {
                    if (attempts == 3)
                    {
                        CachedSubmissions.Add(new Submission(track, startTime));
                        Log.Instance.AddEvent(LogEvent.LogEventSender.LastFM, "The single submission did not work.  The song was added to the cache.", LogEvent.LogEventStatus.Failure);
                    }
                }
                attempts++;
                //Console.WriteLine(_URI);
            }
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "Finished with SubmitSingleTrack()", LogEvent.LogEventStatus.Neutral);//change
        }
        static ObservableCollection<Submission> CachedSubmissions = new ObservableCollection<Submission>();
        public static void CacheTimerPause()
        {
            CacheTimer.Enabled = false;
        }
        public static void CacheTimerStart()
        {
            CacheTimerSetWait();
            CacheTimer.Enabled = true;
        }
        private static void CacheTimerSetWait()
        {
            int lengthSubmit = 0;// CachedSubmissions.Select(c => c.Content.Length * c.Content.NewPlays).Take(MaxSub).Sum();
            int counter = 0;
            foreach (Submission item in CachedSubmissions)
            {
                if (counter == 0)
                {
                    lengthSubmit = item.Content.Length;
                }
                for (int i = 0; i < item.Content.NewPlays; i++)
                {
                    lengthSubmit += item.Content.Length;
                    counter++;
                    if (counter > MaxSub) break;
                }
                if (counter > MaxSub) break;
            }
            int waitTime = lengthSubmit - (UnixFromNow() - LastSubmission);

            CacheTimer.Interval = (waitTime > 60) ? waitTime * 1000 : 30 * 1000;
            InterfaceHelper.SetCacheFire(DateTime.Now.AddSeconds(CacheTimer.Interval));
        }
        static void temp_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InterfaceHelper.SetCacheSize(CachedSubmissions.Count);
            CacheTimerSetWait();
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("The cache will be processed in {0} seconds.", (CacheTimer.Interval / 1000).ToString()), LogEvent.LogEventStatus.Neutral);//change
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (!CacheTimer.Enabled)
                {
                    CacheTimer.Enabled = true;
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (CachedSubmissions.Count == 0)
                {
                    CacheTimer.Enabled = false;
                    InterfaceHelper.SetCacheFire(DateTime.MinValue);
                }
                else
                {
                    CacheTimer.Enabled = true;
                }
            }
        }

        static LastFMHelper()
        {
            Console.WriteLine("The static constructor for LastFM helper has been called.");
            CachedSubmissions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(temp_CollectionChanged);
        }

        static System.Timers.Timer _CacheTimer = null;
        public static System.Timers.Timer CacheTimer
        {
            get
            {
                if (_CacheTimer == null)
                {
                    _CacheTimer = new System.Timers.Timer();
                    _CacheTimer.Enabled = false;
                    _CacheTimer.Elapsed += new System.Timers.ElapsedEventHandler(_CacheTimer_Elapsed);
                }
                return LastFMHelper._CacheTimer;
            }
        }
        static void _CacheTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("The cache timer has fired.");
            ProcessCache();
        }

        static System.Timers.Timer _PlayTimer = null;
        public static System.Timers.Timer PlayTimer
        {
            get
            {
                if (_PlayTimer == null)
                {
                    _PlayTimer = new System.Timers.Timer();
                    _PlayTimer.Enabled = false;
                    _PlayTimer.Elapsed += new System.Timers.ElapsedEventHandler(_PlayTimer_Elapsed);
                }
                return LastFMHelper._PlayTimer;
            }
            set { LastFMHelper._PlayTimer = value; }
        }
        public static void PausePlayTimer()
        {
            PlayTimer.Enabled = false;
        }
        public static void StartPlayTimer(bool restart)
        {
            if (restart)
            {
                PlayTimerIsSubmitted = false;
            }
            PlayTimerLastStart = DateTime.Now;
            PlayTimer.Interval = 1000;
            PlayTimer.Enabled = true;
        }
        private static DateTime PlayTimerLastStart;
        private static bool PlayTimerIsSubmitted = false;
        static void _PlayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (PlayTimerIsSubmitted)
            {
                PlayTimer.Enabled = false;
                return;
            }
            if (iTunes_WPF.iTunesReference.PlayEventCurrent == null) return;
            int lengthPlayed = iTunes_WPF.iTunesReference.PlayEventCurrent.PlayLength + (int)(DateTime.Now - PlayTimerLastStart).TotalSeconds;
            int lengthTotal = iTunes_WPF.iTunesReference.PlayEventCurrent.CurrentTrack.Length;
            lengthTotal = (lengthTotal > 30) ? ((lengthTotal > 480) ? 480 : lengthTotal) : 0;
            string content = iTunes_WPF.iTunesReference.PlayEventCurrent.ToString();
            double percent = ((double)lengthPlayed) / lengthTotal * 2 * 100;
            if (percent >= 100)
            {
                Log.Instance.AddEvent(LogEvent.LogEventSender.iTunes, "The current track has been played enough to submit.", LogEvent.LogEventStatus.Success);
                PlayTimer.Enabled = false;
                PlayTimerIsSubmitted = true;
                SubmitSingleTrack(iTunes_WPF.iTunesReference.PlayEventCurrent.CurrentTrack);
                //iTunesReference.SubmissionRecent = new Submission(iTunesReference.PlayEventCurrent.CurrentTrack, UnixFromNow());
                InterfaceHelper.SetPlayTimeStatus("submitted");
                Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The play timer has been disposed of.", LogEvent.LogEventStatus.Neutral);
            }
            else
            {
                InterfaceHelper.SetProgressProperties(percent);
                InterfaceHelper.SetPlayTimeStatus((int)((100 - percent) * lengthTotal / 2 / 100));
            }
        }

        public static HandshakeResponse Connect(string username, string password, bool fromFile)
        {

            Username = username;
            int timestamp = UnixFromNow();

            PasswordHash = password;
            string hash = password + timestamp.ToString();

            string token = MD5Helper.MD5(hash);
            string uri = string.Format("http://post.audioscrobbler.com/?hs=true&p=1.2&c=tst&v=1.0&u={0}&t={1}&a={2}", username, timestamp, token);
            Console.WriteLine(uri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Proxy = null;
            request.Timeout = 20 * 1000;
            request.KeepAlive = false;

            try
            {
                using (HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream receiveStream = myHttpWebResponse.GetResponseStream())
                    {
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        using (StreamReader readStream = new StreamReader(receiveStream, encode))
                        {
                            string success = readStream.ReadLine();
                            Console.WriteLine(success);
                            if (success == "OK")
                            {
                                Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "Successfully connected to Last.fm", LogEvent.LogEventStatus.Neutral);//change
                                SessionID = readStream.ReadLine();
                                PlayingURI = readStream.ReadLine();
                                SubmitURI = readStream.ReadLine();
                                ConnectionState = HandshakeResponse.OK;

                                if (!fromFile)
                                {
                                    Username = username;
                                    PasswordHash = password;
                                    SaveUserDetails();
                                }
                                InitializeUserDetails(username);
                                InterfaceHelper.SetLastFMConnected(Username);
                                ProcessCache();

                            }
                            else
                            {
                                ConnectionState = HandshakeResponse.FAILED;
                                Log.Instance.AddEvent(LogEvent.LogEventSender.LastFM, string.Format("The connection failed: {0}.", success), LogEvent.LogEventStatus.Failure);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("Exception while connecting to Last.fm handshake: {0}", e.ToString()), LogEvent.LogEventStatus.Neutral);//change
                return HandshakeResponse.FAILED;
            }
            return ConnectionState;
        }
        private static void SaveUserDetails()
        {
            if (ConnectionState == HandshakeResponse.OK)
            {
                using (FileStream fs = new FileStream("config.dat", FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(true);
                        bw.Write(Username);
                        bw.Write(PasswordHash);
                    }
                }
            }
        }
        public static bool ReadUserDetails()
        {
            if (!File.Exists("config.dat")) return false;
            try
            {
                using (FileStream fs = new FileStream("config.dat", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        if (br.ReadBoolean())
                        {
                            Username = br.ReadString();
                            PasswordHash = br.ReadString();
                        }
                    }
                }
            }
            catch { return false; }

            return true;
        }
        public static void Reconnect()
        {
            Connect(Username, PasswordHash, false);
        }

        private static void InitializeUserDetails(string username)
        {
            try
            {
                string userURI = string.Format("http://ws.audioscrobbler.com/1.0/user/{0}/recenttracks.xml", username);
                Log.Instance.AddEvent(userURI);
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(userURI));
                wr.Proxy = null;
                wr.Timeout = 15 * 1000;
                wr.KeepAlive = false;
                using (HttpWebResponse res = (HttpWebResponse)wr.GetResponse())
                {
                    using (Stream str = res.GetResponseStream())
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        using (XmlReader xmlr = XmlReader.Create(str, settings))
                        {
                            xmlr.ReadStartElement();
                            xmlr.ReadStartElement();
                            xmlr.ReadToNextSibling("date");
                            LastSubmission = int.Parse(xmlr.GetAttribute("uts"));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Instance.AddEvent(e);
                LastSubmission = UnixFromNow() - 60 * 60 * 24;
                Log.Instance.AddEvent("The last submission was set to 1 day ago.");
            }
        }

        public static void AddTracksToCacheFromFile(string filename)
        {
            try
            {
                if (!File.Exists(filename)) return;
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (!sr.EndOfStream)
                        {
                            CachedSubmissions.Add(Submission.Deserialize(sr.ReadLine()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }

        }
        [Obsolete]
        public static void SerializeCacheToFile()
        {
            try
            {
                using (FileStream fs = new FileStream("cache.bin", FileMode.Create))
                {
                    BinaryFormatter bw = new BinaryFormatter();
                    bw.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    bw.Serialize(fs, CachedSubmissions);
                    Log.Instance.AddEvent("The cache was saved to the data file");
                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }
        }
        [Obsolete]
        public static void DeserializeCacheFromFile()
        {
            try
            {
                using (FileStream fs = new FileStream("cache.bin", FileMode.Open))
                {
                    BinaryFormatter bw = new BinaryFormatter();
                    CachedSubmissions = (ObservableCollection<Submission>)bw.Deserialize(fs);
                    InterfaceHelper.SetCacheSize(CachedSubmissions.Count);
                    Log.Instance.AddEvent("The cache was updated from the file");
                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }
        }


        /// <summary>
        /// Processes a given URI.
        /// </summary>
        /// <param name="submitPost">URI to be submitted.</param>
        /// <returns>SubmissionResponse representing the success or failure of submission.</returns>
        public static SubmissionResponse Submit(string submitPost)
        {
            if (ConnectionState == HandshakeResponse.OK)
            {
                Console.Write(submitPost + "\r\n");
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(submitPost);
                wr.Method = "POST";
                wr.Proxy = null;
                wr.Timeout = 30 * 1000;
                wr.KeepAlive = false;
                try
                {
                    using (HttpWebResponse re = (HttpWebResponse)wr.GetResponse())
                    {
                        using (Stream str = re.GetResponseStream())
                        {
                            StreamReader sr = new StreamReader(str, Encoding.UTF8);

                            string reply = sr.ReadLine();
                            if (reply == "OK")
                            {
                                return SubmissionResponse.OK;
                            }
                            else
                            {
                                Log.Instance.AddEvent(reply);
                                return SubmissionResponse.FAILED;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("The submit failed with a web exception. Exception: {0}", e.ToString()), LogEvent.LogEventStatus.Neutral);//change
                    return SubmissionResponse.FAILED;
                }
            }
            else
            {
                Log.Instance.AddEvent(LogEvent.LogEventSender.LastFM, "Submission failed because you are not connected to Last.fm.", LogEvent.LogEventStatus.Failure);//change
                return SubmissionResponse.FAILED;
            }
        }
        static int UnixFromNow()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public enum HandshakeResponse
        {
            OK, BANNED, BADAUTH, BADTIME, FAILED, OTHER
        }
        public enum SubmissionResponse
        {
            OK, BADSESSION, FAILED, OTHER
        }
        public static int RebuildCache(string filename)
        {
            int total = 0;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        string _tempData = sr.ReadLine();
                        Submission _tempSub = Submission.Deserialize(_tempData);
                        CachedSubmissions.Add(_tempSub);
                        total++;
                    }
                }
            }

            ProcessCache();

            return total;
        }
    }
    public class Submission
    {
        Track content;
        public Track Content
        {
            get { return content; }
            set { content = value; }
        }
        int handledPlays = 0;
        public int HandledPlays
        {
            get { return handledPlays; }
            set { handledPlays = value; }
        }
        int submitTime;
        public int SubmitTime
        {
            get { return submitTime; }
            set { submitTime = value; }
        }
        public Submission() { }
        public Submission(Track track, int start)
        {
            Content = track;
            SubmitTime = start;
        }
        public string Serialize()
        {
            string str = string.Join("\t", new string[] { Content.Album, Content.Artist, Content.Filename, Content.Length.ToString(), Content.NewPlays.ToString(), Content.Plays.ToString(), Content.Title, Content.TrackNo.ToString(), SubmitTime.ToString() });

            return str;
        }
        public static Submission Deserialize(string input)
        {
            string[] parts = input.Split(new string[] { "**", "!@!", "\t" }, StringSplitOptions.None);
            Track _tempTrack = new Track(parts[1], parts[0], parts[6], parts[2], int.Parse(parts[7]), int.Parse(parts[3]), int.Parse(parts[5]), int.Parse(parts[4]));
            Submission _tempSub = new Submission(_tempTrack, int.Parse(parts[8]));
            if (_tempSub.Content.NewPlays < 1)
            {
                _tempSub.Content.NewPlays = 1;
            }
            return _tempSub;
        }
    }

}
