using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Infrastructure.Data
{
    /// <summary>
    /// To Make IOC Friendly
    /// </summary>
    public interface IDatabase
    {
        Guid DatabaseInstanceId { get; }

        /// <summary>
        /// Does this <see cref='Database'/> object support asynchronous execution?
        /// </summary>
        /// <value>true.</value>
        bool SupportsAsync { get; }

        /// <summary>
        /// Does this <see cref='Database'/> object support parameter discovery?
        /// </summary>
        /// <value>true.</value>
        bool SupportsParemeterDiscovery { get; }

        /// <summary>
        /// Gets the connection string without credentials.
        /// </summary>
        /// <value>
        /// The connection string without credentials.
        /// </value>
        string ConnectionStringWithoutCredentials { get; }

        /// <summary>
        /// <para>Gets the DbProviderFactory used by the database instance.</para>
        /// </summary>
        /// <seealso cref="DbProviderFactory"/>
        DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// <para>Executes the <see cref="SqlCommand"/> and returns a new <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <remarks>
        ///	When the returned reader is closed, the underlying connection will be closed
        /// (with appropriate handling of connections in the case of an ambient transaction).
        /// This is a behavior change from Enterprise Library versions prior to v5.
        /// </remarks>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        XmlReader ExecuteXmlReader(DbCommand command);

        /// <summary>
        /// <para>Executes the <see cref="SqlCommand"/> in a transaction and returns a new <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <remarks>
        ///		Unlike other Execute... methods that take a <see cref="DbCommand"/> instance, this method
        ///		does not set the command behavior to close the connection when you close the reader.
        ///		That means you'll need to close the connection yourself, by calling the
        ///		command.Connection.Close() method.
        /// </remarks>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        XmlReader ExecuteXmlReader(DbCommand command, DbTransaction transaction);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <see cref="SqlCommand"/> which will result in a <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <seealso cref="SqlDatabase.ExecuteXmlReader(System.Data.Common.DbCommand)"/>
        /// <seealso cref="SqlDatabase.EndExecuteXmlReader"/>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteXmlReader"/>, 
        /// which returns the <see cref="XmlReader"/> object.</para>
        /// </returns>
        IAsyncResult BeginExecuteXmlReader(DbCommand command, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <see cref="SqlCommand"/> inside a transaction which will result in a <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <seealso cref="SqlDatabase.ExecuteXmlReader(System.Data.Common.DbCommand,System.Data.Common.DbTransaction)"/>
        /// <seealso cref="SqlDatabase.EndExecuteXmlReader"/>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteXmlReader"/>, 
        /// which returns the <see cref="XmlReader"/> object.</para>
        /// </returns>
        IAsyncResult BeginExecuteXmlReader(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state);

        /// <summary>
        /// Finishes asynchronous execution of a Transact-SQL statement, returning the requested data as XML.
        /// </summary>
        /// <param name="asyncResult">
        /// <para>The <see cref="IAsyncResult"/> returned by a call to any overload of <see cref="SqlDatabase.BeginExecuteXmlReader(System.Data.Common.DbCommand,System.AsyncCallback,object)"/>.</para>
        /// </param>
        /// <seealso cref="SqlDatabase.ExecuteXmlReader(System.Data.Common.DbCommand)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteXmlReader(System.Data.Common.DbCommand,System.AsyncCallback,object)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteXmlReader(System.Data.Common.DbCommand,System.Data.Common.DbTransaction,System.AsyncCallback,object)"/>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object that can be used to fetch the resulting XML data.</para>
        /// </returns>
        XmlReader EndExecuteXmlReader(IAsyncResult asyncResult);

        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        string BuildParameterName(string name);

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>A value indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        void AddParameter(DbCommand command, string name, SqlDbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value);

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        void AddParameter(DbCommand command, string name, SqlDbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value);

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        void AddOutParameter(DbCommand command, string name, SqlDbType dbType, int size);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        void AddInParameter(DbCommand command, string name, SqlDbType dbType);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        void AddInParameter(DbCommand command, string name, SqlDbType dbType, object value);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        void AddInParameter(DbCommand command, string name, SqlDbType dbType, string sourceColumn, DataRowVersion sourceVersion);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <see cref="SqlCommand"/> which will return the number of affected records.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <seealso cref="Database.ExecuteNonQuery(DbCommand)"/>
        /// <seealso cref="SqlDatabase.EndExecuteNonQuery"/>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteNonQuery"/>, 
        /// which returns the number of affected records.</para>
        /// </returns>
        IAsyncResult BeginExecuteNonQuery(DbCommand command, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <see cref="DbCommand"/> inside a transaction which will return the number of affected records.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <seealso cref="Database.ExecuteNonQuery(DbCommand)"/>
        /// <seealso cref="SqlDatabase.EndExecuteNonQuery"/>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteNonQuery"/>, 
        /// which returns the number of affected records.</para>
        /// </returns>
        IAsyncResult BeginExecuteNonQuery(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> which will return the number of rows affected.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteNonQuery"/>, 
        /// which returns the number of affected records.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteNonQuery(string,object[])"/>
        /// <seealso cref="SqlDatabase.EndExecuteNonQuery"/>
        IAsyncResult BeginExecuteNonQuery(string storedProcedureName, AsyncCallback callback, object state, params object[] parameterValues);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> inside a transaction which will return the number of rows affected.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteNonQuery"/>, 
        /// which returns the number of affected records.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteNonQuery(string,object[])"/>
        /// <seealso cref="SqlDatabase.EndExecuteNonQuery"/>
        IAsyncResult BeginExecuteNonQuery(DbTransaction transaction, string storedProcedureName,
                                                          AsyncCallback callback, object state,
                                                          params object[] parameterValues);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> which will return the number of rows affected.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteNonQuery"/>, 
        /// which returns the number of affected records.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteNonQuery(CommandType,string)"/>
        /// <seealso cref="SqlDatabase.EndExecuteNonQuery"/>
        IAsyncResult BeginExecuteNonQuery(CommandType commandType, string commandText, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> inside a tranasaction which will return the number of rows affected.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteNonQuery"/>, 
        /// which returns the number of affected records.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteNonQuery(CommandType,string)"/>
        /// <seealso cref="SqlDatabase.EndExecuteNonQuery"/>
        IAsyncResult BeginExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText,
                                                          AsyncCallback callback, object state);

        /// <summary>
        /// Finishes asynchronous execution of a Transact-SQL statement, returning the number of affected records.
        /// </summary>
        /// <param name="asyncResult">
        /// <para>The <see cref="IAsyncResult"/> returned by a call to any overload of <see cref="SqlDatabase.BeginExecuteNonQuery(System.Data.Common.DbCommand,System.AsyncCallback,object)"/>.</para>
        /// </param>
        /// <seealso cref="Database.ExecuteNonQuery(DbCommand)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteNonQuery(System.Data.Common.DbCommand,System.AsyncCallback,object)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteNonQuery(System.Data.Common.DbCommand,System.Data.Common.DbTransaction,System.AsyncCallback,object)"/>
        /// <returns>
        /// <para>The number of affected records.</para>
        /// </returns>
        int EndExecuteNonQuery(IAsyncResult asyncResult);

        /// <summary>
        /// <para>Initiates the asynchronous execution of a <paramref name="command"/> which will return a <see cref="IDataReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteReader"/>, 
        /// which returns the <see cref="IDataReader"/>.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteReader(DbCommand)"/>
        /// <seealso cref="SqlDatabase.EndExecuteReader"/>
        IAsyncResult BeginExecuteReader(DbCommand command, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of a <paramref name="command"/> inside a transaction which will return a <see cref="IDataReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteReader"/>, 
        /// which returns the <see cref="IDataReader"/>.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteReader(DbCommand)"/>
        /// <seealso cref="SqlDatabase.EndExecuteReader"/>
        IAsyncResult BeginExecuteReader(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> which will return a <see cref="IDataReader"/>.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteReader"/>, 
        /// which returns the <see cref="IDataReader"/>.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteReader(string, object[])"/>
        /// <seealso cref="SqlDatabase.EndExecuteReader"/>
        IAsyncResult BeginExecuteReader(string storedProcedureName, AsyncCallback callback, object state, params object[] parameterValues);

        /// <summary>
        /// <para>Initiates the asynchronous execution of <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> inside a transaction which will return a <see cref="IDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteReader"/>, 
        /// which returns the <see cref="IDataReader"/>.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteReader(DbTransaction, string, object[])"/>
        /// <seealso cref="SqlDatabase.EndExecuteReader"/>
        IAsyncResult BeginExecuteReader(DbTransaction transaction, string storedProcedureName, AsyncCallback callback, object state, params object[] parameterValues);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <paramref name="commandText"/> 
        /// interpreted as specified by the <paramref name="commandType" /> which will return 
        /// a <see cref="IDataReader"/>. When the async operation completes, the
        /// <paramref name="callback"/> will be invoked on another thread to process the
        /// result.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="callback"><see cref="AsyncCallback"/> to execute when the async operation
        /// completes.</param>
        /// <param name="state">State object passed to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteReader"/>, 
        /// which returns the <see cref="IDataReader"/>.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteReader(CommandType, string)"/>
        /// <seealso cref="SqlDatabase.EndExecuteReader"/>
        IAsyncResult BeginExecuteReader(CommandType commandType, string commandText, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> inside an transaction which will return a <see cref="IDataReader"/>.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteReader"/>, 
        /// which returns the <see cref="IDataReader"/>.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteReader(CommandType, string)"/>
        /// <seealso cref="SqlDatabase.EndExecuteReader"/>
        IAsyncResult BeginExecuteReader(DbTransaction transaction, CommandType commandType, string commandText,
                                                        AsyncCallback callback, object state);

        /// <summary>
        /// Finishes asynchronous execution of a Transact-SQL statement, returning an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="asyncResult">
        /// <para>The <see cref="IAsyncResult"/> returned by a call to any overload of BeginExecuteReader.</para>
        /// </param>
        /// <seealso cref="Database.ExecuteReader(DbCommand)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteReader(System.Data.Common.DbCommand,System.AsyncCallback,object)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteReader(System.Data.Common.DbCommand,System.Data.Common.DbTransaction,System.AsyncCallback,object)"/>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object that can be used to consume the queried information.</para>
        /// </returns>     
        IDataReader EndExecuteReader(IAsyncResult asyncResult);

        /// <summary>
        /// <para>Initiates the asynchronous execution of a <paramref name="command"/> which will return a single value.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteScalar"/>, 
        /// which returns the actual result.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteScalar(DbCommand)"/>
        /// <seealso cref="SqlDatabase.EndExecuteScalar"/>
        IAsyncResult BeginExecuteScalar(DbCommand command, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of a <paramref name="command"/> inside a transaction which will return a single value.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteScalar"/>, 
        /// which returns the actual result.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteScalar(DbCommand, DbTransaction)"/>
        /// <seealso cref="SqlDatabase.EndExecuteScalar"/>
        IAsyncResult BeginExecuteScalar(DbCommand command, DbTransaction transaction, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> which will return a single value.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteScalar"/>, 
        /// which returns the actual result.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteScalar(string, object[])"/>
        /// <seealso cref="SqlDatabase.EndExecuteScalar"/>
        IAsyncResult BeginExecuteScalar(string storedProcedureName, AsyncCallback callback, object state, params object[] parameterValues);

        /// <summary>
        /// <para>Initiates the asynchronous execution of <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> inside a transaction which will return a single value.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteScalar"/>, 
        /// which returns the actual result.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteScalar(DbTransaction, string, object[])"/>
        /// <seealso cref="SqlDatabase.EndExecuteScalar"/>
        IAsyncResult BeginExecuteScalar(DbTransaction transaction, string storedProcedureName, AsyncCallback callback, object state, params object[] parameterValues);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> which will return a single value.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteScalar"/>, 
        /// which returns the actual result.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteScalar(CommandType, string)"/>
        /// <seealso cref="SqlDatabase.EndExecuteScalar"/>
        IAsyncResult BeginExecuteScalar(CommandType commandType, string commandText, AsyncCallback callback, object state);

        /// <summary>
        /// <para>Initiates the asynchronous execution of the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> inside an transaction which will return a single value.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="callback">The async callback to execute when the result of the operation is available. Pass <langword>null</langword>
        /// if you don't want to use a callback.</param>
        /// <param name="state">Additional state object to pass to the callback.</param>
        /// <returns>
        /// <para>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, or both; 
        /// this value is also needed when invoking <see cref="SqlDatabase.EndExecuteScalar"/>, 
        /// which returns the actual result.</para>
        /// </returns>
        /// <seealso cref="Database.ExecuteScalar(DbTransaction, CommandType, string)"/>
        /// <seealso cref="SqlDatabase.EndExecuteScalar"/>
        IAsyncResult BeginExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText,
                                                        AsyncCallback callback, object state);

        /// <summary>
        /// <para>Finishes asynchronous execution of a Transact-SQL statement, returning the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="asyncResult">
        /// <para>The <see cref="IAsyncResult"/> returned by a call to any overload of BeginExecuteScalar.</para>
        /// </param>
        /// <seealso cref="Database.ExecuteScalar(DbCommand)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteScalar(System.Data.Common.DbCommand,System.AsyncCallback,object)"/>
        /// <seealso cref="SqlDatabase.BeginExecuteScalar(System.Data.Common.DbCommand,System.Data.Common.DbTransaction,System.AsyncCallback,object)"/>
        /// <returns>
        /// <para>The value of the first column of the first row in the result set returned by the query.
        /// If the result didn't have any columns or rows <see langword="null"/> (<b>Nothing</b> in Visual Basic).</para>
        /// </returns>     
        object EndExecuteScalar(IAsyncResult asyncResult);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        void AddInParameter(DbCommand command,
                                            string name,
                                            DbType dbType);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        void AddInParameter(DbCommand command,
                                            string name,
                                            DbType dbType,
                                            object value);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        void AddInParameter(DbCommand command,
                                            string name,
                                            DbType dbType,
                                            string sourceColumn,
                                            DataRowVersion sourceVersion);

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        void AddOutParameter(DbCommand command,
                                             string name,
                                             DbType dbType,
                                             int size);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        void AddParameter(DbCommand command,
                                          string name,
                                          DbType dbType,
                                          int size,
                                          ParameterDirection direction,
                                          bool nullable,
                                          byte precision,
                                          byte scale,
                                          string sourceColumn,
                                          DataRowVersion sourceVersion,
                                          object value);

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        void AddParameter(DbCommand command,
                                          string name,
                                          DbType dbType,
                                          ParameterDirection direction,
                                          string sourceColumn,
                                          DataRowVersion sourceVersion,
                                          object value);

        /// <summary>
        /// <para>Creates a connection for this database.</para>
        /// </summary>
        /// <returns>
        /// <para>The <see cref="DbConnection"/> for this database.</para>
        /// </returns>
        /// <seealso cref="DbConnection"/>        
        DbConnection CreateConnection();

        /// <summary>
        /// Discovers the parameters for a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> to discover the parameters.</param>
        void DiscoverParameters(DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet ExecuteDataSet(DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet ExecuteDataSet(DbCommand command,
                                               DbTransaction transaction);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        DataSet ExecuteDataSet(string storedProcedureName,
                                               params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues" /> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/> within a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        DataSet ExecuteDataSet(DbTransaction transaction,
                                               string storedProcedureName,
                                               params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        DataSet ExecuteDataSet(CommandType commandType,
                                               string commandText);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        DataSet ExecuteDataSet(DbTransaction transaction,
                                               CommandType commandType,
                                               string commandText);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>       
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" />, and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(DbCommand command,
                                            DbTransaction transaction);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(string storedProcedureName,
                                            params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> within a transaction and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(DbTransaction transaction,
                                            string storedProcedureName,
                                            params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(CommandType commandType,
                                            string commandText);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> as part of the given <paramref name="transaction" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(DbTransaction transaction,
                                            CommandType commandType,
                                            string commandText);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        IDataReader ExecuteReader(DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a transaction and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        IDataReader ExecuteReader(DbCommand command,
                                                  DbTransaction transaction);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>        
        /// <param name="storedProcedureName">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        IDataReader ExecuteReader(string storedProcedureName,
                                                  params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> within the given <paramref name="transaction" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        IDataReader ExecuteReader(DbTransaction transaction,
                                                  string storedProcedureName,
                                                  params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        IDataReader ExecuteReader(CommandType commandType,
                                                  string commandText);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> within the given 
        /// <paramref name="transaction" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        IDataReader ExecuteReader(DbTransaction transaction,
                                                  CommandType commandType,
                                                  string commandText);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(DbCommand command,
                                             DbTransaction transaction);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(string storedProcedureName,
                                             params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> within a 
        /// <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(DbTransaction transaction,
                                             string storedProcedureName,
                                             params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" />  and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(CommandType commandType,
                                             string commandText);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> 
        /// within the given <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(DbTransaction transaction,
                                             CommandType commandType,
                                             string commandText);

        /// <summary>
        /// Gets a DbDataAdapter with Standard update behavior.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        /// <devdoc>
        /// Created this new, public method instead of modifying the protected, abstract one so that there will be no
        /// breaking changes for any currently derived Database class.
        /// </devdoc>
        DbDataAdapter GetDataAdapter();

        /// <summary>
        /// Gets a parameter value.
        /// </summary>
        /// <param name="command">The command that contains the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        object GetParameterValue(DbCommand command,
                                                 string name);

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DbCommand"/> for the SQL query.</para></returns>        
        DbCommand GetSqlStringCommand(string query);

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>       
        DbCommand GetStoredProcCommand(string storedProcedureName);

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        DbCommand GetStoredProcCommand(string storedProcedureName,
                                                       params object[] parameterValues);

        /// <summary>
        /// <para>Discovers parameters on the <paramref name="command"/> and assigns the values from <paramref name="parameterValues"/> to the <paramref name="command"/>s Parameters list.</para>
        /// </summary>
        /// <param name="command">The command the parameeter values will be assigned to</param>
        /// <param name="parameterValues">The parameter values that will be assigned to the command.</param>
        void AssignParameters(DbCommand command, object[] parameterValues);

        /// <summary>
        /// Wraps around a derived class's implementation of the GetStoredProcCommandWrapper method and adds functionality for
        /// using this method with UpdateDataSet.  The GetStoredProcCommandWrapper method (above) that takes a params array 
        /// expects the array to be filled with VALUES for the parameters. This method differs from the GetStoredProcCommandWrapper 
        /// method in that it allows a user to pass in a string array. It will also dynamically discover the parameters for the 
        /// stored procedure and set the parameter's SourceColumns to the strings that are passed in. It does this by mapping 
        /// the parameters to the strings IN ORDER. Thus, order is very important.
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="sourceColumns"><para>The list of DataFields for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        DbCommand GetStoredProcCommandWithSourceColumns(string storedProcedureName,
                                                                        params string[] sourceColumns);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string</exception>
        void LoadDataSet(DbCommand command,
                                         DataSet dataSet,
                                         string tableName);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" /> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        void LoadDataSet(DbCommand command,
                                         DataSet dataSet,
                                         string tableName,
                                         DbTransaction transaction);

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        void LoadDataSet(DbCommand command,
                                         DataSet dataSet,
                                         string[] tableNames);

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/> in  a transaction.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        void LoadDataSet(DbCommand command,
                                         DataSet dataSet,
                                         string[] tableNames,
                                         DbTransaction transaction);

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> with the results returned from a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure name to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        void LoadDataSet(string storedProcedureName,
                                         DataSet dataSet,
                                         string[] tableNames,
                                         params object[] parameterValues);

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> with the results returned from a stored procedure executed in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the stored procedure in.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure name to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        void LoadDataSet(DbTransaction transaction,
                                         string storedProcedureName,
                                         DataSet dataSet,
                                         string[] tableNames,
                                         params object[] parameterValues);

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from command text.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        void LoadDataSet(CommandType commandType,
                                         string commandText,
                                         DataSet dataSet,
                                         string[] tableNames);

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from command text in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        void LoadDataSet(DbTransaction transaction,
                                         CommandType commandType,
                                         string commandText,
                                         DataSet dataSet,
                                         string[] tableNames);

        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        /// <param name="command">The command with the parameter.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        void SetParameterValue(DbCommand command,
                                               string parameterName,
                                               object value);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <param name="updateBatchSize">The number of database commands to execute in a batch.</param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet,
                                          string tableName,
                                          DbCommand insertCommand,
                                          DbCommand updateCommand,
                                          DbCommand deleteCommand,
                                          UpdateBehavior updateBehavior,
                                          int? updateBatchSize);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet,
                                          string tableName,
                                          DbCommand insertCommand,
                                          DbCommand updateCommand,
                                          DbCommand deleteCommand,
                                          UpdateBehavior updateBehavior);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/>.</para></param>
        /// <param name="updateCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/>.</para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/>.</para></param>        
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="updateBatchSize">The number of commands that can be executed in a single call to the database. Set to 0 to
        /// use the largest size the server can handle, 1 to disable batch updates, and anything else to set the number of rows.
        /// </param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet,
                                          string tableName,
                                          DbCommand insertCommand,
                                          DbCommand updateCommand,
                                          DbCommand deleteCommand,
                                          DbTransaction transaction,
                                          int? updateBatchSize);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/>.</para></param>
        /// <param name="updateCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/>.</para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/>.</para></param>        
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet,
                                          string tableName,
                                          DbCommand insertCommand,
                                          DbCommand updateCommand,
                                          DbCommand deleteCommand,
                                          DbTransaction transaction);
    }
}