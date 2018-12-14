using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Infrastructure.Core.RegistryManager
{
    public interface IRegistryManager
    {
        object GetValue(string path, string name);
        Dictionary<string,object> GetValues(string path);

    }
}
