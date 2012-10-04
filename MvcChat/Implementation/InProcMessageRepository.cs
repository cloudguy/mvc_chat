using System;
using System.Collections.Generic;
using System.Linq;
using MvcChat.Domain;
using MvcChat.Interfaces;

namespace MvcChat.Implementation
{
    public class InProcMessageRepository : IMessageRepository
    {
        private static readonly object Sync = new object();
        private static readonly List<ChatMessage> Messages = new List<ChatMessage>();

        public ChatMessage Insert(ChatMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            lock(Sync)
            {
                var lastMessage = Messages.LastOrDefault();
                message.Id = lastMessage == null ? 1 : lastMessage.Id + 1;
                //saving memory
                if (Configuration.Instance.MaxChatMessages < Messages.Count)
                    Messages.RemoveAt(0);
                Messages.Add(message);
            }
            return message;
        }

        public IEnumerable<ChatMessage> GetLatest(int maxCount)
        {
            if (maxCount <= 0)
                throw new ArgumentException("maxCount");
            lock(Sync)
            {
                int skip = Messages.Count - maxCount;
                if (skip<0) skip = 0;
                return Messages.Skip(skip).Select(x => x).ToArray()/*override lazy linq*/;
            }
        }

        public IEnumerable<ChatMessage> GetLatestAfterId(int lastId)
        {
            lock (Sync)
            {
                return Messages.Where(m=>m.Id>lastId).Select(x=>x).ToArray()/*override lazy linq*/;
            }
        }
    }
}