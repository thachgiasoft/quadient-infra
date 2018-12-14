using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Configuration
{
    public class Connection
    {
        public Connection() { }

        public Connection(string databaseName, string connectionString)
        {
            this.DatabaseName = databaseName;
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        /// DEcryption edilrek gonderilecek
        /// Encryipted decyrpted olup olmadigini app.config dosyasinda belirteceksin.
        public string ConnectionString { get; set; }

        public override string ToString()
        {
            return DatabaseName;
        }

        public string ProviderName { get; set; }
    }
}
