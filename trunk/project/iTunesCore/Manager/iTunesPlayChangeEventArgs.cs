using System;
using iTunesLib;

namespace iTunesCore.Manager
{
    class iTunesPlayChangeEventArgs: EventArgs
    {
        public readonly iTunesSongChangeType ChangeType;
        public readonly IITFileOrCDTrack ChangedTrack;

        public iTunesPlayChangeEventArgs(iTunesSongChangeType changeType, IITFileOrCDTrack changedTrack)
        {
            ChangeType = changeType;
            ChangedTrack = changedTrack;
        }
    }

    internal enum iTunesSongChangeType
    {
        Play, Pause, Stop
    }
}