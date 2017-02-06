using System;
using NUnit.Framework;
using mRides_app;

namespace mRides_test
{
    [TestFixture]
    public class TestsSample
    {
        Facebook fb = new Facebook();

        [SetUp]
        public void Setup() {
           

        }


        [TearDown]
        public void Tear() { }

        [Test]
        public void Pass()
        {
            Assert.AreEqual(fb.test(2), 2);
            Console.WriteLine("test1");
            
        }

       

        [Test]
        [Ignore("another time")]
        public void Ignore()
        {
            Assert.True(false);
        }

        [Test]
        public void Inconclusive()
        {
            Assert.Inconclusive("Inconclusive");
        }
    }
}