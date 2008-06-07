using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Class1
    {
        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public void TrueTest()
        {
            Assert.IsTrue(true);

        }
        [TearDown]
        public void TearDown()
        {
        }
    }
}
