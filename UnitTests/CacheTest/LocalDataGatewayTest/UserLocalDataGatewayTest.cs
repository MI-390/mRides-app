using mRides_app.Cache.MemoryCache;
using NUnit.Framework;
using System.Reflection;
using System.Collections.Generic;
using System;
using Android.Graphics;
using System.Net;
using mRides_app.Cache.LocalDatabase;
using mRides_app.Cache.LocalDataGateways;
using SQLite;
using mRides_app.Models;

namespace UnitTests
{
    /// <summary>
    /// Testing class for the User Local DataGateway
    /// </summary>
    [TestFixture]
    public class UserLocalDataGatewayTest
    {
        /// <summary>
        /// Set up method for testing
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Before each test, clear the database
            var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var path = System.IO.Path.Combine(docsFolder, MRidesLocalDatabase.MRIDES_LOCAL_DB_FILENAME);
            System.IO.File.Delete(path);

            // Clear singleton
            SetStaticField(typeof(MRidesLocalDatabase), "instance", null);
            SetStaticField(typeof(UserLocalDataGateway), "instance", null);
        }


        [TearDown]
        public void Tear()
        {

        }

        // -------------------------------------------------------------------------------------------------------------
        // UNIT TESTS
        // -------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Test method for adding a user
        /// </summary>
        [Test]
        public void AddUser()
        {
            // Id of the user used for testing
            int userId = 12345;
            long facebookId = 67890;
            
            // Create a mock user to be stored in the database
            User user = new User
            {
                id = userId,
                facebookID = facebookId,
                isHandicap = false,
                isSmoker = false,
                hasLuggage = true,
                hasPet = true
            };

            // Add the user
            MRidesLocalDatabase localDB = MRidesLocalDatabase.GetInstance();
            UserLocalDataGateway userLocalGateway = UserLocalDataGateway.GetInstance();
            bool addedSuccessfully = userLocalGateway.AddUpdateUser(user);
            Assert.True(addedSuccessfully);

            // Verify that the user has been added
            try
            {
                var db = new SQLiteConnection(localDB.pathToLocalDatabase);
                User foundUser = db.Find<User>(userId);
                Assert.True(foundUser != null
                    && foundUser.id == userId
                    && foundUser.facebookID == facebookId
                    && !foundUser.isHandicap
                    && !foundUser.isSmoker
                    && foundUser.hasLuggage
                    && foundUser.hasPet);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                Assert.True(false);
            }
        }

        /// <summary>
        /// Test method for updating a user
        /// </summary>
        [Test]
        public void UpdateUser()
        {
            // Id of the user used for testing
            int userId = 12345;
            long facebookId = 567890;
            // Create a mock user to be stored in the database
            User user = new User
            {
                id = userId,
                facebookID = facebookId,
                isHandicap = false,
                isSmoker = false,
                hasLuggage = true,
                hasPet = true
            };

            // Obtain the db and gateway
            MRidesLocalDatabase localDB = MRidesLocalDatabase.GetInstance();
            UserLocalDataGateway userLocalGateway = UserLocalDataGateway.GetInstance();

            // Add it to the database manually
            try
            {
                var dbForInsert = new SQLiteConnection(localDB.pathToLocalDatabase);
                int result = dbForInsert.Insert(user);
                Assert.True(result == 1);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                Assert.True(false);
            }
            
            // Update the user
            user.hasPet = false;
            user.hasLuggage = false;
            user.facebookID = 54321;
            bool updatedSuccessfully = userLocalGateway.AddUpdateUser(user);

            // Verify that the user has been added
            var db = new SQLiteConnection(localDB.pathToLocalDatabase);
            User foundUser = db.Find<User>(userId);

            Assert.True(foundUser != null
                && foundUser.id == userId
                && foundUser.facebookID == 54321
                && !foundUser.isHandicap
                && !foundUser.isSmoker
                && !foundUser.hasLuggage
                && !foundUser.hasPet);
        }

        /// <summary>
        /// Test method for finding a user by userId
        /// </summary>
        [Test]
        public void FindUserById()
        {
            // Id of the user used for testing
            int userId = 12345;
            long facebookId = 567890;
            // Create a mock user to be stored in the database
            User user = new User
            {
                id = userId,
                facebookID = facebookId,
                isHandicap = false,
                isSmoker = true,
                hasLuggage = true,
                hasPet = true
            };

            // Obtain the db and gateway
            MRidesLocalDatabase localDB = MRidesLocalDatabase.GetInstance();
            UserLocalDataGateway userLocalGateway = UserLocalDataGateway.GetInstance();

            // Add it to the database manually
            try
            {
                var dbForInsert = new SQLiteConnection(localDB.pathToLocalDatabase);
                int result = dbForInsert.Insert(user);
                Assert.True(result == 1);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                Assert.True(false);
            }

            // Find the user
            User foundUser = userLocalGateway.FindUserById(userId);
            Assert.True(foundUser != null
                && foundUser.id == userId
                && foundUser.facebookID == facebookId
                && !foundUser.isHandicap
                && foundUser.isSmoker
                && foundUser.hasLuggage
                && foundUser.hasPet);
        }

        /// <summary>
        /// Test method for deleting a user
        /// </summary>
        [Test]
        public void DeleteUser()
        {
            // Id of the user used for testing
            int userId = 12345;
            long facebookId = 567890;

            // Create a mock user to be stored in the database
            User user = new User
            {
                id = userId,
                facebookID = facebookId,
                isHandicap = false,
                isSmoker = false,
                hasLuggage = true,
                hasPet = true
            };

            // Obtain the db and gateway
            MRidesLocalDatabase localDB = MRidesLocalDatabase.GetInstance();
            UserLocalDataGateway userLocalGateway = UserLocalDataGateway.GetInstance();

            // Add it to the database manually
            try
            {
                var dbForInsert = new SQLiteConnection(localDB.pathToLocalDatabase);
                int result = dbForInsert.Insert(user);
                Assert.True(result == 1);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                Assert.True(false);
            }

            // Delete the user
            bool deleteSuccess = userLocalGateway.DeleteUser(userId);
            Assert.True(deleteSuccess);

            // Ensure it was deleted
            try
            {
                var db = new SQLiteConnection(localDB.pathToLocalDatabase);
                User foundUser = db.Find<User>(userId);
                Assert.True(foundUser == null);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                Assert.True(false);
            }
        }




        // -------------------------------------------------------------------------------------------------------------
        // SUPPORTIVE METHODS TO PERFORM TESTS
        // -------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Supportive method for testing to access private variables of an object.
        /// </summary>
        /// <param name="instance">Instance of the class (object) for which we want to access the private member.</param>
        /// <param name="fieldName">Name of the member to be accessed.</param>
        /// <returns>Object representing the field, to be casted</returns>
        private T GetObjectField<T>(object instance, String fieldName)
        {
            Type type = instance.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return (T)fieldInfo.GetValue(instance);
        }

        /// <summary>
        /// Supportive method for testing to update/set private variables of an object.
        /// </summary>
        /// <param name="instance">Instance of the class (object) for which we want to access the private member</param>
        /// <param name="fieldName">Name of the member to be set</param>
        /// <param name="value">Value of the field to be set</param>
        private void SetObjectField(object instance, string fieldName, object value)
        {
            Type type = instance.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            fieldInfo.SetValue(instance, value);
        }

        /// <summary>
        /// Supportive mthod for testing to update/set private static fields of an object
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="fieldName">Name of the static field to be set</param>
        /// <param name="value">Value to which the field should be set</param>
        private void SetStaticField(Type type, string fieldName, object value)
        {
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            fieldInfo.SetValue(null, value);
        }
    }
}