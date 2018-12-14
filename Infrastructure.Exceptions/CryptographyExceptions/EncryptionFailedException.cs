using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions.CryptographyExceptions
{
    public class EncryptionFailedException : Exception
    {
        public EncryptionFailedException(string message)
            : base(message)
        {

        }

        public EncryptionFailedException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public string ValueToEncrypt { get; set; }
    }
}
