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
using Firebase.Iid;
using Android.Util;

namespace mRides_app.MessagingService
{
    [Service]
    public class FCMRegistrationService : IntentService
    {
        private const string Tag = "FCMRegistrationService";
        static object locker = new object();

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                lock (locker)
                {
                    var instanceId = FirebaseInstanceId.Instance;
                    var token = instanceId.Token;

                    if (string.IsNullOrEmpty(token))
                        return;

#if DEBUG
                    instanceId.DeleteToken(token, "");
                    instanceId.DeleteInstanceId();

#endif


                }
            }
            catch (Exception e)
            {
                Log.Debug(Tag, e.Message);
            }
        }
    }
}