using System.Xml.Linq;
using iTunesCore;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class TestiTunesDatabase
    {
        private readonly iTunesDatabase database = new iTunesDatabase(new MockITunesDatabaseProvider());

        private const string __filename = @"F:\ASong.mp3";
        private const int __playcount = 1;

        [SetUp]
        public void SetupiTunesDatabase()
        {

        }

        [Test]
        public void InitialObject()
        {
            Assert.AreEqual(database.State, iTunesCore.DatabaseState.Unloaded);
        }

        [Test]
        public void LoadDatabase()
        {
            database.LoadDatabase();
            Assert.AreEqual(database.State, iTunesCore.DatabaseState.Loaded);
        }

        [Test]
        public void CheckDatabaseCount()
        {
            Assert.AreEqual(database.Tracks.Count, 1);
        }

        [Test]
        public void CheckDatabaseKey()
        {
            Assert.IsTrue(database.Tracks.ContainsKey(__filename));
        }

        [Test]
        public void CheckDatabaseValue()
        {
            Assert.IsTrue(database.Tracks.ContainsValue(__playcount));
        }

        [Test]
        public void RetrieveExistingPlayCount()
        {
            DatabaseTrack track = new DatabaseTrack() { Filename = __filename, PlayCount = __playcount };
            int newPlays = database.RetrieveOrAddPlayCount(ref track);

            Assert.AreEqual(track.NewPlays, newPlays);
        }

        [Test]
        public void RetrieveNonExisitingPlayCount()
        {
            const string _tempFilename = @"C:\Filename.mp3";
            const int _newPlays = 3;
            DatabaseTrack track = new DatabaseTrack() { Filename = _tempFilename, PlayCount = _newPlays };
            int newPlays = database.RetrieveOrAddPlayCount(ref track);

            Assert.AreEqual(track.NewPlays, newPlays);
        }

        [Test]
        public void CheckUpdatedDatabase()
        {
            DatabaseTrack track = new DatabaseTrack() { Filename = __filename };
            database.RetrieveOrAddPlayCount(ref track);
            track.PlayCount = 3;
            database.RetrieveOrAddPlayCount(ref track);
            int playCount = database.GetPlayCount(ref track);
            Assert.AreEqual(track.PlayCount, playCount);
        }

        [Test]
        public void AssertUnknownTrackWasAdded()
        {
            Assert.AreEqual(database.Tracks.Count, 2);
        }
        [TearDown]
        public void TearDowniTunesDatabase()
        {

        }
    }
    public class MockITunesDatabaseProvider : IITunesDatabaseProvider
    {
        private const string __XMLDATA = @"<plist>
<dict>
	<key>Major Version</key><integer>1</integer>
	<key>Minor Version</key><integer>1</integer>
	<key>Application Version</key><string>7.6.2</string>
	<key>Features</key><integer>5</integer>
	<key>Show Content Ratings</key><true/>
	<key>Music Folder</key><string>file://localhost/F:/My%20Documents/My%20Music/</string>
	<key>Library Persistent ID</key><string>369A374D81FA90E5</string>
	<key>Tracks</key>
	<dict>
		<key>1140</key>
		<dict>
			<key>Track ID</key><integer>1140</integer>
			<key>Name</key><string>E-Pro</string>
			<key>Artist</key><string>Beck</string>
			<key>Album</key><string>Guero</string>
			<key>Genre</key><string>Alternative</string>
			<key>Kind</key><string>MPEG audio file</string>
			<key>Size</key><integer>6480051</integer>
			<key>Total Time</key><integer>202448</integer>
			<key>Track Number</key><integer>1</integer>
			<key>Year</key><integer>2005</integer>
			<key>Date Modified</key><date>2007-11-13T01:03:42Z</date>
			<key>Date Added</key><date>2007-09-21T04:51:33Z</date>
			<key>Bit Rate</key><integer>256</integer>
			<key>Sample Rate</key><integer>44100</integer>
			<key>Play Count</key><integer>1</integer>
			<key>Play Date</key><integer>3284644155</integer>
			<key>Play Date UTC</key><date>2008-01-31T21:09:15Z</date>
			<key>Album Rating</key><integer>100</integer>
			<key>Album Rating Computed</key><true/>
			<key>Persistent ID</key><string>369A374D81FA9110</string>
			<key>Track Type</key><string>File</string>
			<key>Location</key><string>file://localhost/F:/ASong.mp3</string>
			<key>File Folder Count</key><integer>4</integer>
			<key>Library Folder Count</key><integer>1</integer>
		</dict></dict></dict></plist>";

        public XDocument DatabaseXMLReader
        {
            get
            {
                return XDocument.Parse(__XMLDATA);
            }
        }
    }
}