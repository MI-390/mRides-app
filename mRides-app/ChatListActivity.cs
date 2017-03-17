using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Firebase.Database;
using Firebase.Xamarin.Database;
using System.Collections.Generic;
using System;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Gms.Common; // For FCM
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using Firebase.Xamarin.Database.Query;
using mRides_app.Models;

namespace mRides_app
{

    /// <summary>
    /// Implementation of the chat activity
    /// </summary>
    [Activity(Label = "ChatListActivity", MainLauncher =true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ChatListActivity : AppCompatActivity
    {
        private FirebaseClient firebase;
        ListView listView;
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Chats);
            //listView.Adapter = new ChatAdapter(this,);
            //Firebase get chats
            getFirebase();
            
        }
        public async void getFirebase()
        {
            firebase = new FirebaseClient(GetString(Resource.String.firebase_database_url));
            var query = await firebase.Child("mrides-155403").OrderBy("Time").StartAt(2).OnceAsync<MessagingService.MessageContent>();
        }
    }
}