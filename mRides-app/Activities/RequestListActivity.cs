using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using System.Collections.Generic;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Util;

using mRides_app.Models;
using mRides_app.Mappers;
using System.Threading.Tasks;

namespace mRides_app
{

    /// <summary>
    /// Implementation of the chat activity
    /// </summary>
    [Activity(Label = "RequestListActivity", Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class RequestListActivity : AppCompatActivity
    {
      
        ListView listView;
        List<Request> requestList = new List<Request>();
        protected override async void OnCreate(Bundle bundle)
        {
            UserMapper.getInstance().setTheme(this);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Chats);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "mRides";

            getRequests();

            listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
                                                                 // populate the listview with data
            listView.Adapter = new RequestListAdapter(this, requestList);
            listView.ItemClick += OnListItemClick;  // to be defined


            //Menu bottom bar
            var chatMenuButton = FindViewById<ImageButton>(Resource.Id.menu_chat);
            chatMenuButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(RequestListActivity));
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
            var cl = requestList[e.Position];
            string userID = "";
            Intent i = new Intent(this, typeof(UserCancelRideActivity));
            i.PutExtra("id", cl.ID.ToString());
            StartActivity(i);
            
        }
        public void getRequests()
        {
            requestList = UserMapper.getInstance().GetRequests(User.currentUser.id);

        }
    }
}