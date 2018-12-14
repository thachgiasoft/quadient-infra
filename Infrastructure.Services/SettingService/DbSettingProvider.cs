using System;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Core.Settings;
using Infrastructure.Data;

namespace Infrastructure.Services.SettingService
{
    public class DbSettingProvider : ISettingProvider
    {
        private const string SaveOrUpdateCommandText = @"  
            IF(SELECT COUNT(*) FROM APP_SETTINGS AS S WITH (NOLOCK) WHERE S.Name = @Name ) > 0
                BEGIN
                    UPDATE APP_SETTINGS SET Value=@Value,IsEncrypted=@IsEncrypted WHERE Name = @Name
                END
            ELSE 
                BEGIN
                    INSERT INTO APP_SETTINGS (Name,Value,IsEncrypted) VALUES (@Name,@Value,@IsEncrypted)
                END
            ";

        private readonly IDatabase _database;
        public DbSettingProvider(IDatabase database)
        {
            _database = database;
        }

        public void SetSetting(Setting setting)
        {
            var cmd = _database.GetSqlStringCommand(SaveOrUpdateCommandText);
            _database.AddInParameter(cmd, "@Name", DbType.String, setting.Key);
            _database.AddInParameter(cmd, "@Value", DbType.String, setting.Value);
            _database.AddInParameter(cmd, "@IsEncrypted", DbType.Boolean, setting.IsEncrypted);
            _database.ExecuteNonQuery(cmd);
        }

        public Setting GetSetting(string key)
        {
            var cmd = _database.GetSqlStringCommand("SELECT AppSettingId, Name, Value, IsEncrypted FROM APP_SETTINGS WHERE Name = @Name");
            _database.AddInParameter(cmd, "@Name", DbType.String, key);

            using (var reader = _database.ExecuteReader(cmd))
            {
                Setting setting = null;
                while (reader.Read())
                {
                    setting = new Setting()
                        {
                            Id = Convert.ToInt32(reader["AppSettingId"]),
                            Key = reader["Name"].ToString(),
                            Value = reader["Value"].ToString(),
                            IsEncrypted = bool.Parse(reader["IsEncrypted"].ToString())
                        };
                }
                reader.Close();
                return setting;
            }
        }

        public Setting GetSetting(int id)
        {
            var cmd = _database.GetSqlStringCommand("SELECT AppSettingId, Name, Value, IsEncrypted FROM APP_SETTINGS WHERE AppSettingId = @AppSettingId");
            _database.AddInParameter(cmd, "@AppSettingId", DbType.Int32, id);

            using (var reader = _database.ExecuteReader(cmd))
            {
                Setting setting = null;
                while (reader.Read())
                {
                    setting = new Setting()
                    {
                        Id = Convert.ToInt32(reader["AppSettingId"]),
                        Key = reader["Name"].ToString(),
                        Value = reader["Value"].ToString(),
                        IsEncrypted = bool.Parse(reader["IsEncrypted"].ToString())
                    };
                }
                reader.Close();
                return setting;
            }
        }

        public void DeleteSetting(string key)
        {
            var cmd = _database.GetSqlStringCommand("DELETE FROM APP_SETTINGS WHERE Name= @Name");
            _database.AddInParameter(cmd, "@Name", DbType.String, key);
            _database.ExecuteNonQuery(cmd);
        }

        public void DeleteSetting(int id)
        {
            var cmd = _database.GetSqlStringCommand("DELETE FROM APP_SETTINGS WHERE AppSettingId= @AppSettingId");
            _database.AddInParameter(cmd, "@AppSettingId", DbType.Int32, id);
            _database.ExecuteNonQuery(cmd);
        }

        public bool SettingExists(string key)
        {
            var setting = GetSetting(key);
            return setting != null;
        }

        public IList<Setting> GetAllSettings()
        {
            var cmd = _database.GetSqlStringCommand("SELECT * FROM APP_SETTINGS WITH(NOLOCK)");
            var retList = new List<Setting>();
            using (var reader = _database.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var setting = new Setting()
                        {

                            Id = Convert.ToInt32(reader["AppSettingId"]),
                            Key = reader["Name"].ToString(),
                            Value = reader["Value"].ToString(),
                            IsEncrypted = bool.Parse(reader["IsEncrypted"].ToString())
                        };
                    retList.Add(setting);
                }
                reader.Close();
            }
            return retList;
        }
    }
}
