using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Data.Auditing;
using Infrastructure.Data.ComponentModel;
using Infrastructure.Data.TransactionManagement;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Infrastructure.Data
{
    public class DatabaseLite : IDatabaseLite
    {
        private readonly IDatabase _database;
        private readonly ICacheManager _cacheManager;
        private IAudit Audit
        {
            get
            {
                return EngineContext.Current.IsRegistered<IAudit>()
                           ? EngineContext.Current.Resolve<IAudit>()
                           : new EmptyAudit();
            }
        }
        private GenericTransaction _transaction;
        private const string CodeKey = "HashCode";
        private const int CommandTimeOut = 30;
        private readonly string _databaseName;
        public Guid DatabaseInstanceId { get; private set; }
        public bool TransactionActive { get { return _transaction != null && _transaction.Transaction != null && _transaction.Transaction.Connection != null && _transaction.Transaction.Connection.State == ConnectionState.Open; } }
        public GenericTransaction CurrentTransaction { get { return TransactionActive ? _transaction : null; } }
        public string DatabaseName { get { return _databaseName.ToUpper(CultureInfo.InvariantCulture); } }
        public DatabaseLite(IDatabase database, ICacheManager cacheManager)
        {
            _database = database;
            _cacheManager = cacheManager;
            DatabaseInstanceId = Guid.NewGuid();
            _databaseName = GetDatabaseNameFromConnectionString(_database.ConnectionStringWithoutCredentials);
        }

        public GenericTransaction BeginTransaction()
        {
            return _transaction = new GenericTransaction(_database, IsolationLevel.ReadCommitted);
        }

        public DbCommand GetSqlStringCommand()
        {
            return GetSqlStringCommand("SELECT GETDATE();");
        }

        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            return _database.GetStoredProcCommand(storedProcedureName);
        }

        public DbCommand GetSqlStringCommand(string query)
        {
            return _database.GetSqlStringCommand(query);
        }

        public SqlCommand GetSqlStringSqlCommand()
        {
            return GetSqlStringSqlCommand("SELECT GETDATE();");
        }

        public SqlCommand GetSqlStringSqlCommand(string query)
        {
            return _database.GetSqlStringCommand(query) as SqlCommand;
        }

        public void AddInParameter(DbCommand command, string name, SqlDbType dbType, object value)
        {
            _database.AddInParameter(command, name, dbType, value);
        }

        public T LoadDataTable<T>(DbCommand command, string tableName) where T : DataTable
        {
            var dt = Activator.CreateInstance<T>();
            using (var da = _database.GetDataAdapter())
            {
                da.SelectCommand = command;
                using (var con = _database.CreateConnection())
                {
                    da.SelectCommand.Connection = con;
                    da.Fill(dt);
                    if (string.IsNullOrEmpty(tableName))
                        tableName = dt.GetHashCode().ToString(CultureInfo.InvariantCulture);
                    if (string.Equals(dt.TableName, "Table") || string.IsNullOrEmpty(dt.TableName))
                    {
                        dt.TableName = tableName;
                    }
                }
                SetDataTableCommandToCache(command, dt);
            }
            return dt;
        }

        public T LoadDataTable<T>(DbCommand command) where T : DataTable
        {
            return LoadDataTable<T>(command, string.Empty);
        }

        public void LoadDataTable(DataTable table, DbCommand command)
        {
            IDataReader rdr = _database.ExecuteReader(command);
            const string tableName = "Table1";
            try
            {
                table.Load(rdr);
                if (String.IsNullOrEmpty(table.TableName))
                    table.TableName = tableName;
            }
            finally
            {
                rdr.Close();
                rdr.Dispose();
            }
        }

        public T LoadDataSet<T>(DbCommand command) where T : DataSet
        {
            return LoadDataSet<T>(command, string.Empty);
        }

        public T LoadDataSet<T>(DbCommand command, string tableName) where T : DataSet
        {
            var ds = Activator.CreateInstance<T>();
            if (!string.IsNullOrEmpty(command.CommandText))
            {
                if (string.IsNullOrEmpty(tableName) && ds.Tables.Count > 0)
                    tableName = ds.Tables[0].TableName;
                else
                    tableName = ds.GetHashCode().ToString(CultureInfo.InvariantCulture);
                _database.LoadDataSet(command, ds, tableName);
                SetDatasetCommandToCache(command, ds);
                return ds;
            }
            return ds;
        }

        public DataSet ExecuteDataSet(DbCommand command)
        {
            if (!string.IsNullOrEmpty(command.CommandText))
            {
                DataSet ds = _database.ExecuteDataSet(command);
                if (ds != null)
                {
                    SetDatasetCommandToCache(command, ds);
                    return ds;
                }
                return null;
            }
            return null;
        }

        public DataSet ExecuteDataSet(DbCommand command, GenericTransaction transaction)
        {
            CheckAndCloseConnection(command);
            var ds = _database.ExecuteDataSet(command, transaction.Transaction);
            if (ds != null)
            {
                SetDatasetCommandToCache(command, ds);
                SaveExecutedQuery(command);
            }
            return ds;
        }

        public DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteDataSet(storedProcedureName, parameterValues);
        }

        public DataSet ExecuteDataSet(GenericTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteDataSet(transaction.Transaction, storedProcedureName, parameterValues);
        }

        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            return _database.ExecuteDataSet(commandType, commandText);
        }

        public DataSet ExecuteDataSet(GenericTransaction transaction, CommandType commandType, string commandText)
        {
            return _database.ExecuteDataSet(transaction.Transaction, commandType, commandText);
        }

        public int ExecuteNonQuery(DbCommand command)
        {
            var r = _database.ExecuteNonQuery(command);
            SaveExecutedQuery(command);
            return r;
        }

        public int ExecuteNonQuery(DbCommand command, GenericTransaction transaction)
        {
            CheckAndCloseConnection(command);
            var r = _database.ExecuteNonQuery(command, transaction.Transaction);
            SaveExecutedQuery(command);
            return r;
        }

        public int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteNonQuery(storedProcedureName, parameterValues);
        }

        public int ExecuteNonQuery(GenericTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteNonQuery(transaction.Transaction, storedProcedureName, parameterValues);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return _database.ExecuteNonQuery(commandType, commandText);
        }

        public int ExecuteNonQuery(GenericTransaction transaction, CommandType commandType, string commandText)
        {
            return _database.ExecuteNonQuery(transaction.Transaction, commandType, commandText);
        }

        public object ExecuteScalar(DbCommand command)
        {
            var r = _database.ExecuteScalar(command);
            SaveExecutedQuery(command);
            return r;
        }

        public object ExecuteScalar(DbCommand command, GenericTransaction transaction)
        {
            CheckAndCloseConnection(command);
            var r = _database.ExecuteScalar(command, transaction.Transaction);
            SaveExecutedQuery(command);
            return r;
        }

        public object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteScalar(storedProcedureName, parameterValues);
        }

        public object ExecuteScalar(GenericTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteScalar(transaction.Transaction, storedProcedureName, parameterValues);
        }

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return _database.ExecuteScalar(commandType, commandText);
        }

        public object ExecuteScalar(GenericTransaction transaction, CommandType commandType, string commandText)
        {
            return _database.ExecuteScalar(transaction.Transaction, commandType, commandText);
        }

        public int UpdateDataTable(DataTable dataTable)
        {
            return UpdateDataTable(dataTable, tran: null);
        }

        public int UpdateDataTable(DataTable dataTable, GenericTransaction tran)
        {
            return UpdateDataTable(dataTable, tran, CommandTimeOut);
        }

        public int UpdateDataTable(DataTable dataTable, GenericTransaction tran, int commandTimeout)
        {
            return UpdateDataTable(dataTable, tran, commandTimeout, true);
        }

        public int UpdateDataTable(DataTable dataTable, GenericTransaction tran, int commandTimeout, bool acceptChangesDuringUpdate)
        {
            var cmd = GetDataTableCommandFromCache(dataTable);
            if (cmd == null)
            {
                //get select command : read from cache if exist, otherwise use basic select query.
                string cmdSelect = string.Format("select * from {0} WITH(NOLOCK) ", GetFullTableName(dataTable));
                cmd = GetSqlStringSqlCommand(cmdSelect);
                cmd.CommandTimeout = commandTimeout;
            }
            return UpdateDataTable(dataTable, cmd, tran, acceptChangesDuringUpdate);
        }

        public int UpdateDataTable(DataTable dataTable, DbCommand cmd)
        {
            return UpdateDataTable(dataTable, cmd, null);
        }

        public int UpdateDataTable(DataTable dataTable, DbCommand cmd, GenericTransaction tran)
        {
            return UpdateDataTable(dataTable, cmd, tran, true);
        }

        public int UpdateDataTable(DataTable dataTable, DbCommand cmd, GenericTransaction tran, bool acceptChangesDuringUpdate)
        {
            var rowsAffected = 0;
            using (var da = _database.GetDataAdapter())
            {
                if (da != null)
                {
                    var mCommandBuilder = GetCommandBuilder(da);
                    //Specifies whether all column values in an update statement are included or only changed ones.
                    mCommandBuilder.SetAllValues = false;
                    mCommandBuilder.ConflictOption = ConflictOption.CompareAllSearchableValues;

                    da.SelectCommand = cmd;
                    if (da.SelectCommand != null && tran != null)
                    {
                        da.SelectCommand.Connection = tran.Connection as DbConnection;
                        da.SelectCommand.Transaction = tran.Transaction;
                    }
                    else if (da.SelectCommand != null)
                    {
                        da.SelectCommand.Connection = _database.CreateConnection();
                    }

                    string sIdentityColumnName = GetIdentityColumnName(dataTable);
                    da.InsertCommand = mCommandBuilder.GetInsertCommand();
                    string insertCommandText = mCommandBuilder.GetInsertCommand().CommandText;

                    if (!String.IsNullOrEmpty(sIdentityColumnName))
                    {
                        insertCommandText += string.Format(";declare @id bigint;set @id=SCOPE_IDENTITY(); {0} WHERE {1} = @id;", cmd.CommandText, sIdentityColumnName);
                    }

                    da.InsertCommand.CommandText = insertCommandText;
                    da.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
                    da.UpdateCommand = mCommandBuilder.GetUpdateCommand();
                    da.DeleteCommand = mCommandBuilder.GetDeleteCommand();
                    da.InsertCommand.CommandTimeout = cmd.CommandTimeout;
                    da.UpdateCommand.CommandTimeout = cmd.CommandTimeout;
                    da.AcceptChangesDuringUpdate = acceptChangesDuringUpdate;
                    da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    var dtCopy = dataTable.Copy();
                    rowsAffected = da.Update(dataTable);
                    SaveDataChanges(dtCopy, dataTable);// audit old state
                }
            }

            return rowsAffected;
        }

        public int UpdateDataSet(DataSet dataSet)
        {
            return UpdateDataSet(dataSet, CommandTimeOut);
        }

        public int UpdateDataSet(DataSet dataSet, int commandTimeout)
        {
            return UpdateDataSet(dataSet, commandTimeout, true);
        }

        public int UpdateDataSet(DataSet dataSet, bool acceptChanges)
        {
            return UpdateDataSet(dataSet, UpdateBehavior.Standard, CommandTimeOut, acceptChanges);
        }

        public int UpdateDataSet(DataSet dataSet, int commandTimeout, bool acceptChanges)
        {
            return UpdateDataSet(dataSet, UpdateBehavior.Standard, commandTimeout, acceptChanges);
        }

        public int UpdateDataSet(DataSet dataSet, UpdateBehavior updateBehavior)
        {
            return UpdateDataSet(dataSet, updateBehavior, CommandTimeOut);
        }

        public int UpdateDataSet(DataSet dataSet, UpdateBehavior updateBehavior, int commandTimeout)
        {
            return UpdateDataSet(dataSet, updateBehavior, commandTimeout, true);
        }

        public int UpdateDataSet(DataSet dataSet, UpdateBehavior updateBehavior, int commandTimeout, bool acceptChanges)
        {
            return UpdateDataSet(dataSet, string.Empty, updateBehavior, null, null, commandTimeout, acceptChanges);
        }

        public int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int? updateBatchSize)
        {
            return UpdateDataSet(dataSet, transaction, updateBatchSize, CommandTimeOut);
        }

        public int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int? updateBatchSize, int commandTimeout)
        {
            return UpdateDataSet(dataSet, transaction, updateBatchSize, commandTimeout, true);
        }

        public int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int? updateBatchSize, int commandTimeout, bool acceptChange)
        {
            return UpdateDataSet(dataSet, string.Empty, UpdateBehavior.Standard, updateBatchSize, transaction, commandTimeout, acceptChange);
        }

        public int UpdateDataSet(DataSet dataSet, GenericTransaction transaction)
        {
            return UpdateDataSet(dataSet, transaction, CommandTimeOut);
        }

        public int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int commandTimeout)
        {
            return UpdateDataSet(dataSet, transaction, commandTimeout, true);
        }

        public int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int commandTimeout, bool acceptChanges)
        {
            return UpdateDataSet(dataSet, string.Empty, UpdateBehavior.Standard, null, transaction, commandTimeout, acceptChanges);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize)
        {
            return UpdateDataSet(dataSet, tableName, updateBehavior, updateBatchSize, null);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize,
                                 GenericTransaction tran)
        {
            return UpdateDataSet(dataSet, tableName, updateBehavior, updateBatchSize, tran, CommandTimeOut);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize,
                                 GenericTransaction tran, int commandTimeout)
        {
            return UpdateDataSet(dataSet, tableName, updateBehavior, updateBatchSize, tran, commandTimeout, true);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize, GenericTransaction tran, int commandTimeout, bool acceptChanges)
        {
            var cmd = GetDataSetCommandFromCache(dataSet);
            if (string.IsNullOrEmpty(tableName) && dataSet.Tables.Count > 0)
                tableName = GetTableName(dataSet.Tables[0]);
            var rowsAffected = 0;
            if (dataSet.Tables.Count == 0)
                throw new InvalidOperationException(string.Format("There is no table in dataset"));

            string cmdSelect = string.Format("select * from {0} WITH(NOLOCK) ", GetFullTableName(dataSet.Tables[0]));

            if (cmd == null)
            {
                //get select command : read from cache if exist, otherwise use basic select query.
                cmd = GetSqlStringSqlCommand(cmdSelect);
            }
            cmd.CommandTimeout = commandTimeout;
            using (var da = _database.GetDataAdapter())
            {
                if (da != null)
                {
                    var mCommandBuilder = GetCommandBuilder(da);
                    //Specifies whether all column values in an update statement are included or only changed ones.
                    mCommandBuilder.SetAllValues = false;
                    da.SelectCommand = cmd;
                    if (da.SelectCommand != null && tran != null)
                    {
                        da.SelectCommand.Connection = tran.Connection as DbConnection;
                        da.SelectCommand.Transaction = tran.Transaction;
                    }
                    else if (da.SelectCommand != null)
                    {
                        da.SelectCommand.Connection = _database.CreateConnection();
                    }
                    var dtCopy = dataSet.Tables[0].Copy();
                    string sIdentityColumnName = GetIdentityColumnName(dataSet.Tables[0]);
                    da.InsertCommand = mCommandBuilder.GetInsertCommand();
                    string insertCommandText = mCommandBuilder.GetInsertCommand().CommandText;

                    if (!String.IsNullOrEmpty(sIdentityColumnName))
                    {
                        insertCommandText += string.Format(";declare @id bigint;set @id=SCOPE_IDENTITY(); {0} WHERE {1} = @id;", cmdSelect, sIdentityColumnName);
                    }

                    da.InsertCommand.CommandText = insertCommandText;
                    da.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
                    da.UpdateCommand = mCommandBuilder.GetUpdateCommand();
                    da.DeleteCommand = mCommandBuilder.GetDeleteCommand();
                    da.InsertCommand.CommandTimeout = cmd.CommandTimeout;
                    da.UpdateCommand.CommandTimeout = cmd.CommandTimeout;
                    da.AcceptChangesDuringUpdate = acceptChanges;


                    //da.Update(dataSet.Tables[0]); //kapatildi
                    //tek data adapter kullanildiginda update isleminden sonra data adapter insert command i bozuluyor scope identity siliniyor
                    //bu sebeple 
                    //new adapter for performing the update 
                    using (var daAutoNum = _database.GetDataAdapter())
                    {
                        if (daAutoNum != null)
                        {
                            //new adaptor for performing the update                     
                            daAutoNum.DeleteCommand = da.DeleteCommand;
                            daAutoNum.InsertCommand = da.InsertCommand;
                            daAutoNum.UpdateCommand = da.UpdateCommand;
                            daAutoNum.InsertCommand.CommandTimeout = cmd.CommandTimeout;
                            daAutoNum.UpdateCommand.CommandTimeout = cmd.CommandTimeout;
                            daAutoNum.AcceptChangesDuringUpdate = acceptChanges;

                            //kapatildi
                            //if (tran != null)
                            //    rowsAffected = _database.UpdateDataSet(dataSet, tableName, daAutoNum.InsertCommand,
                            //                                           daAutoNum.UpdateCommand, daAutoNum.DeleteCommand,
                            //                                           tran.Transaction, updateBatchSize);
                            //else
                            //    rowsAffected = _database.UpdateDataSet(dataSet, tableName, daAutoNum.InsertCommand,
                            //                                           daAutoNum.UpdateCommand, daAutoNum.DeleteCommand,
                            //
                            //updateBehavior, updateBatchSize);


                            rowsAffected = daAutoNum.Update(dataSet.Tables[0]);

                            if (acceptChanges)
                                dataSet.AcceptChanges();
                            SaveDataChanges(dtCopy, dataSet.Tables[0]);
                        }
                    }
                }
            }

            return rowsAffected;
        }

        private string GetIdentityColumnName(DataTable pDataTable)
        {
            List<string> mColNames = (from DataColumn o in pDataTable.Columns
                                      where o.AutoIncrement == true
                                      select o.ColumnName).ToList();
            if (mColNames.Count == 0)
                return string.Empty;
            else
                return mColNames[0];
        }

        private void SetDatasetCommandToCache(DbCommand command, DataSet dataSet)
        {
            var strSql = command.CommandText.ToLower(CultureInfo.InvariantCulture);
            if (strSql.Contains("join"))
                return;
            var cmd = CloneCommand(command);
            var hashcode = dataSet.GetHashCode().ToString(CultureInfo.InvariantCulture);
            _cacheManager.Set(hashcode, cmd, new TimeSpan(0, 5, 0));
            dataSet.ExtendedProperties.Add(CodeKey, hashcode);
        }
        private DbCommand GetDataSetCommandFromCache(DataSet dataSet)
        {
            var dsCode = dataSet.ExtendedProperties[CodeKey];
            DbCommand cmd = null;
            if (dsCode != null)
                cmd = _cacheManager.Get<DbCommand>(dsCode.ToString());
            return cmd;
        }
        private void SetDataTableCommandToCache(DbCommand command, DataTable dataTable)
        {
            var strSql = command.CommandText.ToLower(CultureInfo.InvariantCulture);
            if (strSql.Contains("join"))
                return;
            var cmd = CloneCommand(command);
            var hashcode = dataTable.GetHashCode().ToString(CultureInfo.InvariantCulture);
            _cacheManager.Set(hashcode, cmd, new TimeSpan(0, 5, 0));
            dataTable.ExtendedProperties.Add(CodeKey, hashcode);
        }
        private DbCommand GetDataTableCommandFromCache(DataTable dataTable)
        {
            var dtCode = dataTable.ExtendedProperties[CodeKey];
            DbCommand cmd = null;
            if (dtCode != null)
                cmd = _cacheManager.Get<DbCommand>(dtCode.ToString());
            return cmd;
        }
        private void CheckAndCloseConnection(DbCommand command)
        {
            if (command.Connection != null && command.Connection.State == ConnectionState.Open)
                command.Connection.Close();
        }
        private string GetTableName(DataTable dataTable)
        {
            if (dataTable == null) throw new ArgumentNullException("dataTable");
            var tableName = dataTable.TableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = dataTable.GetHashCode().ToString(CultureInfo.InvariantCulture);
            return tableName;
        }
        private string GetFullTableName(DataTable dataTable)
        {
            if (dataTable == null) throw new ArgumentNullException("dataTable");
            var tableName = string.Empty;
            if (dataTable.GetType().IsDefined(typeof(FullTableNameAttribute), false))
            {
                var dbAttribute = (FullTableNameAttribute)
                                 dataTable.GetType()
                                                   .GetCustomAttributes(typeof(FullTableNameAttribute), false)[0];
                tableName = dbAttribute.FullTableName;
            }
            if (string.IsNullOrEmpty(tableName))
                tableName = dataTable.TableName;
            return tableName;
        }
        /// <summary>
        /// Verilen bir <see cref="System.Data.Common.DbDataAdapter"/> nesnesi ile ilişkilendirilmiş
        /// bir DbCommandBuilder nesnesi döndürür. Bu fonksiyon HybridDatabase nesnesi tarafından UpdateDataTable
        /// fonksiyonu içinde kullanıılır
        /// </summary>
        /// <param name="adapter">DbDataAdapter</param>
        /// <returns>DbCommandBuilder</returns>
        private DbCommandBuilder GetCommandBuilder(DbDataAdapter adapter)
        {
            var dataAdapter = adapter as OleDbDataAdapter;
            if (dataAdapter != null)
                return new OleDbCommandBuilder(dataAdapter);
            var sqlDataAdapter = adapter as SqlDataAdapter;
            if (sqlDataAdapter != null)
                return new SqlCommandBuilder(sqlDataAdapter);
            if (adapter is System.Data.OracleClient.OracleDataAdapter)
                return new OracleCommandBuilder((OracleDataAdapter)adapter);
            throw new InvalidOperationException("");
        }
        private DbCommand CloneCommand(DbCommand from)
        {
            var cmd = GetSqlStringCommand();
            cmd.CommandText = from.CommandText;
            cmd.CommandTimeout = from.CommandTimeout;
            cmd.CommandType = from.CommandType;
            cmd.DesignTimeVisible = from.DesignTimeVisible;
            cmd.UpdatedRowSource = from.UpdatedRowSource;
            var parameters = cmd.Parameters;
            foreach (object obj2 in from.Parameters)
            {
                parameters.Add((obj2 is ICloneable) ? (obj2 as ICloneable).Clone() : obj2);
            }

            CheckAndCloseConnection(cmd);
            return cmd;
        }

        private void SaveExecutedQuery(DbCommand cmd)
        {
            try
            {
                Audit.SaveExecutedQuery(cmd);
            }
            catch (Exception exception)
            {
                System.Diagnostics.EventLog.WriteEntry("Application", string.Format("Sql Query Audit Error : {0} - Command Text : {1}", exception, cmd != null ? cmd.CommandText : string.Empty), EventLogEntryType.Error);
            }
        }
        private void SaveDataChanges(DataTable oldDt, DataTable newDt)
        {
            try
            {
                Audit.SaveDataChanges(oldDt, newDt);
            }
            catch (Exception exception)
            {
                System.Diagnostics.EventLog.WriteEntry("Application", string.Format("Sql Data Changes Audit Error : {0} - Table Name : {1}", exception, oldDt.TableName), EventLogEntryType.Error);
            }
        }
        private string GetDatabaseNameFromConnectionString(string connectionString)
        {
            const string defaultValue = "IB_GENEL";
            var keyAliases = new string[] { "Database", "Initial Catalog" };
            var keyValuePairs = connectionString.Split(';')
                                     .Where(kvp => kvp.Contains('='))
                                     .Select(kvp => kvp.Split(new char[] { '=' }, 2))
                                     .ToDictionary(kvp => kvp[0].Trim().ToLower(),
                                                   kvp => kvp[1].Trim(),
                                                   StringComparer.InvariantCulture);
            foreach (var alias in keyAliases)
            {
                string value;
                if (keyValuePairs.TryGetValue(alias.ToLower(CultureInfo.InvariantCulture), out value))
                    return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            return defaultValue;
        }
    }
}
