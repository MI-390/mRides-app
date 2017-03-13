using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;
using mRides_app;

namespace mRides_app.MessagingService
{
    /// <summary>
    /// Implementation of the Firebase Messaging Service class
    /// </summary>
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";

        /// <summary>
        /// This method is called upon receiving a message. It will log the received message into the IDE output.
        /// </summary>
        /// <param name="message"></param>
        public override void OnMessageReceived(RemoteMessage message)
        {
            //When the client app receives a message from FCM, OnMessageReceived extracts the message content from the passed-in RemoteMessage object
            //by calling its GetNotification method.
            //Message content will be shown in the IDE output window
            Log.Debug(TAG, "From: " + message.From);
            Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
            SendNotification(message.GetNotification().Body);
        }

        /// <summary>
        /// This method will run whenever a notification is received while the app is in the foreground, and the notification will appear in the notification area.
        /// If the app is in the background, then this method will not run and there will be a background notification.
        /// </summary>
        /// <param name="messageBody"></param>
        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MessagingActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            // used to create the notification
            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("FCM Message")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            // used to launch the notification
            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}