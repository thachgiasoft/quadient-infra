using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Infrastructure.Data.DatabaseManagement
{
    public interface IDatabaseFactory
    {
        T CreateDatabase<T>(string name) where T : Database;
    }
}
