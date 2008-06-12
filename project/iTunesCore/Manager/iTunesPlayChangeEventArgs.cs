using System;
using iTunesLib;

namespace iTunesCore.Manager
{
    public class iTunesPlayChangeEventArgs: EventArgs
    {
        public readonly ITunesSongChangeType ChangeType;
        public readonly IITFileOrCDTrack ChangedTrack;

        public iTunesPlayChangeEventArgs(ITunesSongChangeType changeType, IITFileOrCDTrack changedTrack)
        {
            ChangeType = changeType;
            ChangedTrack = changedTrack;
        }
    }

    public enum ITunesSongChangeType
    {
        Play, Pause, Stop
    }
}