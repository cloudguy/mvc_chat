using System;

namespace MvcChat.Domain
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTimeOffset SendTime { get; set; }
    }
}