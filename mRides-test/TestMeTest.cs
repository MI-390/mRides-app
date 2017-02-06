
using NUnit.Framework;
using mRides_app;

namespace mRides_test
{
    [TestFixture]
    public class TestMeTest
    {

        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        [Test]
        public void TestMe_OneToFive()
        {
            int num = TestMe.OneToFive();
            Assert.True(num >= 1 && num <= 5);
        }

        [Test]
        public void TestMe_Counter()
        {
            TestMe testedObj = new TestMe(1);
            testedObj.incrementCounter();

            Assert.True(testedObj.getCounter() == 1);

        }
    }
}