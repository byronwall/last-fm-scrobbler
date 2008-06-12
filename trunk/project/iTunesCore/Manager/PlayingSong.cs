using System;

namespace iTunesCore.Manager
{
    public class PlayingSong
    {
        private readonly TimeProvider _timeProvider;
        public PlayingSong(DatabaseTrack track)
        {
            _timeProvider = new TimeProvider();
            Track = track;
            StartTime = _timeProvider.GetCurrentTime();
            RecentStopTime = StartTime;
        }

        public PlayingSong(DatabaseTrack track, TimeProvider timeProvider)
        {
            Track = track;
            _timeProvider = timeProvider;
            StartTime = _timeProvider.GetCurrentTime();
            RecentStopTime = StartTime;
        }


        public DatabaseTrack Track { get; set; }

        private bool _isStopped = true;
        public bool IsStopped
        {
            get { return _isStopped; }
            private set
            {
                _isStopped = value;
            }
        }

        public bool IsHalfWay
        {
            get
            {
                return PlayTime > Track.Length / 2;
            }
        }

        public DateTime StartTime { get; set; }

        private double _playTime;
        public double PlayTime
        {
            get
            {
                if (!IsStopped)
                {
                    return _playTime + (_timeProvider.GetCurrentTime() - RecentStopTime).TotalSeconds;
                }
                return _playTime;
            }
            set { _playTime = value; }
        }

        public DateTime RecentStopTime { get; set; }

        public void Stop()
        {
            IsStopped = true;
            PlayTime += (_timeProvider.GetCurrentTime() - RecentStopTime).TotalSeconds;
        }
        public void Start()
        {
            IsStopped = false;
            RecentStopTime = _timeProvider.GetCurrentTime();
        }

    }

    public class TimeProvider
    {
        public virtual DateTime GetCurrentTime() { return DateTime.Now; }
    }
}