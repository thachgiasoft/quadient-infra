using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DistributedCaching;
using Tangosol.IO.Pof;
using Tangosol.Net;

namespace Infrastructure.Console
{
    internal class Program
    {
        // private reCoherenceDistributedCacheManager _distributedCacheManager

        private static void Main(string[] args)
        {
           var props = typeof(Person).GetType().GetProperties();
            foreach (var prop in props)
            {
                var typeName = prop.PropertyType.FullName;
            }

            //EngineContext.Initialize(false);
            //var oracledb = EngineContext.Current.Resolve<IDatabaseLite>(Consts.DefaultDatabaseKey + "Oracle");
            //var cmd = oracledb.GetSqlStringCommand("select * from OLAY");
            //var dt = oracledb.LoadDataTable<DataTable>(cmd);
            //var sqldb = EngineContext.Current.Resolve<IDatabaseLite>(Consts.DefaultDatabaseKey);
            //var sqlreadonlydb = EngineContext.Current.Resolve<IDatabaseLite>(Consts.ReadOnlyDatabaseKey);
            //  INamedCache cache = CacheFactory.GetCache("dist-extend");
            //var manager = new Infrastructure.DistributedCaching.CoherenceDistributedCacheManager();
        }

        //public void SetPersonObjectToCache()
        //{
        //    var p = new Person { Name = "erman", Surname = "bas" };
        //    _distributedCacheManager.Set(CacheTestKey, p);
        //    _distributedCacheManager.Remove(CacheTestKey);
        //}
    }

    public class Person : IPortableObject
    {
        private string ad;
        private string soyad;


        public string Name
        {
            get { return ad; }
            set { ad = value; }
        }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string Surname
        {
            get { return soyad; }
            set { soyad = value; }
        }

        public void ReadExternal(IPofReader reader)
        {
            ad = reader.ReadString(0);
            soyad = reader.ReadString(1);
        }

        /// <see cref="IPortableObject"/>
        public void WriteExternal(IPofWriter writer)
        {
            writer.WriteString(0, ad);
            writer.WriteString(1, soyad);
        }

        public override int GetHashCode()
        {
            return (Name == null ? 0 : Name.GetHashCode()) ^
                   (Surname == null ? 0 : Surname.GetHashCode());
        }
    }
}
