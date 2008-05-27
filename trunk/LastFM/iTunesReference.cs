using System;
using System.Collections.Generic;
using iTunesLib;
using LastFM;
using LastFM.TrackInfo;

namespace iTunes_WPF
{
    public sealed class iTunesReference
    {
        static iTunesApp _instance = null;
        static iTunesPlayEvent _currentlyPlaying = null;
        static object locker = new object();
        public static object connectLocker = new object();
        const int _starterSeconds = 3;
        public static int _SourceID = -1;
        public static int SourceID
        {
            get
            {
                if (_SourceID == -1)
                {
                    _SourceID = Instance.LibraryPlaylist.sourceID;
                }
                return _SourceID;
            }
        }
        private static int _PlaylistID = -1;
        public static int PlaylistID
        {
            get
            {
                if (_PlaylistID == -1)
                {
                    _PlaylistID = Instance.LibraryPlaylist.playlistID;
                }
                return _PlaylistID;
            }
        }
        public static int TotalSongs
        {
            get
            {
                return Instance.LibraryPlaylist.Tracks.Count;
            }
        }
        public static iTunesPlayEvent PlayEventCurrent
        {
            get
            {
                return _currentlyPlaying;
            }
        }
        public static iTunesApp Instance
        {
            get
            {
                lock (connectLocker)
                {
                    if (_instance == null)
                    {
                        Initialize();
                    }
                }
                return _instance;
            }
        }

        private iTunesReference()
        {

        }
        public static void Initialize()
        {
            if (_instance == null)
            {
                Log.Instance.AddEvent("iTunes is being initialized.");
                _instance = new iTunesApp();
                Log.Instance.AddEvent("iTunes object was created, wiring up the events.");
                _instance.OnDatabaseChangedEvent += new _IiTunesEvents_OnDatabaseChangedEventEventHandler(iTunesDatabaseMethod);
                _instance.OnPlayerPlayEvent += new _IiTunesEvents_OnPlayerPlayEventEventHandler(iTunesPlayMethod);
                _instance.OnPlayerStopEvent += new _IiTunesEvents_OnPlayerStopEventEventHandler(iTuneStopMethod);
                _instance.OnCOMCallsDisabledEvent += new _IiTunesEvents_OnCOMCallsDisabledEventEventHandler(iTunesCOMDisabled);
                _instance.OnCOMCallsEnabledEvent += new _IiTunesEvents_OnCOMCallsEnabledEventEventHandler(iTunesCOMEnabled);
                _instance.OnAboutToPromptUserToQuitEvent += new _IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler(iTunesQutting);
                Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "Events are now watching iTunes", LogEvent.LogEventStatus.Neutral);//change
                InterfaceHelper.SetITunesConnected(true);
            }
            Log.Instance.AddEvent("iTunes is done connecting.");
        }

        static void iTunesQutting()
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "iTunes is quitting.  Releasing objects", LogEvent.LogEventStatus.Neutral);//change
            System.Runtime.InteropServices.Marshal.ReleaseComObject(_instance);
            _instance = null;
            GC.Collect();
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The objects have been released.", LogEvent.LogEventStatus.Neutral);//change
            InterfaceHelper.SetITunesConnected(false);
        }
        static void iTunesCOMEnabled()
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "COM calls have been enabled", LogEvent.LogEventStatus.Neutral);//change
        }
        static void iTunesCOMDisabled(ITCOMDisabledReason reason)
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, string.Format("COM calls have been disabled: {0}", reason.ToString()), LogEvent.LogEventStatus.Neutral);//change
        }
        static void iTuneStopMethod(object iTrack)
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("A stop event occurred: {0}", (iTrack as IITTrack).Name), LogEvent.LogEventStatus.Neutral);//change
            int position = iTunesReference.Instance.PlayerPosition;
            if (iTunesReference.Instance.PlayerPosition > _starterSeconds)
            {
                //this is most likely caused by a pause event
                if (_currentlyPlaying != null)
                {
                    _currentlyPlaying.EndTime = DateTime.Now;
                }
                LastFMHelper.PausePlayTimer();
                Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The stop event is determined to be a pause.", LogEvent.LogEventStatus.Neutral);//change
            }
            else
            {
                //this is most likely caused by a stop event
                Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The stop is determined to be an actual stop.", LogEvent.LogEventStatus.Neutral);//change
                _stopAddClearCurrentlyPlaying((IITFileOrCDTrack)iTrack);
            }
        }
        static void iTunesPlayMethod(object iTrack)
        {
            LastFMHelper.CacheTimerPause();
            IITFileOrCDTrack _iTrack = iTrack as IITFileOrCDTrack;
            InterfaceHelper.SetPlayingAlbum(_iTrack.Album);
            InterfaceHelper.SetPlayingArtist(_iTrack.Artist);
            InterfaceHelper.SetPlayingTitle(_iTrack.Name);
            if (_currentlyPlaying == null)
            {
                _currentlyPlaying = new iTunesPlayEvent { StartTime = DateTime.Now, CurrentTrack = Track.FromIITTrack(_iTrack) };
                LastFMHelper.StartPlayTimer(true);
            }
            else if (!_currentlyPlaying.Stopped)
            {
                if (_iTrack.Location == _currentlyPlaying.CurrentTrack.Filename)
                {
                    if (Instance.PlayerPosition > _starterSeconds)
                    {
                        _currentlyPlaying.RestartTime = DateTime.Now;
                        LastFMHelper.StartPlayTimer(false);
                        Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The play event is a resume from a pause.", LogEvent.LogEventStatus.Neutral);//change
                    }
                    else
                    {
                        Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The play event is the same song starting again.", LogEvent.LogEventStatus.Neutral);//change
                        _stopAddClearCurrentlyPlaying((IITFileOrCDTrack)iTrack);
                        LastFMHelper.StartPlayTimer(true);
                    }
                }
                else
                {
                    _stopAddClearCurrentlyPlaying((IITFileOrCDTrack)iTrack);
                    LastFMHelper.StartPlayTimer(true);
                    Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The play event is a song change after a pause.", LogEvent.LogEventStatus.Neutral);//change
                }
            }
            else
            {
                Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "There is an error: the player is calling for a new track at the wrong time", LogEvent.LogEventStatus.Neutral);//change
            }
            //LastFMHelper.StartPlayTimer(false);
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("A play event occurred: {0}", (iTrack as IITTrack).Name), LogEvent.LogEventStatus.Neutral);//change
        }
        static void _stopAddClearCurrentlyPlaying(IITFileOrCDTrack temp)
        {
            if (_currentlyPlaying != null)
            {
                Log.Instance.AddEvent(LogEvent.LogEventSender.iTunes, "The process has been started to close a playing track.", LogEvent.LogEventStatus.Neutral);
                _currentlyPlaying = null;
                LastFMHelper.CacheTimerStart();
            }
        }
        static void iTunesDatabaseMethod(object deletedObjectIDs, object changedObjectIDs)
        {
            lock (locker)
            {
                object[,] _changedObjectIDs = (object[,])changedObjectIDs;
                int _changedItems = _changedObjectIDs.GetLength(0);
                Track _tempTrack = null;
                if (_changedItems > 0)
                {
                    List<Track> PlayedSongs = new List<Track>();
                    Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("Changed {0} items", _changedItems), LogEvent.LogEventStatus.Neutral);//change
                    if (_changedItems > iTunesReference.TotalSongs * 75 / 100)
                    {
                        Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "Too many items changed (more than 75%, probably initial load))", LogEvent.LogEventStatus.Neutral);//change
                    }
                    else
                    {
                        for (int i = 0; i < _changedItems; i++)
                        {
                            int _sourceID = (int)_changedObjectIDs[i, 0];
                            int _playlistID = (int)_changedObjectIDs[i, 1];
                            int _trackID = (int)_changedObjectIDs[i, 2];
                            int _trackDBID = (int)_changedObjectIDs[i, 3];

                            if (_sourceID == iTunesReference.SourceID && _playlistID == iTunesReference.PlaylistID && _trackDBID > 0)
                            {
                                IITFileOrCDTrack _track = (IITFileOrCDTrack)iTunesReference.Instance.GetITObjectByID(_sourceID, _playlistID, _trackID, _trackDBID);
                                bool dbActive;

                                _tempTrack = Database.RetrieveTrack(_track, out dbActive);
                                if (_tempTrack != null)
                                {
                                    int playDif = _track.PlayedCount - _tempTrack.Plays;
                                    if (playDif > 0)
                                    {
                                        Log.Instance.AddEvent(LogEvent.LogEventSender.iTunes, String.Format("Track {0} - {1} played {2} times", _track.Artist, _track.Name, playDif), LogEvent.LogEventStatus.Neutral);
                                        _tempTrack.NewPlays = playDif;
                                        _tempTrack.Plays = _track.PlayedCount;
                                        Database.SetPlays(_tempTrack);
                                        PlayedSongs.Add(_tempTrack);
                                    }
                                }
                                else
                                {
                                    if (dbActive)
                                    {
                                        Log.Instance.AddEvent("Serious error occurred while trying to retrieve track.");
                                    }
                                    else
                                    {
                                        Log.Instance.AddEvent("The DB is not active and the change was ignored.");
                                    }
                                }
                            }
                        }
                    }
                    if (PlayedSongs.Count > 0)
                    {
                        if (PlayedSongs.Count == 1 && PlayedSongs[0].NewPlays <= 1)
                        {
                            Log.Instance.AddEvent(LogEvent.LogEventSender.iTunes, "A single song was changed in the iTunes DB and it is going to be ignored for Last.fm submission.", LogEvent.LogEventStatus.Neutral);
                            //Track _justPlayed = PlayedSongs[0];
                            //_justPlayed.NewPlays = 0;
                            //_justPlayed.Plays++;
                            //Database.SetPlays(_justPlayed);
                            //Track _reallytemp = Database.RetrieveTrack(_justPlayed.TrackDBID);

                        }
                        else
                        {
                            Log.Instance.AddEvent(LogEvent.LogEventSender.iTunes, "Multiple songs were played.  Cache engaged.", LogEvent.LogEventStatus.Neutral);
                            LastFMHelper.AddTracksToSubmission(PlayedSongs);
                        }
                        PlayedSongs.Clear();
                    }
                }
            }
        }

    }

}
