using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DirectoryServices
{
    public struct UserAccountControlFlags
    {
        public const int SCRIPT = 0x0001;
        public const int ACCOUNTDISABLE = 0x0002;
        public const int HOMEDIR_REQUIRED = 0x0008;
        public const int LOCKOUT = 0x0010;
        public const int PASSWD_NOTREQD = 0x0020;
        public const int PASSWD_CANT_CHANGE = 0x0040;
        public const int ENCRYPTED_TEXT_PWD_ALLOWED = 0x0080;
        public const int TEMP_DUPLICATE_ACCOUNT = 0x0100;
        public const int NORMAL_ACCOUNT = 0x0200;
        public const int INTERDOMAIN_TRUST_ACCOUNT = 0x0800;
        public const int WORKSTATION_TRUST_ACCOUNT = 0x1000;
        public const int SERVER_TRUST_ACCOUNT = 0x2000;
        public const int DONT_EXPIRE_PASSWORD = 0x10000;
        public const int MNS_LOGON_ACCOUNT = 0x20000;
        public const int SMARTCARD_REQUIRED = 0x40000;
        public const int TRUSTED_FOR_DELEGATION = 0x80000;
        public const int NOT_DELEGATED = 0x100000;
        public const int USE_DES_KEY_ONLY = 0x200000;
        public const int DONT_REQ_PREAUTH = 0x400000;
        public const int PASSWORD_EXPIRED = 0x800000;
        public const int TRUSTED_TO_AUTH_FOR_DELEGATION = 0x1000000;
    }
}
