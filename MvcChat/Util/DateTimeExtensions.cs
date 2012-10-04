using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcChat.Util
{
    public static class DateTimeExtensions
    {
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static double ToUnixTime(this DateTimeOffset dateTimeOffset)
        {
            TimeSpan timeSpan = dateTimeOffset - UnixEpoch;
            return timeSpan.TotalSeconds;
        }       
    }
}

