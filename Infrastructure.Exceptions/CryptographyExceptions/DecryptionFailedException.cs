using System;

namespace Infrastructure.Exceptions.CryptographyExceptions
{
    public class DecryptionFailedException :Exception
    {
         public DecryptionFailedException(string message)
            : base(message)
        {

        }

         public DecryptionFailedException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public string ValueToDecrypt { get; set; }
    }
}
