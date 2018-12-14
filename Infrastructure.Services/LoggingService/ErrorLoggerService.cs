using System;
using System.Data;
using System.Diagnostics;
using Infrastructure.Core.Logging;
using Infrastructure.Data;
using Infrastructure.Data.Extensions;

namespace Infrastructure.Services.LoggingService
{
    public class ErrorLoggerService : ILogger
    {
        private readonly IDatabaseLite _databaseLite;
        private readonly IDataConfigurationSettings _dataConfigurationSettings;

        private const string InsertLogQuery =
            "Insert Into {0}.dbo.HATA_LOG(TarihSaat,Loglayan,BirimKey,KullaniciKey,IstemciBilgileri,SunucuBilgileri,SayfaURL,Method,Mesaj,Hata) values (@TarihSaat,@Loglayan,@BirimKey,@KullaniciKey,@IstemciBilgileri,@SunucuBilgileri,@SayfaURL,@Method,@Mesaj,@Hata) {1}";
        public const string ScopeIdentityQuery = " select SCOPE_IDENTITY() as identityC;";
        private readonly string _insertLogQuery;
        public ErrorLoggerService(IDatabaseLite databaseLite, IDataConfigurationSettings dataConfigurationSettings)
        {
            _databaseLite = databaseLite;
            _dataConfigurationSettings = dataConfigurationSettings;
            _insertLogQuery = string.Format(InsertLogQuery, _dataConfigurationSettings.LogDatabaseName ?? "IB_LOG_TEXT", ScopeIdentityQuery);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message)
        {
            return WriteLog(userId, clientIp, url, method, host, source, message, EventLogEntryType.Information);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type)
        {
            return WriteLog(userId, clientIp, url, method, host, source, message, type, string.Empty);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId)
        {
            return WriteLog(userId, clientIp, url, method, host, source, message, type, eventId, string.Empty);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, string rawData)
        {
            return WriteLog(userId, clientIp, url, method, host, source, message, type, 6666, rawData);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, short category)
        {
            return WriteLog(userId, clientIp, url, method, host, source, message, type, eventId, category, string.Empty);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, string rawData)
        {
            return WriteLog(userId, clientIp, url, method, host, source, message, type, eventId, 0, rawData);
        }

        public string WriteLog(string userId, string clientIp, string url, string method, string host, string source, string message, EventLogEntryType type, int eventId, short category, string rawData)
        {
            var cmd = _databaseLite.GetSqlStringCommand(_insertLogQuery);
            var param =
                "@TarihSaat,@Loglayan,@BirimKey,@KullaniciKey,@IstemciBilgileri,@SunucuBilgileri,@SayfaURL,@Method,@Mesaj,@Hata";
            cmd.AddParameterWithValue("@TarihSaat", DateTime.Now, DbType.DateTime);
            cmd.AddParameterWithValue("@BirimKey", -1, DbType.Int32);
            int userid;
            int.TryParse(userId, out userid);
            cmd.AddParameterWithValue("@KullaniciKey", userid, DbType.Int32);
            cmd.AddParameterWithValue("@Loglayan", source, DbType.String);
            cmd.AddParameterWithValue("@IstemciBilgileri", clientIp, DbType.String);
            cmd.AddParameterWithValue("@SunucuBilgileri", host, DbType.String);
            cmd.AddParameterWithValue("@SayfaURL", url, DbType.String);
            cmd.AddParameterWithValue("@Method", method, DbType.String);
            cmd.AddParameterWithValue("@Mesaj", message.Length <= 240 ? message : message.Substring(0, 240), DbType.String);
            cmd.AddParameterWithValue("@Hata", rawData, DbType.String);
            var result = _databaseLite.ExecuteScalar(cmd);
            return result != null ? result.ToString() : "-3";
        }

    }
}
