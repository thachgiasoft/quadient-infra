using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Infrastructure.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution
    /// </summary>
    [Serializable]
    public class CoreException : Exception
    {
        private readonly string _errorCode;
        private readonly SourceLevels _level;

        public string ErrorCode { get { return _errorCode; } }
        public SourceLevels SourceLevel { get { return _level; } }

        public string FormattedMessage
        {
            get
            {
                switch (SourceLevel)
                {
                    case SourceLevels.Error:
                        return String.Format("Hata: {0} Hata Kodu: {1}", Message, ErrorCode);
                    case SourceLevels.Warning:
                        return String.Format("Uyarı: {0} Uyarı Kodu: {1}", Message, ErrorCode);
                    case SourceLevels.Information:
                        return String.Format("Bilgi: {0} Bilgi Kodu: {1}", Message, ErrorCode);
                    default:
                        return Message;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        public CoreException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CoreException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message.
        /// </summary>
        /// <param name="messageFormat">The exception message format.</param>
        /// <param name="args">The exception message arguments.</param>
        public CoreException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        protected CoreException(SerializationInfo
                                     info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CoreException(Exception innerException, string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args), innerException)
        {
        }

        /// <summary>
        /// Hata kodu her uygulama için standarttır örn: TankKart için ilk hata kodu şu şekilde olmalıdır TKC001. 
        /// Yani hatanın oluştuğu controllerın büyük harfleri TankKartController daki TKC hata kodunun ilk kısmını oluşturur, 
        /// ikinci kısım ise numarasıdır buda 001 ile 999 arasında olmalıdır.
        /// Source levels ise: SourceLevels.Information, SourceLevels.Error, SourceLevels.Warning şeklinde verilebilir.
        /// Bu durumlara göre, ilgili messagebox tetiklenecektir.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="level"></param>
        public CoreException(string message, string errorCode, SourceLevels level)
            : base(message)
        {
            _errorCode = errorCode;
            _level = level;
        }

        public CoreException(Exception innerException, string message, string errorCode, SourceLevels level)
            : base(message, innerException)
        {
            _errorCode = errorCode;
            _level = level;
        }
    }
}
