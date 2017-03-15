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

namespace mRides_app
{

    /// <summary>
    /// Implementation of the chat activity
    /// </summary>
    [Activity(Label = "ChatActivity", Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ChatActivity : AppCompatActivity, IValueEventListener
    {
        private FirebaseClient firebase;
        private List<MessagingService.MessageContent> listMessage = new List<MessagingService.MessageContent>();
        private ListView listChat;
        private EditText editChat;
        private Button sendButton;
        string chatName;

        private String userName = Models.User.currentUser.firstName;

        public int MyResultCode = 1;

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Chat);
            chatName = Intent.GetStringExtra("ChatName");
            firebase = new FirebaseClient(GetString(Resource.String.firebase_database_url));
            // adding listener to "chats" everytime this activity is run
            FirebaseDatabase.Instance.GetReference(chatName).AddValueEventListener(this);

            sendButton = FindViewById<Button>(Resource.Id.sendMsgButton);
            editChat = FindViewById<EditText>(Resource.Id.chatMsg);
            listChat = FindViewById<ListView>(Resource.Id.list_of_messages);

            DisplayChatMessage();
            Log.Debug("CHAT", "InstanceID token: " + FirebaseInstanceId.Instance.Token);
            sendButton.Click += delegate
            {
                PostMessage();
            };
        }

        /// <summary>
        /// Method to send a message to the chat interface
        /// ***We will have to get the user's name and replace it 
        /// </summary>
        private async void PostMessage()
        {
            // Post a message to "chats" specifically by creating a new MessageContent which takes a username and a text as parameters
            var items = await firebase.Child(chatName).PostAsync(new MessagingService.MessageContent(userName, editChat.Text));
            editChat.Text = ""; // empty the text field
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        /// <summary>
        /// Method that will display the message after a user posts a message
        /// </summary>
        /// <param name="snapshot"></param>
        public void OnDataChange(DataSnapshot snapshot)
        {
            DisplayChatMessage();
        }

        /// <summary>
        /// Method to display the message on the interface
        /// </summary>
        private async void DisplayChatMessage()
        {
            listMessage.Clear();
            var items = await firebase.Child(chatName)
                .OnceAsync<MessagingService.MessageContent>();

            foreach (var ImageButton in items)
                listMessage.Add(ImageButton.Object);

            // create a new adapter that takes the list of messages
            MessagingService.ListViewAdapter adapter = new MessagingService.ListViewAdapter(this, listMessage);
            listChat.Adapter = adapter;
        }
    }
}