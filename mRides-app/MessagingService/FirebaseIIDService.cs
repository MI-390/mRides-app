using System;
using Android.App;
using Firebase.Iid;
using Android.Util;
using mRides_app.Mappers;
using mRides_app.Models;

namespace mRides_app.MessagingService
{
    /// <summary>
    /// Implementation of the Firebase Instance ID Service class to maintain receiver services
    /// </summary>
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "FirebaseIIDService";

        /// <summary>
        /// This method is invoked when the registration token is initially created or changed.
        /// It retrieves the latest token from the FirebaseInstanceId.Instance.Token property (which is updated asynchronously by FCM).
        /// In this example, the refreshed token is logged so that it can be viewed in the output window:
        /// </summary>
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }

        /// <summary>
        /// Method to associate the user's registration token with the server-side account
        /// </summary>
        /// <param name="token"></param>
        void SendRegistrationToServer(string token)
        {
            if (User.currentUser!=null){
                UserMapper.getInstance().updateFcmToken(token);
            }
            
        }
    }
}