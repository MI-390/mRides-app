using mRides_app.Mappers;
using mRides_app.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture]
    public class RequestMapperTests
    {
        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        // -------------------------------------------------------------------
        // REQUEST RELATED TESTS
        // -------------------------------------------------------------------

        [Test]
        public void CreateRequest()
        {
            User user = new User();
            user = UserMapper.getInstance().CreateUser(user);
            User.currentUser = user;

            Request request = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_RIDER);
            //Request response = MRidesWebApi.CreateRequest(request);

            //Assert.True(request.location.Equals(response.location));
            Assert.True(false); // The server needs to return the newly created request, or at least the id of it
        }
    }
}