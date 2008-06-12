using iTunesCore;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class TestReference
    {
        [SetUp]
        public void SetupReference()
        {

        }

        [Test]
        public void Reference()
        {
            DatabaseTrack track = new DatabaseTrack(){PlayCount = 0};
            ChangePlayCount(ref track);
            Assert.AreEqual(2,track.PlayCount);
        }

        private void ChangePlayCount(ref DatabaseTrack track)
        {
            track.PlayCount = 2;
        }

        [TearDown]
        public void TearDownReference()
        {
        }
    }
}
