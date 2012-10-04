using System.Configuration;
using System.Globalization;

namespace MvcChat
{
    public class Configuration
    {
        private int _maxChatMessages;
        public int MaxChatMessages { get { return _maxChatMessages; } }

        private int _checkNewMessageTimeout;
        public int CheckNewMessageTimeout { get { return _checkNewMessageTimeout; } }

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                            _instance = new Configuration().Init();
                    }
                }

                return _instance;
            }
        }


        private static int ParseInt(int maxVal, int minVal, int defVal, string configKey)
        {
            string valueSt = ConfigurationManager.AppSettings[configKey];
            int value;
            if (!int.TryParse(valueSt, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                return defVal;

            if (value < minVal || value > maxVal)
                return defVal;
            return value;
        }

        private Configuration Init()
        {
            _maxChatMessages = ParseInt(100, 1, 10, "maxChatMessages");
            _checkNewMessageTimeout = ParseInt(30000, 3000, 3000, "checkNewMessageTimeout");
            return this;
        }

        private static Configuration _instance;
        public static readonly object Sync = new object();
        private Configuration() { }
    }
}