using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DirectoryServices
{
    public class LdapServices : IDirectoryServices
    {
        public void CreateUser(object user)
        {
            throw new NotImplementedException();
        }

        public T SearchUser<T>()
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(object user)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(object user)
        {
            throw new NotImplementedException();
        }
    }
}
