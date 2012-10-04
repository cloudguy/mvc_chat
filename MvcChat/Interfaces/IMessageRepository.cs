using System.Collections.Generic;
using MvcChat.Domain;

namespace MvcChat.Interfaces
{
    public interface IMessageRepository
    {
        ChatMessage Insert(ChatMessage message);
        IEnumerable<ChatMessage> GetLatest(int maxCount);
        IEnumerable<ChatMessage> GetLatestAfterId(int lastId);
    }
}
