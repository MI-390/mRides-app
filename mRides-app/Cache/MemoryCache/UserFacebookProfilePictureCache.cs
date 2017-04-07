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
using Android.Graphics;

namespace mRides_app.Cache.MemoryCache
{
    /// <summary>
    /// This imitates the android LRU cache which stores a limited number of bitmaps 
    /// objects depending on the maximum size of the cache.
    /// </summary>
    public class UserFacebookProfilePictureCache
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        // Maximum size of the cache
        private const int cacheMaxSize = 4 * 1024 * 1024; // 4MB

        // ---------------------------------------------------------------------------
        // Static variables
        // ---------------------------------------------------------------------------

        // Singleton
        private static UserFacebookProfilePictureCache userFacebookProfilePictureCache;

        // ---------------------------------------------------------------------------
        // Instance variables
        // ---------------------------------------------------------------------------

        // Current size of the cache
        private int cacheCurrentSize;

        // Dictionary containing the cached image, this mimics the android LruCache
        private Dictionary<long, Bitmap> lruCache;

        // Lock, used when reading/writing from cache
        private object cacheLock = new object();

        // List of user ID corresponding to the profile picture, this is used to determine what
        // element is to be removed from the cache when the cache is full and a new element needs
        // to be added, the list is sorted by recently accessed. The beginning of the list index 0
        // is the most recently accessed cached element.
        private List<long> listUserFacebookIdRecentlyAccessed;



        /// <summary>
        /// Singleton getter
        /// </summary>
        /// <returns>Singleton instance of the UserFacebookProfilePictureCache</returns>
        public static UserFacebookProfilePictureCache GetInstance()
        {
            if(userFacebookProfilePictureCache == null )
            {
                userFacebookProfilePictureCache = new UserFacebookProfilePictureCache();
            }
            return userFacebookProfilePictureCache;
        }

        /// <summary>
        /// Private constructor, only used to instantiate the singleton.
        /// </summary>
        private UserFacebookProfilePictureCache ()
        {
            this.cacheCurrentSize = 0;
            this.lruCache = new Dictionary<long, Bitmap>();
            this.listUserFacebookIdRecentlyAccessed = new List<long>();
        }

        /// <summary>
        /// Queries the cache to see if the profile picture of a user is already loaded
        /// as a Bitmap.
        /// </summary>
        /// <param name="userFacebookId">ID of the user for which the profile picture should be looked for</param>
        /// <returns>Bitmap representing the picture of the user, or null if not found</returns>
        public Bitmap FindUserFacebookProfilePicture(long userFacebookId)
        {
            Bitmap bitmap = null;
            lock(this.cacheLock)
            {
                if (this.lruCache.ContainsKey(userFacebookId))
                {
                    this.lruCache.TryGetValue(userFacebookId, out bitmap);
                    if (bitmap != null)
                    {
                        this.AccessElement(userFacebookId);
                    }
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Add a new facebook profile picture of a user to the cache. The picture will not
        /// get added if it exceeds the maximum size of the cache.
        /// </summary>
        /// <param name="userFacebookId">ID of the user associated with the picture</param>
        /// <param name="bitmap">Bitmap representing the profile picture of the user</param>
        public void AddUserFacebookProfilePicture(long userFacebookId, Bitmap bitmap)
        {
            // Exceeds the max size, return
            if(bitmap.ByteCount >= cacheMaxSize)
            {
                return;
            }

            // We need to remove elements from the cache to store the new one
            // while there is not enough space.
            lock(this.cacheLock)
            {
                while (this.cacheCurrentSize + bitmap.ByteCount > cacheMaxSize)
                {
                    long userFacebookIdToRemove = this.listUserFacebookIdRecentlyAccessed.Last();
                    this.lruCache.Remove(userFacebookIdToRemove);
                    this.listUserFacebookIdRecentlyAccessed.Remove(userFacebookIdToRemove);
                }
            }

            // Add or update the image to the cache
            this.RemoveUserFacebookProfilePictureFromCache(userFacebookId);
            lock(this.cacheLock)
            {
                this.lruCache.Add(userFacebookId, bitmap);
            }

            // Update list
            this.AccessElement(userFacebookId);
        }

        /// <summary>
        /// Removes the bitmap associated with a user ID from the cache, also removing it from the list
        /// of recently accessed items
        /// </summary>
        /// <param name="userFacebookId">ID of the user associated with the bitmap to be removed</param>
        public void RemoveUserFacebookProfilePictureFromCache(long userFacebookId)
        {
            lock(this.cacheLock)
            {
                if (this.lruCache.ContainsKey(userFacebookId))
                {
                    this.lruCache.Remove(userFacebookId);
                }

                if(this.listUserFacebookIdRecentlyAccessed.Contains(userFacebookId))
                {
                    this.listUserFacebookIdRecentlyAccessed.Remove(userFacebookId);
                }
            }
        }

        /// <summary>
        /// Supportive method that updates the list of recently accessed
        /// cached element. Moves or add the user id to the top of the 
        /// list
        /// </summary>
        /// <param name="userFacebookId">ID of the user representing the key of an accessed element</param>
        private void AccessElement(long userFacebookId)
        {
            lock(this.cacheLock)
            {
                if (this.listUserFacebookIdRecentlyAccessed.Contains(userFacebookId))
                {
                    this.listUserFacebookIdRecentlyAccessed.Remove(userFacebookId);
                }
                this.listUserFacebookIdRecentlyAccessed.Insert(0, userFacebookId);
            }
        }
    }
}