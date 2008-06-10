using iTunesCore;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class TestiTunesDatabaseWithFile
    {
        private const string filename = @"F:\My Documents\My Music\iTunes\iTunes Music Library.xml";
        private readonly iTunesDatabase Database = new iTunesDatabase(new ITunesDatabaseProviderFromFile(filename));

        [SetUp]
        public void SetupTest()
        {

        }
        [Test]
        public void InitialObject()
        {
            Assert.AreEqual(Database.State, DatabaseState.Unloaded);

        }

        [Test]
        public void HasElements()
        {
            Database.LoadDatabase();
            Assert.Greater(Database.Tracks.Count, 0);
        }
        [TearDown]
        public void TearDownTest()
        {
        }

    }
}
