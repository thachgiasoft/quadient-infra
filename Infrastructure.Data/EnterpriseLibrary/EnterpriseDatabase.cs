using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using Infrastructure.Core.Caching;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Infrastructure.Data.EnterpriseLibrary
{
    public class EnterpriseDatabase : IDatabase
    {
        private readonly Database _database;
        public Guid DatabaseInstanceId { get; private set; }
        public EnterpriseDatabase(Database database)
        {
            _database = database;
            DatabaseInstanceId = Guid.NewGuid();
        }
        public bool SupportsAsync { get { return _database.SupportsAsync; } }
        public bool SupportsParemeterDiscovery { get { return _database.SupportsParemeterDiscovery; } }
        public string ConnectionStringWithoutCredentials { get { return _database.ConnectionStringWithoutCredentials; } }
        public DbProviderFactory DbProviderFactory { get { return _database.DbProviderFactory; } }
        public XmlReader ExecuteXmlReader(DbCommand command)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            return ((SqlDatabase)_database).ExecuteXmlReader(command);
        }

        public XmlReader ExecuteXmlReader(DbCommand command, DbTransaction transaction)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            return ((SqlDatabase)_database).ExecuteXmlReader(command, transaction);
        }

        public IAsyncResult BeginExecuteXmlReader(DbCommand command, AsyncCallback callback, object state)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            return ((SqlDatabase)_database).BeginExecuteXmlReader(command, callback, state);
        }

        public IAsyncResult BeginExecuteXmlReader(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            return ((SqlDatabase)_database).BeginExecuteXmlReader(command, transaction, callback, state);
        }

        public XmlReader EndExecuteXmlReader(IAsyncResult asyncResult)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            return ((SqlDatabase)_database).EndExecuteXmlReader(asyncResult);
        }

        public string BuildParameterName(string name)
        {
            return _database.BuildParameterName(name);
        }

        public void AddParameter(DbCommand command, string name, SqlDbType dbType, int size, ParameterDirection direction,
                                 bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion,
                                 object value)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            ((SqlDatabase)_database).AddParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
        }

        public void AddParameter(DbCommand command, string name, SqlDbType dbType, ParameterDirection direction, string sourceColumn,
                                 DataRowVersion sourceVersion, object value)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            ((SqlDatabase)_database).AddParameter(command, name, dbType, direction, sourceColumn, sourceVersion, value);
        }

        public void AddOutParameter(DbCommand command, string name, SqlDbType dbType, int size)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            ((SqlDatabase)_database).AddOutParameter(command, name, dbType, size);
        }

        public void AddInParameter(DbCommand command, string name, SqlDbType dbType)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            ((SqlDatabase)_database).AddInParameter(command, name, dbType);
        }

        public void AddInParameter(DbCommand command, string name, SqlDbType dbType, object value)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            ((SqlDatabase)_database).AddInParameter(command, name, dbType, value);
        }

        public void AddInParameter(DbCommand command, string name, SqlDbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            if (!(_database is SqlDatabase))
                throw new NotSupportedException(string.Format("Database type does not support this method. Database Type : {0}", _database.GetType()));
            ((SqlDatabase)_database).AddInParameter(command, name, dbType, sourceColumn, sourceVersion);
        }

        public IAsyncResult BeginExecuteNonQuery(DbCommand command, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteNonQuery(command, callback, state);
        }

        public IAsyncResult BeginExecuteNonQuery(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteNonQuery(command, transaction, callback, state);
        }

        public IAsyncResult BeginExecuteNonQuery(string storedProcedureName, AsyncCallback callback, object state,
                                                 params object[] parameterValues)
        {
            return _database.BeginExecuteNonQuery(storedProcedureName, callback, state, parameterValues);
        }

        public IAsyncResult BeginExecuteNonQuery(DbTransaction transaction, string storedProcedureName, AsyncCallback callback,
                                                 object state, params object[] parameterValues)
        {
            return _database.BeginExecuteNonQuery(transaction, storedProcedureName, callback, state, parameterValues);
        }

        public IAsyncResult BeginExecuteNonQuery(CommandType commandType, string commandText, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteNonQuery(commandType, commandText, callback, state);
        }

        public IAsyncResult BeginExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText,
                                                 AsyncCallback callback, object state)
        {
            return _database.BeginExecuteNonQuery(transaction, commandType, commandText, callback, state);
        }

        public int EndExecuteNonQuery(IAsyncResult asyncResult)
        {
            return _database.EndExecuteNonQuery(asyncResult);
        }

        public IAsyncResult BeginExecuteReader(DbCommand command, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteReader(command, callback, state);
        }

        public IAsyncResult BeginExecuteReader(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteReader(command, transaction, callback, state);
        }

        public IAsyncResult BeginExecuteReader(string storedProcedureName, AsyncCallback callback, object state,
                                               params object[] parameterValues)
        {
            return _database.BeginExecuteReader(storedProcedureName, callback, state, parameterValues);
        }

        public IAsyncResult BeginExecuteReader(DbTransaction transaction, string storedProcedureName, AsyncCallback callback,
                                               object state, params object[] parameterValues)
        {
            return _database.BeginExecuteReader(transaction, storedProcedureName, callback, state, parameterValues);
        }

        public IAsyncResult BeginExecuteReader(CommandType commandType, string commandText, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteReader(commandType, commandText, callback, state);
        }

        public IAsyncResult BeginExecuteReader(DbTransaction transaction, CommandType commandType, string commandText,
                                               AsyncCallback callback, object state)
        {
            return _database.BeginExecuteReader(transaction, commandType, commandText, callback, state);
        }

        public IDataReader EndExecuteReader(IAsyncResult asyncResult)
        {
            return _database.EndExecuteReader(asyncResult);
        }

        public IAsyncResult BeginExecuteScalar(DbCommand command, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteScalar(command, callback, state);
        }

        public IAsyncResult BeginExecuteScalar(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteScalar(command, transaction, callback, state);
        }

        public IAsyncResult BeginExecuteScalar(string storedProcedureName, AsyncCallback callback, object state,
                                               params object[] parameterValues)
        {
            return _database.BeginExecuteScalar(storedProcedureName, callback, state, parameterValues);
        }

        public IAsyncResult BeginExecuteScalar(DbTransaction transaction, string storedProcedureName, AsyncCallback callback,
                                               object state, params object[] parameterValues)
        {
            return _database.BeginExecuteScalar(transaction, storedProcedureName, callback, state, parameterValues);
        }

        public IAsyncResult BeginExecuteScalar(CommandType commandType, string commandText, AsyncCallback callback, object state)
        {
            return _database.BeginExecuteScalar(commandType, commandText, callback, state);
        }

        public IAsyncResult BeginExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText,
                                               AsyncCallback callback, object state)
        {
            return _database.BeginExecuteScalar(transaction, commandType, commandText, callback, state);
        }

        public object EndExecuteScalar(IAsyncResult asyncResult)
        {
            return _database.EndExecuteScalar(asyncResult);
        }

        public void AddInParameter(DbCommand command, string name, DbType dbType)
        {
            _database.AddInParameter(command, name, dbType);
        }

        public void AddInParameter(DbCommand command, string name, DbType dbType, object value)
        {
            _database.AddInParameter(command, name, dbType, value);
        }

        public void AddInParameter(DbCommand command, string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            _database.AddInParameter(command, name, dbType, sourceColumn, sourceVersion);
        }

        public void AddOutParameter(DbCommand command, string name, DbType dbType, int size)
        {
            _database.AddOutParameter(command, name, dbType, size);
        }

        public void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable,
                                 byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            _database.AddParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
        }

        public void AddParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction, string sourceColumn,
                                 DataRowVersion sourceVersion, object value)
        {
            _database.AddParameter(command, name, dbType, direction, sourceColumn, sourceVersion, value);
        }

        public DbConnection CreateConnection()
        {
            return _database.CreateConnection();
        }

        public void DiscoverParameters(DbCommand command)
        {
            _database.DiscoverParameters(command);
        }

        public DataSet ExecuteDataSet(DbCommand command)
        {
            return _database.ExecuteDataSet(command);
        }

        public DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction)
        {
            return _database.ExecuteDataSet(command, transaction);
        }

        public DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteDataSet(storedProcedureName, parameterValues);
        }

        public DataSet ExecuteDataSet(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteDataSet(transaction, storedProcedureName, parameterValues);
        }

        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            return _database.ExecuteDataSet(commandType, commandText);
        }

        public DataSet ExecuteDataSet(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return _database.ExecuteDataSet(transaction, commandType, commandText);
        }

        public int ExecuteNonQuery(DbCommand command)
        {
            return _database.ExecuteNonQuery(command);
        }

        public int ExecuteNonQuery(DbCommand command, DbTransaction transaction)
        {
            return _database.ExecuteNonQuery(command, transaction);
        }

        public int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteNonQuery(storedProcedureName, parameterValues);
        }

        public int ExecuteNonQuery(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteNonQuery(transaction, storedProcedureName, parameterValues);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return _database.ExecuteNonQuery(commandType, commandText);
        }

        public int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return _database.ExecuteNonQuery(transaction, commandType, commandText);
        }

        public IDataReader ExecuteReader(DbCommand command)
        {
            return _database.ExecuteReader(command);
        }

        public IDataReader ExecuteReader(DbCommand command, DbTransaction transaction)
        {
            return _database.ExecuteReader(command, transaction);
        }

        public IDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteReader(storedProcedureName, parameterValues);
        }

        public IDataReader ExecuteReader(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteReader(transaction, storedProcedureName, parameterValues);
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return _database.ExecuteReader(commandType, commandText);
        }

        public IDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return _database.ExecuteReader(transaction, commandType, commandText);
        }

        public object ExecuteScalar(DbCommand command)
        {
            return _database.ExecuteScalar(command);
        }

        public object ExecuteScalar(DbCommand command, DbTransaction transaction)
        {
            return _database.ExecuteScalar(command, transaction);
        }

        public object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteScalar(storedProcedureName, parameterValues);
        }

        public object ExecuteScalar(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            return _database.ExecuteScalar(transaction, storedProcedureName, parameterValues);
        }

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return _database.ExecuteScalar(commandType, commandText);
        }

        public object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return _database.ExecuteScalar(transaction, commandType, commandText);
        }

        public DbDataAdapter GetDataAdapter()
        {
            return _database.GetDataAdapter();
        }

        public object GetParameterValue(DbCommand command, string name)
        {
            return _database.GetParameterValue(command, name);
        }


        public DbCommand GetSqlStringCommand(string query)
        {
            return _database.GetSqlStringCommand(query);
        }

        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            return _database.GetStoredProcCommand(storedProcedureName);
        }

        public DbCommand GetStoredProcCommand(string storedProcedureName, params object[] parameterValues)
        {
            return _database.GetStoredProcCommand(storedProcedureName, parameterValues);
        }

        public void AssignParameters(DbCommand command, object[] parameterValues)
        {
            _database.AssignParameters(command, parameterValues);
        }

        public DbCommand GetStoredProcCommandWithSourceColumns(string storedProcedureName, params string[] sourceColumns)
        {
            return _database.GetStoredProcCommandWithSourceColumns(storedProcedureName, sourceColumns);
        }

        public void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            _database.LoadDataSet(command, dataSet, tableName);
        }

        public void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, DbTransaction transaction)
        {
            _database.LoadDataSet(command, dataSet, tableName, transaction);
        }

        public void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            _database.LoadDataSet(command, dataSet, tableNames);
        }

        public void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, DbTransaction transaction)
        {
            _database.LoadDataSet(command, dataSet, tableNames, transaction);
        }

        public void LoadDataSet(string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            _database.LoadDataSet(storedProcedureName, dataSet, tableNames, parameterValues);
        }

        public void LoadDataSet(DbTransaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames,
                                params object[] parameterValues)
        {
            _database.LoadDataSet(transaction, storedProcedureName, dataSet, tableNames, parameterValues);
        }

        public void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            _database.LoadDataSet(commandType, commandText, dataSet, tableNames);
        }

        public void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet,
                                string[] tableNames)
        {
            _database.LoadDataSet(transaction, commandType, commandText, dataSet, tableNames);
        }

        public void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            _database.SetParameterValue(command, parameterName, value);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand,
                                 DbCommand deleteCommand, UpdateBehavior updateBehavior, int? updateBatchSize)
        {
            return _database.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand,
                                           updateBehavior, updateBatchSize);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand,
                                 DbCommand deleteCommand, UpdateBehavior updateBehavior)
        {
            return _database.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand,
                                           updateBehavior);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand,
                                 DbCommand deleteCommand, DbTransaction transaction, int? updateBatchSize)
        {
            return _database.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, transaction,
                                           updateBatchSize);
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand,
                                 DbCommand deleteCommand, DbTransaction transaction)
        {
            return _database.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, transaction);
        }
    }
}
