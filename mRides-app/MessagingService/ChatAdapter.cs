using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Android.Content;

namespace mRides_app.MessagingService
{
    /// <summary>
    /// Implementation of the Chat Adapter adapter class
    /// </summary>
    internal class ChatAdapter : BaseAdapter
    {
        private List<MessageContent> lstMessage;
        private ChatActivity chatActivity;

        /// <summary>
        /// Constructor of ChatAdapter
        /// </summary>
        /// <param name="chatActivity">An instance of chatActivity</param>
        /// <param name="lstMessage">A list of MessageContent</param>
        public ChatAdapter(ChatActivity chatActivity, List<MessageContent> lstMessage)
        {
            this.chatActivity = chatActivity;
            this.lstMessage = lstMessage;
        }

        /// <summary>
        /// Method to get the number of MessageContent in the list
        /// </summary>
        public override int Count
        {
            get
            {
                return lstMessage.Count;
            }
        }

        /// <summary>
        /// Method to get an item given its position in the list.
        /// </summary>
        /// <param name="position">Position of item in list</param>
        /// <returns>Position of item</returns>
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        /// <summary>
        /// Method to get ID of the item.
        /// </summary>
        /// <returns>ID of item</returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Method that is returns the view of the adapter
        /// </summary>
        /// <param name="position">Position of the view</param>
        /// <param name="convertView">A View object</param>
        /// <param name="parent">The parent View object</param>
        /// <returns>View object</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)chatActivity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = convertView;
            if (itemView == null) // no view to re-use, create new
                itemView = inflater.Inflate(Resource.Layout.List_Item, null);

            TextView message_user, message_time, message_content;
            message_user = itemView.FindViewById<TextView>(Resource.Id.message_user);
            message_content = itemView.FindViewById<TextView>(Resource.Id.message_text);
            message_time = itemView.FindViewById<TextView>(Resource.Id.message_time);

            message_user.Text = lstMessage[position].Name;
            message_time.Text = lstMessage[position].Time;
            message_content.Text = lstMessage[position].Message;

            return itemView;
        }
    }
}