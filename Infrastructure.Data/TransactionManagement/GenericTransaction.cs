using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Infrastructure.Core;

namespace Infrastructure.Data.TransactionManagement
{
    [Serializable]
    public sealed class GenericTransaction : IDbTransaction
    {
        private readonly IDatabase _db;
        private readonly DbConnection _transactionConnection;
        private readonly DbTransaction _transaction;
        private readonly IsolationLevel _isolation;
        private readonly Guid _transactionGuid;
        private readonly bool _isFirst;
        internal GenericTransaction(IDatabase db, IsolationLevel isolationLevel)
        {
            _db = db;
            _transactionGuid = Guid.NewGuid();
            _isolation = isolationLevel;
            if (HybridContext.Current[_db.DatabaseInstanceId] == null)
            {
                _transactionConnection = db.CreateConnection();
                _transactionConnection.Open();
                _transaction = _transactionConnection.BeginTransaction(isolationLevel);
                HybridContext.Current[_db.DatabaseInstanceId] = _transaction;
                _isFirst = true;
            }
        }
        /// <summary>
        /// Başlatılmış olan transactionı commit eder
        /// </summary>
        public void Commit()
        {
            try
            {
                if (_isFirst)
                    _transaction.Commit();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_isFirst)
                {
                    _transactionConnection.Close();
                    HybridContext.Current[_db.DatabaseInstanceId] = null;
                }
            }
        }
        /// <summary>
        /// Başlatılmış olan trasaction rollback olur.
        /// </summary>
        public void Rollback()
        {
            try
            {
                if (_isFirst)
                    _transaction.Rollback();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_isFirst)
                {
                    _transactionConnection.Close();
                    HybridContext.Current[_db.DatabaseInstanceId] = null;
                }
            }
        }
        [XmlIgnore]
        public IDbConnection Connection { get { return _isFirst ? _transactionConnection : (HybridContext.Current[_db.DatabaseInstanceId] == null ? null : ((DbTransaction)HybridContext.Current[_db.DatabaseInstanceId]).Connection); } }
        [XmlIgnore]
        public IsolationLevel IsolationLevel { get { return _isolation; } }
        [XmlIgnore]
        internal DbTransaction Transaction { get { return _isFirst ? _transaction : (HybridContext.Current[_db.DatabaseInstanceId] == null ? null : (DbTransaction)HybridContext.Current[_db.DatabaseInstanceId]); } }
        public Guid TransactionGuid { get { return _transactionGuid; } }

        public void Dispose()
        {
            if (!_isFirst) return;
            _transaction.Dispose();
            _transactionConnection.Dispose();
            HybridContext.Current[_db.DatabaseInstanceId] = null;
        }
        ~GenericTransaction()
        {
            if (!_isFirst) return;
            if (_transaction != null)
                _transaction.Dispose();
            if (_transactionConnection != null)
                _transactionConnection.Dispose();
                HybridContext.Current[_db.DatabaseInstanceId] = null;
        }
    }
}
