using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Infrastructure.Data.TransactionManagement;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Infrastructure.Data
{
    public interface IDatabaseLite
    {
        Guid DatabaseInstanceId { get; }
        string DatabaseName { get; }
        bool TransactionActive { get; }
        GenericTransaction CurrentTransaction { get; }
        GenericTransaction BeginTransaction();
        DbCommand GetSqlStringCommand();
        DbCommand GetStoredProcCommand(string storedProcedureName);
        DbCommand GetSqlStringCommand(string query);
        SqlCommand GetSqlStringSqlCommand();
        SqlCommand GetSqlStringSqlCommand(string query);
        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        void AddInParameter(DbCommand command, string name, SqlDbType dbType, object value);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>    
        T LoadDataSet<T>(DbCommand command) where T : DataSet;

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// // <param name="tableName">
        /// <para>DataTable Name</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>    
        T LoadDataTable<T>(DbCommand command, string tableName) where T : DataTable;

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>    
        T LoadDataTable<T>(DbCommand command) where T : DataTable;

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataTable"/>.</para>
        /// </summary>
        /// <param name="table"></param>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>    
        void LoadDataTable(DataTable table, DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// // <param name="tableName">
        /// <para>DataTable Name</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>    
        T LoadDataSet<T>(DbCommand command, string tableName) where T : DataSet;
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
                                               GenericTransaction transaction);

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
        DataSet ExecuteDataSet(GenericTransaction transaction,
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
        DataSet ExecuteDataSet(GenericTransaction transaction,
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
                                            GenericTransaction transaction);

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
        int ExecuteNonQuery(GenericTransaction transaction,
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
        int ExecuteNonQuery(GenericTransaction transaction,
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
                                             GenericTransaction transaction);

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
        object ExecuteScalar(GenericTransaction transaction,
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
        object ExecuteScalar(GenericTransaction transaction,
                                             CommandType commandType,
                                             string commandText);


        /// <summary>
        /// Update DataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        int UpdateDataTable(DataTable dataTable);

        /// <summary>
        /// Update DataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        int UpdateDataTable(DataTable dataTable, GenericTransaction tran);

        /// <summary>
        /// Update DataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tran"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        int UpdateDataTable(DataTable dataTable, GenericTransaction tran, int commandTimeout);

        /// <summary>
        /// Update DataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tran"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="acceptChangesDuringUpdate"></param>
        /// <returns></returns>
        int UpdateDataTable(DataTable dataTable, GenericTransaction tran, int commandTimeout, bool acceptChangesDuringUpdate);

        /// <summary>
        /// UpdateDataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        int UpdateDataTable(DataTable dataTable, DbCommand cmd);

        /// <summary>
        /// UpdateDataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="cmd"></param>
        /// <param name="tran">transactional</param>
        /// <returns></returns>
        int UpdateDataTable(DataTable dataTable, DbCommand cmd, GenericTransaction tran);

        /// <summary>
        /// UpdateDataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="cmd"></param>
        /// <param name="tran">transactional</param>
        /// <param name="acceptChangesDuringUpdate"></param>
        /// <returns></returns>
        int UpdateDataTable(DataTable dataTable, DbCommand cmd, GenericTransaction tran, bool acceptChangesDuringUpdate);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="commandTimeout"></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, int commandTimeout);

        /// <summary>
        /// CommandTimeOut 30 by default
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="acceptChanges"></param>
        /// <returns></returns>
        int UpdateDataSet(DataSet dataSet, bool acceptChanges);
        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="commandTimeout"></param>
        /// <param name="acceptChanges"></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, int commandTimeout, bool acceptChanges);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, UpdateBehavior updateBehavior);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <param name="commandTimeout"></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, UpdateBehavior updateBehavior, int commandTimeout);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <param name="commandTimeout"></param>
        /// <param name="acceptChanges"></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, UpdateBehavior updateBehavior, int commandTimeout, bool acceptChanges);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="updateBatchSize">The number of commands that can be executed in a single call to the database. Set to 0 to
        /// use the largest size the server can handle, 1 to disable batch updates, and anything else to set the number of rows.
        /// </param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int? updateBatchSize);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="updateBatchSize">The number of commands that can be executed in a single call to the database. Set to 0 to
        /// use the largest size the server can handle, 1 to disable batch updates, and anything else to set the number of rows.
        /// </param>
        /// <param name="commandTimeout"></param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int? updateBatchSize, int commandTimeout);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="updateBatchSize">The number of commands that can be executed in a single call to the database. Set to 0 to
        /// use the largest size the server can handle, 1 to disable batch updates, and anything else to set the number of rows.
        /// </param>
        /// <param name="commandTimeout"></param>
        /// <param name="acceptChanges"></param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int? updateBatchSize, int commandTimeout, bool acceptChanges);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet, GenericTransaction transaction);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int commandTimeout);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="commandTimeout"></param>
        /// <param name="acceptChanges"></param>
        /// <returns>Number of records affected.</returns>        
        int UpdateDataSet(DataSet dataSet, GenericTransaction transaction, int commandTimeout, bool acceptChanges);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <param name="updateBatchSize">The number of database commands to execute in a batch.</param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <param name="updateBatchSize">The number of database commands to execute in a batch.</param>
        /// <param name="tran">transactional</param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize, GenericTransaction tran);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <param name="updateBatchSize">The number of database commands to execute in a batch.</param>
        /// <param name="tran">transactional</param>
        /// <param name="commandTimeout"></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize, GenericTransaction tran, int commandTimeout);

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <param name="updateBatchSize">The number of database commands to execute in a batch.</param>
        /// <param name="tran">transactional</param>
        /// <param name="commandTimeout"></param>
        /// <param name="acceptChanges"></param>
        /// <returns>number of records affected</returns>        
        int UpdateDataSet(DataSet dataSet, string tableName, UpdateBehavior updateBehavior, int? updateBatchSize, GenericTransaction tran, int commandTimeout, bool acceptChanges);
    }
}
