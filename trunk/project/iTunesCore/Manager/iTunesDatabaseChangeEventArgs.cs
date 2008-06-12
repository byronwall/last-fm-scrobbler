using System;
using System.Collections.Generic;

namespace iTunesCore.Manager
{
    public class iTunesDatabaseChangeEventArgs : EventArgs
    {
        public readonly List<DatabaseTrack> TracksChanged;

        public iTunesDatabaseChangeEventArgs(List<DatabaseTrack> tracksChanged)
        {
            TracksChanged = tracksChanged;
        }
    }
}