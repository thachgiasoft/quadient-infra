using System;
using System.Runtime.Serialization;
using Infrastructure.Exceptions.Enums;

namespace Infrastructure.Exceptions.ServiceExceptions
{
    [DataContract]
    public class CEBakanlikException
    {
        private EnumExceptionType mExType;
        private string _errorMessage;
        private string _errorCode;
        private string mInnerExceptionString;
        private string mErrorMethodName;
        private string mLogKey;
        private string mErrorDetail;
        [DataMember]
        public EnumExceptionType ExType
        {
            get { return mExType; }
            set { mExType = value; }
        }
        [DataMember]
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
        [DataMember]
        public string ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }
        [DataMember]
        public string InnerExceptionString
        {
            get { return mInnerExceptionString; }
            set { mInnerExceptionString = value; }
        }
        [DataMember]
        public string ErrorMethodName
        {
            get { return mErrorMethodName; }
            set { mErrorMethodName = value; }
        }
        [DataMember]
        public string LogKey
        {
            get { return mLogKey; }
            set { mLogKey = value; }
        }
        [DataMember]
        public string ErrorDetail
        {
            get { return mErrorDetail; }
            set { mErrorDetail = value; }
        }

        public CEBakanlikException()
        {
            CleanParameters();
            this.mExType = EnumExceptionType.SystemException;
            this._errorMessage = "Hata Oluştu";
            this._errorCode = "SistemHatası";
        }
        public CEBakanlikException(EnumExceptionType pExceptionType, string pErrorMessage, string pErrorCode)
        {
            CleanParameters();
            this.mExType = pExceptionType;
            this._errorMessage = pErrorMessage;
            this._errorCode = pErrorCode;
        }
        public CEBakanlikException(EnumExceptionType pExceptionType, string pErrorDetail, string pErrorMessage, string pErrorCode, string pErrorMethodName, string pLogKey)
        {
            CleanParameters();
            this.mExType = pExceptionType;
            this.mErrorDetail = pErrorDetail;
            this._errorMessage = pErrorMessage;
            this._errorCode = pErrorCode;
            this.mErrorMethodName = pErrorMethodName;
            this.mLogKey = pLogKey;
        }

        private void CleanParameters()
        {
            _errorMessage = string.Empty;
            _errorCode = string.Empty;
            mInnerExceptionString = string.Empty;
            mErrorMethodName = string.Empty;
            mLogKey = string.Empty;
            mErrorDetail = string.Empty;
        }
    }
}
