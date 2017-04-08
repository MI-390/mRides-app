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
using mRides_app.Mappers;
using System.Threading.Tasks;

namespace mRides_app
{

    /// <summary>
    /// Implementation of the chat activity
    /// </summary>
    [Activity(Label = "ChatListActivity", Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ChatListActivity : AppCompatActivity
    {
        private FirebaseClient firebase;
        ListView listView;
        List<ChatList> chatList = new List<ChatList>();
        protected override async void OnCreate(Bundle bundle)
        {
            UserMapper.getInstance().setTheme(this);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Chats);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "mRides";

            getFirebase();

            listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
                                                                 // populate the listview with data
            listView.Adapter = new ChatListAdapter(this, chatList);
            listView.ItemClick += OnListItemClick;  // to be defined


            //Menu bottom bar
            var chatMenuButton = FindViewById<ImageButton>(Resource.Id.menu_chat);
            chatMenuButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(ChatListActivity));
                StartActivity(i);
            };

            var userProfileButton = FindViewById<ImageButton>(Resource.Id.menu_user);
            userProfileButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(UserProfileActivity));
                i.PutExtra("id", User.currentUser.id.ToString());
                StartActivity(i);
            };

            var mainMenuButton = FindViewById<ImageButton>(Resource.Id.menu_home);
            mainMenuButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(MainMenuActivity));
                StartActivity(i);
            };

        }
        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var cl = chatList[e.Position];
            string userID="";
            Intent i = new Intent(this, typeof(ChatActivity));
            i.PutExtra("ChatName", cl.ChatName);
            userID = cl.user1.id.ToString();
            if (User.currentUser.id == cl.user1.id)
                userID = cl.user2.id.ToString();
        

            i.PutExtra("id", userID);
            StartActivity(i);
            //Android.Widget.Toast.MakeText(this, t.Heading, Android.Widget.ToastLength.Short).Show();
        }
        public async Task<int> getFirebase()
        {
            firebase = new FirebaseClient(GetString(Resource.String.firebase_database_url));
            //chatList.Clear();
            var allObjects = firebase.Child("").OnceAsync<ChatList>();
            var ss = firebase.Child("3 & 8").OnceAsync<object>();
            var f = 2;
            foreach(dynamic allObject in allObjects.Result)
            {
                if (allObject.Object.user1.id == User.currentUser.id || allObject.Object.user2.id==User.currentUser.id)
                {
                    ChatList cl = allObject.Object;
                    cl.ChatName = allObject.Key;
                    chatList.Add(cl);
                }

            }
            return 2;
           
            
        }
    }
}