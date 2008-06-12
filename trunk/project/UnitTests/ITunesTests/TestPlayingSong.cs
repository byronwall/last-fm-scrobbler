using System;
using iTunesCore;
using iTunesCore.Manager;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class PlayingSongTest
    {
        private const string __FILENAME = @"C:\song.mp3";
        private const int _LENGTH = 200;
        private readonly MockTimeProvider _timeProvider = new MockTimeProvider();
        internal static readonly DateTime _startTime = new DateTime(2008, 6, 6, 0, 0, 0);

        private const int _LESSTHANHALFOFFSET = 50;
        private const int _MORETHANHALFOFFSET = 150;

        private PlayingSong _song;
        private DatabaseTrack _track;
        [SetUp]
        public void SetupPlayingSong()
        {

        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _track = new DatabaseTrack { Length = _LENGTH, Filename = __FILENAME };
            _song = new PlayingSong(_track, _timeProvider);

        }

        [Test]
        public void TestInitialStartTime()
        {
            Assert.IsNotNull(_song.StartTime);
        }

        [Test]
        public void TestInitialRecentStopTime()
        {
            Assert.IsNotNull(_song.RecentStopTime);

        }

        [Test]
        public void TestInitialDatabaseTrack()
        {
            Assert.AreEqual(200, _track.Length);
        }

        [Test]
        public void TestInitialIsStopped()
        {
            Assert.AreEqual(true, _song.IsStopped);

        }

        [Test]
        public void TestInitialIsHalfway()
        {
            Assert.AreEqual(false, _song.IsHalfWay);

        }

        [Test]
        public void TestInitialPlayTime()
        {
            Assert.Less(_song.PlayTime, 5);

        }

        [Test]
        public void TestStopLessThanHalfway()
        {
            _song.Start();
            _timeProvider.TimeOffset = _LESSTHANHALFOFFSET;
            _song.Stop();

            Assert.AreEqual(false, _song.IsHalfWay);
            Assert.AreEqual(_LESSTHANHALFOFFSET, _song.PlayTime);


        }
        [Test]
        public void TestStopMoreThanHalfway()
        {
            _song.Start();
            _timeProvider.TimeOffset = _MORETHANHALFOFFSET;
            _song.Stop();

            Assert.AreEqual(true, _song.IsHalfWay);
            Assert.AreEqual(_MORETHANHALFOFFSET, _song.PlayTime);
        }

        [Test]
        public void TestMultipleStartAndStop()
        {
            _song.Start();
            _timeProvider.TimeOffset = _LESSTHANHALFOFFSET;
            _song.Stop();
            _timeProvider.TimeOffset += 30;
            _song.Start();
            _timeProvider.TimeOffset += _MORETHANHALFOFFSET;
            _song.Stop();

            Assert.AreEqual(_MORETHANHALFOFFSET + _LESSTHANHALFOFFSET, _song.PlayTime);
        }

        [Test]
        public void TestPlayTimeWhilePlayingNoStops()
        {
            _song.Start();
            _timeProvider.TimeOffset = _LESSTHANHALFOFFSET;

            Assert.AreEqual(_LESSTHANHALFOFFSET, _song.PlayTime);
        }

        [Test]
        public void TestPlayTimeWhilePlayingWithStops()
        {
            _song.Start();
            _timeProvider.TimeOffset = _LESSTHANHALFOFFSET;
            _song.Stop();
            _timeProvider.TimeOffset += 30;
            _song.Start();
            _timeProvider.TimeOffset += _MORETHANHALFOFFSET;

            Assert.AreEqual(_MORETHANHALFOFFSET + _LESSTHANHALFOFFSET, _song.PlayTime);
        }

        [TearDown]
        public void TearDownPlayingSong()
        {
        }
    }
    class MockTimeProvider : TimeProvider
    {
        public int TimeOffset { get; set; }

        public override DateTime GetCurrentTime()
        {
            return PlayingSongTest._startTime.AddSeconds(TimeOffset);
        }
    }

}
