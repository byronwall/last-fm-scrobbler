using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;
using LastFM.TrackInfo;

namespace iTunes_WPF
{
    public class iTunesPlayEvent
    {
        private DateTime? _startTime = null;
        private DateTime? _restartTime = null;
        private DateTime? _endTime = null;
        private int _playLength = 0;
        private bool _fullPlay = false;
        private double _percentage = 0.5;
        private Track _currentTrack = null;

        public bool Stopped = false;
        public iTunesPlayEvent()
        {

        }
        public Track CurrentTrack
        {
            get
            {
                return _currentTrack;
            }
            set
            {
                _currentTrack = value;
            }
        }
        public DateTime StartTime
        {
            get
            {
                return _startTime.Value;
            }
            set
            {
                _startTime = value;
            }
        }
        public DateTime RestartTime
        {
            get
            {
                return _restartTime.Value;
            }
            set
            {
                _restartTime = value;
            }
        }
        public DateTime EndTime
        {
            get
            {
                return _endTime.Value;
            }
            set
            {
                _endTime = value;
                if (_restartTime == null)
                {
                    PlayLength += (int)(_endTime - _startTime).Value.TotalSeconds;
                }
                else
                {
                    PlayLength += (int)(_endTime - _restartTime).Value.TotalSeconds;
                }

            }
        }
        public int PlayLength
        {
            get
            {
                return _playLength;
            }
            set
            {
                _playLength = value;
                if (_playLength > _percentage * CurrentTrack.Length)
                {
                    _fullPlay = true;
                }
            }
        }
        public bool FullPlay
        {
            get
            {
                return _fullPlay;
            }
        }
        public override string ToString()
        {
            return String.Format("{0} - {1}", _currentTrack.Artist, _currentTrack.Title);
        }
    }
}
