using System;

namespace mRides_app.MessagingService
{
    internal class MessageContent
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }

        public MessageContent() { }
        public MessageContent(string Name, string Message)
        {
            this.Name = Name;
            this.Message = Message;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}