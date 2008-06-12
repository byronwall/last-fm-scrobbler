using System;
using System.Collections.Generic;
using System.Threading;
using iTunesLib;

namespace iTunesCore.Manager
{
    public class ITunesManager
    {
        private Timer _timer;
        private iTunesDatabase _database;
        private PlayingSong _currentSong;

        public event EventHandler<iTunesPlayChangeEventArgs> ITunesPlayChanged;
        protected virtual void OniTunesPlayChanged(iTunesPlayChangeEventArgs e)
        {
            if (ITunesPlayChanged != null)
            {
                ITunesPlayChanged(this, e);
            }
        }

        public event EventHandler<iTunesDatabaseChangeEventArgs> ITunesSubmissionReady;

        protected virtual void OniTunesSubmissionReady(iTunesDatabaseChangeEventArgs e)
        {
            if (ITunesSubmissionReady != null) ITunesSubmissionReady(this, e);
        }

        public ITunesManager()
        {
            initialize();
        }

        private void initialize()
        {
            iTunesInstance.Instance.OnDatabaseChangedEvent += instance_OnDatabaseChangedEvent;
            iTunesInstance.Instance.OnPlayerPlayEvent += instance_OnPlayerPlayEvent;
            iTunesInstance.Instance.OnPlayerStopEvent += instance_OnPlayerStopEvent;

            _database = new iTunesDatabase(iTunesInstance.Instance.LibraryXMLPath);
            _timer = new Timer(new TimerCallback(TimerCallbackTarget), null, 0, int.MaxValue);
        }

        private void TimerCallbackTarget(object state)
        {
            if (_currentSong == null) return;

            if (_currentSong.IsHalfWay)
            {
                if (_currentSong.Track != null)
                    OniTunesSubmissionReady(new iTunesDatabaseChangeEventArgs(new List<DatabaseTrack>() { _currentSong.Track }));
            }
        }

        void instance_OnPlayerStopEvent(object iTrack)
        {
            IITFileOrCDTrack track = (IITFileOrCDTrack)iTrack;

            if (_currentSong == null) return;

            _currentSong.Stop();


        }

        void instance_OnPlayerPlayEvent(object iTrack)
        {
            DatabaseTrack track = DatabaseTrack.FromIITTrack((IITFileOrCDTrack)iTrack);

            if (_currentSong == null)
            {
                //New song being played or song played for first time
                _currentSong = new PlayingSong(track);
            }
            else if (_currentSong.Track.Filename == track.Filename)
            {
                //Same song being played again or restarted
                if (iTunesInstance.Instance.PlayerPosition > 5)
                {
                    //This is the same song starting from a pause.
                    _currentSong.Start();
                    _currentSong.RecentStopTime = DateTime.Now;
                }
                else
                {
                    //This is the same song starting over
                    _currentSong = new PlayingSong(track);

                }
            }
            else
            {
                //New song being played
                _currentSong = new PlayingSong(track);

            }
            _timer.Change(0, 1000);

        }

        void instance_OnDatabaseChangedEvent(object deletedObjectIDs, object changedObjectIDs)
        {
            object[,] changedObjects = (object[,])changedObjectIDs;

            if (changedObjects.GetUpperBound(0) > iTunesInstance.Instance.LibraryPlaylist.Tracks.Count)
            {
                return;
            }

            int[,] changedTrackIDs = new int[changedObjects.GetUpperBound(0) + 1, changedObjects.GetUpperBound(1) + 1];
            for (int i = 0; i <= changedObjects.GetUpperBound(0); i++)
            {
                for (int j = 0; j < changedObjects.GetUpperBound(1); j++)
                {
                    changedTrackIDs[i, j] = (int)changedObjects[i, j];
                }
            }

            ThreadPool.QueueUserWorkItem(analyzeDatabaseChange, changedTrackIDs);
        }

        private void analyzeDatabaseChange(object state)
        {
            List<DatabaseTrack> tracksChanged = new List<DatabaseTrack>();
            int[,] changedTrackIDs = (int[,])state;

            int librarySourceID = iTunesInstance.Instance.LibraryPlaylist.sourceID;
            int libraryPlaylistID = iTunesInstance.Instance.LibraryPlaylist.playlistID;

            for (int i = 0; i <= changedTrackIDs.GetUpperBound(0); i++)
            {
                int sourceID = changedTrackIDs[i, 0];
                int playlistID = changedTrackIDs[i, 1];
                int trackID = changedTrackIDs[i, 2];
                int trackDatabaseID = changedTrackIDs[i, 3];

                if (sourceID != librarySourceID || playlistID != libraryPlaylistID || trackDatabaseID <= 0) continue;
                IITFileOrCDTrack track = (IITFileOrCDTrack)iTunesInstance.Instance.GetITObjectByID(sourceID, playlistID, trackID, trackDatabaseID);

                DatabaseTrack databaseTrack = DatabaseTrack.FromIITTrack(track);
                if (_database.RetrieveOrAddPlayCount(ref databaseTrack) <= 0) continue;
                tracksChanged.Add(DatabaseTrack.FromIITTrack(track));
            }
            if (tracksChanged.Count > 0)
            {
                OniTunesSubmissionReady(new iTunesDatabaseChangeEventArgs(tracksChanged));
            }

        }
    }
}