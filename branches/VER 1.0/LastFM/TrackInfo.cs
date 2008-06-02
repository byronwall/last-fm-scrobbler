using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using iTunesLib;

namespace LastFM.TrackInfo
{
    public class Database
    {
        private const string __NULLALBUM = "ALBUMNULLNULL";
        private const string __NULLARTIST = "ARTISTNULLNULL";
        private static Dictionary<string, long> _Tracks = new Dictionary<string, long>();
        public static Dictionary<string, long> Tracks
        {
            get { return Database._Tracks; }
            set { Database._Tracks = value; }
        }
        public static bool DBActive = false;
        public static bool UpdateDB()
        {
            //using (FileStream fs = new FileStream("db.dat", FileMode.Create))
            //{
            //    Log.Instance.AddEvent(LogEvent.LogEventSender.Database, "The database file was initialzied, about to be filled.", LogEvent.LogEventStatus.Success);
            //}
            int totalSongs = iTunes_WPF.iTunesReference.TotalSongs;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.ProhibitDtd = false;

            Stopwatch sw = Stopwatch.StartNew();
            string library = iTunes_WPF.iTunesReference.Instance.LibraryXMLPath;
            using (XmlReader r = XmlReader.Create(library, settings))
            {
                r.ReadStartElement();

                r.ReadStartElement("dict");
                r.ReadToNextSibling("dict");
                r.ReadStartElement("dict");
                using (FileStream fs = new FileStream("db.dat", FileMode.Append))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        while (r.Name == "key")
                        {
                            try
                            {
                                r.ReadToNextSibling("dict");
                                Track _track = Track.FromXML(r);
                                AddTrack(_track, bw, fs.Position);
                            }
                            catch (Exception e)
                            {
                                Log.Instance.AddEvent(e);
                            }
                        }
                    }
                }
            }
            sw.Stop();
            var t = sw.Elapsed;
            DBActive = true;

            Log.Instance.AddEvent(LogEvent.LogEventSender.Database, String.Format("The database has been updated in {0} seconds.", sw.Elapsed.TotalSeconds.ToString()), LogEvent.LogEventStatus.Success);//change
            InterfaceHelper.SetDBConnected(true);
            return true;
        }

        public static void AddTrack(Track track, BinaryWriter bw, long offset)
        {
            //TODO: Fix this so that it can handle NULL types
            try
            {
                Tracks.Add(track.Hash, offset);

                bw.Write(track.Plays);
                bw.Write(track.Length);
                bw.Write(track.TrackNo);
                bw.Write(track.Title);
                if (track.Album == null)
                {
                    bw.Write(__NULLALBUM);
                }
                else
                {
                    bw.Write(track.Album);
                }
                if (track.Artist == null)
                {
                    bw.Write(__NULLARTIST);
                }
                else
                {
                    bw.Write(track.Artist);
                }
            }
            catch (System.ArgumentException)
            {
                Log.Instance.AddEvent("There was a repeat in the hash: " + track.Filename);
            }
        }
        public static void AddTrack(Track track)
        {

            using (FileStream fs = new FileStream("db.dat", FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    AddTrack(track, bw, fs.Position);
                }
            }
        }
        public static Track RetrieveTrack(IITFileOrCDTrack track, out bool dbActive)
        {
            dbActive = true;
            Track _temp;
            if (DBActive == false)
            {
                dbActive = false;
                return null;
            }

            if (Tracks.ContainsKey(Track.GetStringHash(track)))
            {
                //Log.Instance.AddEvent(LogEvent.LogEventSender.Database, string.Format("A track is being retrieved: {0}", track.Location), LogEvent.LogEventStatus.Neutral);
                long offset = Tracks[Track.GetStringHash(track)];
                _temp = new Track();
                try
                {
                    using (FileStream fs = new FileStream("db.dat", FileMode.Open))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            fs.Seek(offset, SeekOrigin.Begin);
                            _temp.Plays = br.ReadInt32();
                            _temp.Length = br.ReadInt32();
                            _temp.TrackNo = br.ReadInt32();
                            _temp.Title = br.ReadString();
                            _temp.Album = br.ReadString();
                            if (_temp.Album == __NULLALBUM)
                            {
                                _temp.Album = null;
                            }
                            _temp.Artist = br.ReadString();
                            if (_temp.Artist == __NULLARTIST)
                            {
                                _temp.Artist = null;
                            }
                        }
                    }
                    _temp.Filename = track.Location;
                    return _temp;
                }
                catch (Exception e)
                {
                    Log.Instance.AddEvent(e);
                    return null;
                }
            }
            else
            {
                _temp = Track.FromIITTrack(track);
                if (_temp.Plays > 0)
                {
                    Log.Instance.AddEvent("A track was added with more than 0 plays. ERROR.");
                }
                //else
                //{
                //    //Log.Instance.AddEvent("A normal track was added to the DB from an iTunes call.");
                //}
                AddTrack(_temp);
                return _temp;
            }
        }
        public static void SetPlays(Track track)
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Database, string.Format("A track is being changed plays: {0}", track.Filename), LogEvent.LogEventStatus.Neutral);
            long offset = Tracks[track.Hash];
            try
            {
                using (FileStream fs = new FileStream("db.dat", FileMode.Open))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        bw.Write(track.Plays);
                        //bw.Write(track.NewPlays);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }
        }
        public static void DeleteDB()
        {
            try
            {
                File.Delete("db.dat");
            }
            catch
            {
                Log.Instance.AddEvent(LogEvent.LogEventSender.Database, "The DB file was not deleted.", LogEvent.LogEventStatus.Failure);
            }
        }
    }
    public class Track
    {
        const int __HASH_LENGTH = 20;
        string _artist, _album, _title, _filename;
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string Album
        {
            get { return _album; }
            set { _album = value; }
        }
        public string Artist
        {
            get { return _artist; }
            set { _artist = value; }
        }
        int _length, _trackNo, _plays, _newPlays, _trackDBID;
        public int TrackDBID
        {
            get { return _trackDBID; }
            set { _trackDBID = value; }
        }
        public int NewPlays
        {
            get { return _newPlays; }
            set { _newPlays = value; }
        }
        public int Plays
        {
            get { return _plays; }
            set { _plays = value; }
        }
        public int TrackNo
        {
            get { return _trackNo; }
            set { _trackNo = value; }
        }
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        private string _hash = string.Empty;
        public string Hash
        {
            get
            {
                if (_hash == string.Empty)
                {
                    _hash = GetStringHash();
                }
                return _hash;
            }
        }
        public Track() { }
        public Track(string artist, string album, string title, string filename, int trackNo, int length, int plays, int newplays)
        {
            Artist = artist;
            Album = album;
            Title = title;
            TrackNo = trackNo;
            Length = length;
            Filename = filename;
            Plays = plays;
            NewPlays = newplays;
        }
        public static Track FromIITTrack(iTunesLib.IITFileOrCDTrack item)
        {
            //Track _temp = new Track(item.Artist, item.Album, item.Name, item.Location, item.TrackNumber, item.Duration, item.PlayedCount, 0);
            Track temp = new Track() { Album = item.Album, Artist = item.Artist, Filename = item.Location, Length = item.Duration, NewPlays = 0, Plays = item.PlayedCount, Title = item.Name, TrackNo = item.TrackNumber };
            return temp;
        }
        public static Track FromXML(XmlReader r)
        {
            Track _track = new Track();

            XElement x = (XElement)XNode.ReadFrom(r);
            foreach (XElement node in x.Elements("key"))
            {
                if (node.Value == "Name") _track.Title = (string)((XElement)node.NextNode);
                else if (node.Value == "Artist") _track.Artist = (string)((XElement)node.NextNode);
                else if (node.Value == "Album") _track.Album = (string)((XElement)node.NextNode);
                else if (node.Value == "Total Time") _track.Length = ((int)((XElement)node.NextNode)) / 1000;
                else if (node.Value == "Track Number") _track.TrackNo = (int)((XElement)node.NextNode);
                else if (node.Value == "Location") _track.Filename = Uri.UnescapeDataString(((string)((XElement)node.NextNode)).Replace("file://localhost/", "").Replace("/", @"\"));
                else if (node.Value == "Play Count") _track.Plays = (int)((XElement)node.NextNode);
                //else if (node.Value == "Track ID") _track.TrackDBID = (int)((XElement)node.NextNode);
            }
            return _track;

        }
        public string GetStringHash()
        {
            return MD5Helper.MD5(Filename);
        }
        public static string GetStringHash(IITFileOrCDTrack track)
        {
            return MD5Helper.MD5(track.Location);
        }
    }
}
