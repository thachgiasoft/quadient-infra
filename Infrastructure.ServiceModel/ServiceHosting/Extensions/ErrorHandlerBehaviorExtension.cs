using System;
using System.Data;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Infrastructure.Core;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Exceptions.Enums;
using Infrastructure.Exceptions.ServiceExceptions;
using Infrastructure.ServiceModel.ServiceLibrary;

namespace Infrastructure.ServiceModel.ServiceHosting.Extensions
{
    public class ErrorHandlerBehavior : ServiceBase, IServiceBehavior, IErrorHandler
    {
        private readonly ILogger _logger;
        public ErrorHandlerBehavior(ILogger logger)
        {
            _logger = logger;
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher chDispatcher in serviceHostBase.ChannelDispatchers)
            {
                chDispatcher.ErrorHandlers.Add(this);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public bool HandleError(Exception error)
        {
            return false; // handled.
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException<CEBakanlikException> fe = ConvertExceptionType(error);
            MessageFault mFault = fe.CreateMessageFault();
            fault = Message.CreateMessage(version, mFault, fe.Action);
        }

        public FaultException<CEBakanlikException> ConvertExceptionType(Exception error)
        {
            CEBakanlikException oError;
            var userId = "-1";
            var clientIp = string.Empty;
            var context = GetServiceContextInfo<DefaultServiceContextInfo>();
            if (context != null && context.UserId != null)
                userId = context.UserId.ToString();
            if (context != null && context.ClientIp != null)
                clientIp = context.ClientIp;
            if (string.IsNullOrEmpty(clientIp))
                clientIp = ClientInfo.ClientIp;//Client dan gelmezse base den al
            var mesaj = error.Message;
            if (error is ApplicationException)
            {
                oError = new CEBakanlikException(EnumExceptionType.ApplicationException, "Uygulama Hatası", "EBakanlikSunucu")
                {
                    InnerExceptionString = error.InnerException != null ? error.InnerException.Message : error.Message
                };
                //handled error lari loglamaya gerek yok
                //_logger.WriteLog(userId, ClientInfo.ServerName, ClientInfo.TypeName, ClientInfo.Action, ClientInfo.ServerName, "Services", mesaj, EventLogEntryType.Error, oError.InnerExceptionString);
            }
            else if (error is DBConcurrencyException)
            {
                oError = new CEBakanlikException(EnumExceptionType.ApplicationException, "Uygulama Hatası", "EBakanlikSunucu")
                {
                    InnerExceptionString = "Üzerinde işlem yaptığınız kayıt başka biri tarafından değiştirilmiş. Veriyi görüntülediğiniz ekranı yenileyerek tekrar deneyiniz."
                };
                _logger.WriteLog(userId, ClientInfo.ServerName, ClientInfo.TypeName, ClientInfo.Action, ClientInfo.ServerName, "Services", mesaj, EventLogEntryType.Error, error.InnerException != null ? error.InnerException.Message : error.Message);
            }
            else
            {
                string hataliSql = error.Data["SQL"] == null ? string.Empty : error.Data["SQL"].ToString();
                string method = error.Data["Method"] == null ? error.TargetSite.ToString() : error.Data["Method"].ToString();
                string hata = error.ToString();

                if (!string.IsNullOrEmpty(hataliSql))
                {
                    hataliSql = "HATALI SQL CUMLESI: \r\n " + hataliSql;
                    if (hata.Length > 0) hata += "\r\n" + hataliSql; else hata = hataliSql;
                }
                string logKey;

                try
                {
                    logKey = _logger.WriteLog(userId, clientIp, ClientInfo.TypeName, ClientInfo.Action, ClientInfo.ServerName, "Services", mesaj, EventLogEntryType.Error, hata);
                }
                catch (Exception exc)
                {
                    string errMsg = string.Format("HATA BİLGİLERİ ALINIRKEN HATA OLUSTU: {0} | ORJINAL HATA: Mesaj: {1} | Hata Detayı: {2} | Bilgiler {3}", exc.ToString(), mesaj, hata,
                        string.Format("{0}-{1}-{2}", ClientInfo.TypeName, ClientInfo.Action, ClientInfo.ServerName));
                    var defaultLogger = EngineContext.Current.Resolve<ILogger>(CoreConsts.DefaultLoggerKey);
                    defaultLogger.WriteLog(userId, clientIp, ClientInfo.TypeName, ClientInfo.Action, ClientInfo.ServerName, "Services", errMsg, EventLogEntryType.Error);
                    logKey = "-3";
                }
                oError = logKey == string.Empty ?
                    new CEBakanlikException(EnumExceptionType.CanceledException, "Log edilmesi istenmeyen hata", "EBakanlikSunucu")
                    : new CEBakanlikException(EnumExceptionType.SystemException, hata, error.Message, "EBakanlikSunucu", method, logKey);
            }

            if (error.Data["SQL"] != null) error.Data.Remove("SQL");
            if (error.Data["Method"] != null) error.Data.Remove("Method");

            var fe = new FaultException<CEBakanlikException>(oError, new FaultReason(oError.ErrorMessage), new FaultCode(oError.ErrorCode));
            return fe;
        }

    }

    public class ErrorHandlerBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ErrorHandlerBehavior); }
        }

        protected override object CreateBehavior()
        {
            if (EngineContext.Current.IsRegistered<ILogger>(ServiceBase.DefaultLoggerKey))
                return new ErrorHandlerBehavior(EngineContext.Current.Resolve<ILogger>(ServiceBase.DefaultLoggerKey));
            return new ErrorHandlerBehavior(EngineContext.Current.Resolve<ILogger>(CoreConsts.DefaultLoggerKey));
        }
    }
}