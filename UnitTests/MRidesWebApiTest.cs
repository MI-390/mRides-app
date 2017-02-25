using mRides_app;
using mRides_app.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture]
    public class MRidesWebApiTest
    {

        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }


        // -------------------------------------------------------------------
        // USER RELATED TESTS
        // -------------------------------------------------------------------

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
            newUser.isHandicap = false;
            User response = MRidesWebApi.CreateUser(newUser);
            Assert.True(response.gsd == 5012);
            Assert.True(response.hasLuggage);
            Assert.True(!response.isHandicap);
        }

        /**
         * Test whether we can retrieve a user by its Facebook id
         */
        [Test]
        public void GetUserByFacebookId()
        {
            long facebookId = 10212089607586312;

            // Test when the database is empty
            User response = MRidesWebApi.GetUserByFacebookId(99999);
            Assert.True(response == null);

            // Create a mock user
            User newUser = new User
            {
                firstName = "Facebook",
                lastName = "User",
                gsd = 666,
                hasLuggage = true,
                isHandicap = false,
                facebookID = facebookId
            };
            MRidesWebApi.CreateUser(newUser);

            // Try to find that user
            User response2 = MRidesWebApi.GetUserByFacebookId(facebookId);
            Assert.True(response2.facebookID == facebookId);
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

        /**
         * Test whether we are able to successfully get the reviews of a user
         */
        [Test]
        public void GetReviews()
        {
            User reviewer = new User
            {
                firstName = "Reviewer"
            };
            User reviewee = new User
            {
                firstName = "Reviewee"
            };

            reviewer = MRidesWebApi.CreateUser(reviewer);
            reviewee = MRidesWebApi.CreateUser(reviewee);

            // TODO
            Assert.True(false);
        }





        // -------------------------------------------------------------------
        // CONSOLE RELATED TESTS
        // -------------------------------------------------------------------

        [Test]
        public void FindDrivers()
        {
            // Create a driver and create a request that this driver previously posted
            User driver = new User();
            User.currentUser = MRidesWebApi.CreateUser(driver);
            Request driverRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_DRIVER);
            MRidesWebApi.CreateRequest(driverRequest);

            // Create a rider and set it to be the current user
            // Make a request for this current user and try to find drivers matching this request
            // The above driver should be matched!
            User rider = new User();
            User.currentUser = MRidesWebApi.CreateUser(rider);
            Request riderRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_RIDER);
            List<Request> requests = MRidesWebApi.FindDrivers(riderRequest);

            // Ensure that FindDrivers updated the ID of the request we created
            Assert.True(riderRequest.ID != 0);

            // Check the requests list
            Assert.True(requests != null && requests.Count > 0);
        }

        [Test]
        public void FindRiders()
        {
            // Create a rider and create a request that the rider made in the past
            User.currentUser = MRidesWebApi.CreateUser(new User());
            Request riderRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_RIDER);
            MRidesWebApi.CreateRequest(riderRequest);

            // Now switch to be a driver looking for riders, having the same destination
            User.currentUser = MRidesWebApi.CreateUser(new User());
            Request driverRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_DRIVER);
            List<Request> response = MRidesWebApi.FindRiders(driverRequest);

            // Ensure that the FindRiders() updated the ID of the newly created request
            Assert.True(driverRequest.ID > 0);

            // Ensure that the response received is not null and contains the rider
            Assert.True(response != null && response.Count > 0);
        }

        [Test]
        public void Confirm()
        {
            // Create a driver
            //User.currentUser = MRidesWebApi.CreateUser(new User());
            Assert.True(false);
        }






        // -------------------------------------------------------------------
        // REQUEST RELATED TESTS
        // -------------------------------------------------------------------

        [Test]
        public void CreateRequest()
        {
            User user = new User();
            user = MRidesWebApi.CreateUser(user);
            User.currentUser = user;

            Request request = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_RIDER);
            //Request response = MRidesWebApi.CreateRequest(request);

            //Assert.True(request.location.Equals(response.location));
            Assert.True(false); // The server needs to return the newly created request, or at least the id of it
        }
    }
}