using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Extensions;

namespace Infrastructure.Core.Logging
{
    public class DefaultLogger : ILogger
    {
        private const string Source = "Application";

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message)
        {
            EventLog.WriteEntry(Source, CreateMessage(userId, clientIp, url, method, host, message));
            return message;
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type)
        {
            EventLog.WriteEntry(Source, CreateMessage(userId, clientIp, url, method, host, message), type);
            return message;
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId)
        {
            EventLog.WriteEntry(Source, CreateMessage(userId, clientIp, url, method, host, message), type, eventId);
            return message;
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, string rawData)
        {
            return WriteLog(userId, clientIp, url, method, host, Source, CreateMessage(userId, clientIp, url, method, host, message), type, 6666, 0, rawData); //category None
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, short category)
        {
            EventLog.WriteEntry(Source, CreateMessage(userId, clientIp, url, method, host, message), type, eventId, category);
            return message;
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, string rawData)
        {
            return WriteLog(userId, clientIp, url, method, host, Source, CreateMessage(userId, clientIp, url, method, host, message), type, 6666, 0, rawData);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, short category, string rawData)
        {
            EventLog.WriteEntry(Source, CreateMessage(userId, clientIp, url, method, host, message), type, eventId, category, GetBinaryData(rawData));
            return message;
        }

        private byte[] GetBinaryData(string rawData)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(rawData);
        }
        private string CreateMessage(string userId, string clientIp, string url, string method, string host, string message)
        {
            return
                string.Format(
                    "User ID : {0} , Client IP : {1} , Url : {2} , Method : {3} , Host : {4} , Message : {5}", userId,
                    clientIp, url, method, host, StringProcessingExtensions.Substring(message, 0, 30000));//event log max deger
        }

    }
}
