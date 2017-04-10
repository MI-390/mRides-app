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

namespace mRides_app.Models
{
    /// <summary>
    /// Class that corresponds to the users engaged in a chat.
    /// </summary>
    class ChatList
    {
        public string ChatName {get;set;}
        public User user1 { get; set; }
        public User user2 { get; set; }
    }
}