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

using SQLite;
using mRides_app.Models;

namespace mRides_app.Cache.LocalDatabase
{
    /// <summary>
    /// Implementation of a local SQLite database to store persistent information
    /// retrieved from the server, which has structured data. 
    /// Resource: https://developer.xamarin.com/recipes/android/data/databases/sqlite/
    /// </summary>
    public class MRidesLocalDatabase
    {
        // -------------------------------------------------------------------------
        // Constants
        // -------------------------------------------------------------------------

        // Filename of the mRides local database
        public const string MRIDES_LOCAL_DB_FILENAME = "MRidesLocalDatabase.db";


        // -------------------------------------------------------------------------
        // Static variables
        // -------------------------------------------------------------------------

        // Singleton instance of the local database
        private static MRidesLocalDatabase instance;

        
        // -------------------------------------------------------------------------
        // Instance variables
        // -------------------------------------------------------------------------

        // Path to access the local database
        public string pathToLocalDatabase { get; }



        /// <summary>
        /// Singleton getter
        /// </summary>
        /// <returns>Singleton instance of the mRides local database</returns>
        public static MRidesLocalDatabase GetInstance()
        {
            if (instance == null)
            {
                instance = new MRidesLocalDatabase();
            }
            return instance;
        }

        /// <summary>
        /// Private constructor since this is a singleton
        /// </summary>
        private MRidesLocalDatabase()
        {
            // Initialize the local database path
            var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            this.pathToLocalDatabase = System.IO.Path.Combine(docsFolder, MRIDES_LOCAL_DB_FILENAME);
            this.CreateDatabase();
        }

        /// <summary>
        /// Create the database tables if they do not exist already.
        /// </summary>
        /// <param name="path">Path to the location of the database</param>
        /// <returns>True if the operation is successful, false otherwise</returns>
        private bool CreateDatabase()
        {
            try
            {
                // Open a connection
                var connection = new SQLiteConnection(this.pathToLocalDatabase);

                // Create the tables (if they do not exist)
                connection.CreateTable<User>();
                
                // Success, return successful message
                return true;
            }
            catch (SQLiteException e)
            {
                return false;
            }
        }
    }
}