using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using mRides_app.Cache.LocalDatabase;
using mRides_app.Models;
using SQLite;
using System.Threading.Tasks;

namespace mRides_app.Cache.LocalDataGateways
{
    public class UserLocalDataGateway
    {
        // -------------------------------------------------------------------------
        // Static variables
        // -------------------------------------------------------------------------

        // Singleton instance
        private static UserLocalDataGateway instance;



        // -------------------------------------------------------------------------
        // Instance variables
        // -------------------------------------------------------------------------

        // MRides local database to be accessed. It stores the table user that this
        // class will access and modify
        private MRidesLocalDatabase mRidesLocalDB; 


        /// <summary>
        /// Private constructor
        /// </summary>
        private UserLocalDataGateway ()
        {
            this.mRidesLocalDB = MRidesLocalDatabase.GetInstance();
        }

        /// <summary>
        /// Singleton getter
        /// </summary>
        /// <returns>UserLocalDataGateway singleton instance</returns>
        public static UserLocalDataGateway GetInstance()
        {
            if(instance == null)
            {
                instance = new UserLocalDataGateway();
            }
            return instance;
        }

        /// <summary>
        /// Adds or updates the record of a user
        /// </summary>
        /// <param name="user">User for which fields need to be updated</param>
        /// <returns>bool, true if the insert/update were successful, false otherwise</returns>
        public bool AddUpdateUser(User user)
        {
            try
            {
                var db = new SQLiteConnection(this.mRidesLocalDB.pathToLocalDatabase);
                db.InsertOrReplace(user);
                return true;
            }
            catch(SQLiteException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Finds a user from the local database by it's user ID 
        /// </summary>
        /// <param name="userId">ID of the user to be found</param>
        /// <returns>User if found, null is no records were found or an exception occurred</returns>
        public User FindUserById(int userId)
        {
            try
            {
                var db = new SQLiteConnection(this.mRidesLocalDB.pathToLocalDatabase);
                var user = db.Find<User>(u => u.id == userId);
                return user;
            }
            catch (SQLiteException e)
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes a user by its ID
        /// </summary>
        /// <param name="userId">ID of the user to be deleted</param>
        /// <returns>bool: true if the operation was successful, false otherwise</returns>
        public bool DeleteUser(int userId)
        {
            try
            {
                var db = new SQLiteConnection(this.mRidesLocalDB.pathToLocalDatabase);
                int result = db.Delete<User>(userId);
                return result < 1 ? false : true;
            }
            catch (SQLiteException e)
            {
                return false;
            }
        }
    }
}