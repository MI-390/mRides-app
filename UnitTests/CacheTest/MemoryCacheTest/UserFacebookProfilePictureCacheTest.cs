using mRides_app.Cache.MemoryCache;
using NUnit.Framework;
using System.Reflection;
using System.Collections.Generic;
using System;
using Android.Graphics;
using System.Net;

namespace UnitTests
{
    [TestFixture]
    public class UserFacebookProfilePictureCacheTest
    {
        [SetUp]
        public void Setup()
        {
            UserFacebookProfilePictureCache userFbPicCache = UserFacebookProfilePictureCache.GetInstance();

            // Clear the cache before each test using reflection since the cache is a singleton
            Dictionary<long, Bitmap> cache = GetObjectField<Dictionary<long, Bitmap>>(userFbPicCache, "lruCache");
            cache.Clear();

            // Clear the list of recently accessed
            List<long> listRecentlyAccessed = GetObjectField<List<long>>(userFbPicCache, "listUserFacebookIdRecentlyAccessed");
            listRecentlyAccessed.Clear();

            // Reset the current size to 0
            SetObjectField(userFbPicCache, "cacheCurrentSize", 0);
        }


        [TearDown]
        public void Tear()
        {

        }

        // -------------------------------------------------------------------------------------------------------------
        // UNIT TESTS
        // -------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Basic scenario in which we ensure that a picture is added correctly (associated with its facebook ID) 
        /// to the cache, and that the facebook ID is found on the list of recently accessed item in the cache list.
        /// </summary>
        [Test]
        public void AddUserFacebookProfilePicture()
        {
            long facebookId = 11111;

            // Get the profile picture
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData("http://graph.facebook.com/" + facebookId + "/picture?type=large");
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            
            // Add it to the cache
            UserFacebookProfilePictureCache userFbPicCache = UserFacebookProfilePictureCache.GetInstance();
            userFbPicCache.AddUserFacebookProfilePicture(11111, imageBitmap);

            // Ensure that it is present in the cache and correctly associated with the facebook id
            Dictionary<long, Bitmap> cache = GetObjectField<Dictionary<long, Bitmap>>(userFbPicCache, "lruCache");
            Bitmap cacheImageBitmap;
            cache.TryGetValue(facebookId, out cacheImageBitmap);
            Assert.True(cacheImageBitmap != null && cacheImageBitmap.Equals(imageBitmap));

            // Ensure that the facebook id is also present on the list of recently accessed
            List<long> listRecentlyAccessed = GetObjectField<List<long>>(userFbPicCache, "listUserFacebookIdRecentlyAccessed");
            Assert.True(listRecentlyAccessed.Contains(facebookId));
        }

        /// <summary>
        /// Basic scenario in which we ensure that we are able to retrieve back an item from the cache, given
        /// that it is present.
        /// </summary>
        [Test]
        public void FindUserFacebookProfilePicture()
        {
            long facebookId = 11111;

            // Get the profile picture
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData("http://graph.facebook.com/" + facebookId + "/picture?type=large");
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            // Add it to the cache using reflection (this is to make this test independent of the add method)
            UserFacebookProfilePictureCache userFbPicCache = UserFacebookProfilePictureCache.GetInstance();
            Dictionary<long, Bitmap> cache = GetObjectField<Dictionary<long, Bitmap>>(userFbPicCache, "lruCache");
            cache.Add(facebookId, imageBitmap);

            // Also add it to the list of recently accessed
            List<long> listRecentlyAccessed = GetObjectField<List<long>>(userFbPicCache, "listUserFacebookIdRecentlyAccessed");
            listRecentlyAccessed.Add(facebookId);

            // Check what we can find from the cache
            Bitmap bitmapFound = userFbPicCache.FindUserFacebookProfilePicture(facebookId);
            Assert.True(bitmapFound != null && bitmapFound.Equals(imageBitmap));
        }

        /// <summary>
        /// Ensure that an element of the cache can be removed correctly given the facebook Id
        /// </summary>
        [Test]
        public void RemoveUserFacebookProfilePictureFromCache()
        {
            long facebookId = 11111;

            // Get the profile picture
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData("http://graph.facebook.com/" + facebookId + "/picture?type=large");
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            // Add it to the cache using reflection (this is to make this test independent of the add method)
            UserFacebookProfilePictureCache userFbPicCache = UserFacebookProfilePictureCache.GetInstance();
            Dictionary<long, Bitmap> cache = GetObjectField<Dictionary<long, Bitmap>>(userFbPicCache, "lruCache");
            cache.Add(facebookId, imageBitmap);

            // Also add it to the list of recently accessed
            List<long> listRecentlyAccessed = GetObjectField<List<long>>(userFbPicCache, "listUserFacebookIdRecentlyAccessed");
            listRecentlyAccessed.Add(facebookId);

            // Remove it
            userFbPicCache.RemoveUserFacebookProfilePictureFromCache(facebookId);

            // Ensure that it is no longer in the cache and in the list
            Assert.True(!cache.ContainsKey(facebookId)
                && !cache.ContainsValue(imageBitmap)
                && !listRecentlyAccessed.Contains(facebookId));
        }



        // -------------------------------------------------------------------------------------------------------------
        // INTEGRATION TESTS
        // -------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Ensure that whenever an item is accessed on the cache, the element is moved to the top of 
        /// the recently accessed list (at index 0)
        /// </summary>
        [Test]
        public void RecentlyAccessedList()
        {
            long facebookId1 = 11111;
            long facebookId2 = 4;

            // Get the profile pictures
            Bitmap imageBitmap1 = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData("http://graph.facebook.com/" + facebookId1 + "/picture?type=large");
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap1 = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            Bitmap imageBitmap2 = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData("http://graph.facebook.com/" + facebookId2 + "/picture?type=large");
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap2 = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            // Add both to the cache, user 1 first then user 2
            UserFacebookProfilePictureCache userFbPicCache = UserFacebookProfilePictureCache.GetInstance();
            userFbPicCache.AddUserFacebookProfilePicture(facebookId1, imageBitmap1);
            userFbPicCache.AddUserFacebookProfilePicture(facebookId2, imageBitmap2);

            // Obtain private members of the cache
            Dictionary<long, Bitmap> cache = GetObjectField<Dictionary<long, Bitmap>>(userFbPicCache, "lruCache");
            List<long> listRecentlyAccessed = GetObjectField<List<long>>(userFbPicCache, "listUserFacebookIdRecentlyAccessed");

            // Ensure that the first is now at the end of the list and the second at the beginning
            Assert.True(listRecentlyAccessed[0] == facebookId2 && listRecentlyAccessed[1] == facebookId1);

            // Access the first facebook id picture
            userFbPicCache.FindUserFacebookProfilePicture(facebookId1);

            // Ensure that the first is now on the beginning and the second at the endo f the list
            Assert.True(listRecentlyAccessed[0] == facebookId1 && listRecentlyAccessed[1] == facebookId2);
        }

        /// <summary>
        /// Ensure that whenever the cache is full, and a new picture needs to be added, 
        /// the correct element (element that is not accessed for the longest time) is 
        /// removed, and the new element is added, being on the top of the recently 
        /// accessed list.
        /// </summary>
        [Test]
        public void AddUserFacebookProfilePictureWhenCacheFull()
        {
            // Obtain all private members of the cache that need to be monitored for this test
            UserFacebookProfilePictureCache userFbPicCache = UserFacebookProfilePictureCache.GetInstance();
            Dictionary<long, Bitmap> cache = GetObjectField< Dictionary<long, Bitmap>>(userFbPicCache, "lruCache");
            List<long> listRecentlyAccessed = GetObjectField<List<long>>(userFbPicCache, "listUserFacebookIdRecentlyAccessed");
            int cacheMaxSize = UserFacebookProfilePictureCache.cacheMaxSize;
            int currentCacheSizeReal = GetObjectField<int>(userFbPicCache, "cacheCurrentSize");
            // Fill up the cache with mock bitmaps
            int currentCacheSize = 0;
            long facebookIdCounter = 1;
            while(true)
            {
                // Make a mock bitmap of size 200x200 and configuration Argb8888
                // just like the one we get from facebook
                Bitmap mock = Bitmap.CreateBitmap(200, 200, Bitmap.Config.Argb8888);
                int byteCount = mock.ByteCount;

                // If adding this items results in reaching the max capacity of the cache, stop adding mocks
                if(currentCacheSize + byteCount > cacheMaxSize)
                {
                    break;
                }

                // Otherwise, add the mock image and update the byte count 
                userFbPicCache.AddUserFacebookProfilePicture(facebookIdCounter, mock);
                currentCacheSize += byteCount;

                // Next facebookid 
                facebookIdCounter++;
            }

            // The next image we will add will cause the one of the image from the cache to be removed,
            // since no one accessed any of them, the first image (facebookId = 1) should be the one 
            // being removed. Ensure that this one is found at the end of the list and still present
            // in the cache
            int numberElementsInCache = cache.Count;
            int numberElementsInList = listRecentlyAccessed.Count;
            Assert.True(numberElementsInCache == numberElementsInList
                && listRecentlyAccessed[numberElementsInList - 1] == 1
                && cache.ContainsKey(1));

            // Add the new image
            userFbPicCache.AddUserFacebookProfilePicture(facebookIdCounter, Bitmap.CreateBitmap(200, 200, Bitmap.Config.Argb8888));

            // Now the first one should have been removed from the cache
            Assert.True(!listRecentlyAccessed.Contains(1)
                && !cache.ContainsKey(1));

            // The last one in the list is now 2
            Assert.True(listRecentlyAccessed[numberElementsInList - 1] == 2);

            // The most recently accessed is the new element that was added, and it is present in the cache.
            Assert.True(listRecentlyAccessed[0] == facebookIdCounter
                && cache.ContainsKey(facebookIdCounter));
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
            return (T) fieldInfo.GetValue(instance);
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
    }
}