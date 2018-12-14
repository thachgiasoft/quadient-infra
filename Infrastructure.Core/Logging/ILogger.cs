using System.Diagnostics;
using System.ServiceModel;

namespace Infrastructure.Core.Logging
{
    [ServiceContract]
    public interface ILogger
    {
        [OperationContract(Name = "WriteLog1")]
        string WriteLog(string userId, string clientIp, string url, string method, string host,string source, string message);
        [OperationContract(Name = "WriteLog2")]
        string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type);
        [OperationContract(Name = "WriteLog3")]
        string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId);
        [OperationContract(Name = "WriteLog4")]
        string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, string rawData);
        [OperationContract(Name = "WriteLog5")]
        string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, short category);
        [OperationContract(Name = "WriteLog6")]
        string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, string rawData);
        [OperationContract(Name = "WriteLog7")]
        string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, short category, string rawData);


    }
}
