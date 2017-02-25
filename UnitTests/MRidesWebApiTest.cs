using mRides_app;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class MRidesWebApiTest
    {

        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }


        /**
         * Test whether sending a POST request to the server to create a user is successful
         * and a response is received. This does not test the content of the response.
         */
        [Test]
        public void CreateUser()
        {
            User newUser = new User();
            newUser.gsd = 5012;
            newUser.hasLuggage = true;
            User response = MRidesWebApi.CreateUser(newUser);
            Assert.True(response.Equals(newUser));
        }

        /**
         * Test whether we are able to successfully send a request to the server to
         * obtain information about a user. For now assume that there is a user in the database.
         */
        [Test]
        public void GetUser()
        {
            User response = MRidesWebApi.GetUser(1);
            Assert.True(response.id == 1);
        }

        //[Test]
        //public void CreateRequest()
        //{
        //    MRidesWebApi mRidesWebApi = new MRidesWebApi();
        //    object newRequest = new
        //    {
        //        destination = "Sample destination",
        //        location = "Sample location",
        //        dateTime = DateTime.Now,
        //        isWeekly = true
        //    };
        //    object response = mRidesWebApi.SendPost<object>(MRidesWebApi.ApiEndPointUrl.createRequest, newRequest);
        //    Assert.True(response != null);
        //}

        //[Test]
        //public void DeleteRequest()
        //{
        //    MRidesWebApi mRidesWebApi = new MRidesWebApi();
        //    object response = mRidesWebApi.SendDeleteWithUrlSegment<object>(MRidesWebApi.ApiEndPointUrl.deleteRequest, "id", "1");
        //    Assert.True(response != null);
        //}
    }
}