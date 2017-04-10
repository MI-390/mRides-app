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
using mRides_app.Models;
using mRides_app.Mappers;
using System.Threading.Tasks;
using Android.Views.InputMethods;

namespace mRides_app
{

    /// <summary>
    /// Activity class for the chat messaging system
    /// </summary>
    [Activity(Label = "ChatActivity", Icon = "@drawable/icon")]
    public class ChatActivity : Activity, IValueEventListener
    {
        private FirebaseClient firebase;
        private List<MessagingService.MessageContent> listMessage = new List<MessagingService.MessageContent>();
        private ListView listChat;
        private EditText editChat;
        private Button sendButton;
        int ctr = 0;
        string chatName;
        int userId;

        private String userName = Models.User.currentUser.firstName+Models.User.currentUser.lastName;

        public int MyResultCode = 1;

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Method that is invoked when activity is first created
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            UserMapper.getInstance().setTheme(this);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Chat);
            chatName = Intent.GetStringExtra("ChatName");
            userId = Convert.ToInt32(Intent.GetStringExtra("id"));
            firebase = new FirebaseClient(GetString(Resource.String.firebase_database_url));

            // adding listener to the right chat name everytime this activity is run;
            FirebaseDatabase.Instance.GetReference(chatName+"/messages").AddValueEventListener(this);

            sendButton = FindViewById<Button>(Resource.Id.sendMsgButton);
            editChat = FindViewById<EditText>(Resource.Id.chatMsg);
            listChat = FindViewById<ListView>(Resource.Id.list_of_messages);

            createMetaFields();

            DisplayChatMessage();

            Log.Debug("CHAT", "InstanceID token: " + FirebaseInstanceId.Instance.Token);
            sendButton.Click += delegate
            {
                PostMessage();
            };
        }

       /// <summary>
       /// Method to create meta fields in the firebase real-time database to categorize chat names, users and messages
       /// </summary>
       /// <returns>A task</returns>
        private async Task<int> createMetaFields()
        {
            await firebase.Child(chatName + "/user1").PatchAsync(User.currentUser);
            User user2 = UserMapper.getInstance().GetUser(userId);
            await firebase.Child(chatName + "/user2").PatchAsync(UserMapper.getInstance().GetUser(userId));
            return 2;
        }

        /// <summary>
        /// Method to send a message to the chat interface
        /// </summary>
        private async void PostMessage()
        {
            // Post a message to the right chat name by creating a new MessageContent which takes a username and a text as parameters
            var items = await firebase.Child(chatName+"/messages").PostAsync(new MessagingService.MessageContent(userName, editChat.Text));
            editChat.Text = ""; // empty the text field
        }

        public void OnCancelled(DatabaseError error)
        {

        }


        /// <summary>
        /// Method that is invoked upon data change, such as when a user posts a message
        /// </summary>
        public async void OnDataChange(DataSnapshot snapshot)
        {
            if (ctr > 0)
            {
                DisplayChatMessage();
            }
        }

        /// <summary>
        /// Method to display all chat messages
        /// </summary>
        private async void DisplayChatMessage()
        {
            listMessage.Clear();
            var items = await firebase.Child(chatName+"/messages")
                .OnceAsync<MessagingService.MessageContent>();
            
            foreach (var item in items)
                listMessage.Add(item.Object);

            // create a new adapter that takes the list of messages
            MessagingService.ChatAdapter adapter = new MessagingService.ChatAdapter(this, listMessage);
            listChat.Adapter = adapter;
            listChat.SetSelection(adapter.Count - 1);
            ctr++;
        }
    }
}