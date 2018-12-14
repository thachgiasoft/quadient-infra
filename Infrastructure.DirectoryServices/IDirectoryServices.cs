using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DirectoryServices
{
    public interface IDirectoryServices
    {
        void CreateUser(object user);
        T SearchUser<T>();
        void UpdateUser(object user);
        void DeleteUser(object user);
    }
}
