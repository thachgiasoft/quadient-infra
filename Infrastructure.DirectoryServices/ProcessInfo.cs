using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DirectoryServices
{
    public enum ProcessInfo
    {
        NoInfo = 0,
        UserAlreadyExistOnActiveDirectory = 1,
        UserAlreadyExistOnMailBox = 2,
        UserAlreadyExistOnLync = 3
    }
}
