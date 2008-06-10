using System;
using iTunesLib;

namespace iTunesCore.Manager
{
    class iTunesManager
    {
        public event EventHandler<iTunesPlayChangeEventArgs> iTunesPlayChanged;
        protected virtual void OniTunesPlayChanged(iTunesPlayChangeEventArgs e)
        {
            if (iTunesPlayChanged != null)
            {
                iTunesPlayChanged(this, e);
            }
        }
        
        public event EventHandler<iTunesDatabaseChangeEventArgs> iTunesDatabaseChanged;

        protected virtual void OniTunesDatabaseChanged(iTunesDatabaseChangeEventArgs e)
        {
            if (iTunesDatabaseChanged != null)
            {
                iTunesDatabaseChanged(this, e);
            }
        }

        public iTunesManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            iTunesInstance.Instance.OnDatabaseChangedEvent += Instance_OnDatabaseChangedEvent;
            iTunesInstance.Instance.OnPlayerPlayEvent += Instance_OnPlayerPlayEvent;
            iTunesInstance.Instance.OnPlayerStopEvent += Instance_OnPlayerStopEvent;
        }

        void Instance_OnPlayerStopEvent(object iTrack)
        {
            OniTunesPlayChanged(new iTunesPlayChangeEventArgs(iTunesSongChangeType.Stop, (IITFileOrCDTrack) iTrack));
        }

        void Instance_OnPlayerPlayEvent(object iTrack)
        {
            OniTunesPlayChanged(new iTunesPlayChangeEventArgs(iTunesSongChangeType.Play, (IITFileOrCDTrack)iTrack));
        }

        void Instance_OnDatabaseChangedEvent(object deletedObjectIDs, object changedObjectIDs)
        {
            throw new NotImplementedException();
        }
    }
}