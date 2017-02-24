using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;
using mRides_app;

namespace UITest
{
    [TestFixture]
    public class UnitTest
    {
        [SetUp]
        public void Setup() { }

        [TearDown]
        public void Tear() { }

        [Test]
        public void TestMe_OneToFive()
        {
            int num = TestMe.OneToFive();
            Assert.IsTrue(num >= 1 && num <= 5);
        }

        [Test]
        public void TestMe_Counter()
        {
            TestMe testedObj = new TestMe(1);
            testedObj.incrementCounter();
            Assert.IsTrue(testedObj.getCounter() == 1);
        }
    }
}
