using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Infrastructure.Core;
using Infrastructure.Core.Helpers;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.Core.TypeFinders;
using Infrastructure.ServiceModel;
using Infrastructure.ServiceModel.DynamicProxy;
using Infrastructure.ServiceModel.ServiceHosting.Behaviors;
using Infrastructure.ServiceModel.ServiceHosting.Extensions;
using Quartz;

namespace Infrastructure.JobScheduling
{
    public class GenericJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap map = context.Trigger.JobDataMap;

            if (map.ContainsKey("endpoint") && map.ContainsKey("method"))
            {
                var endpoint = (string)context.Trigger.JobDataMap["endpoint"];
                var method = (string)context.Trigger.JobDataMap["method"];

                //WCF servisinde yani client tarafinda belirlenen Job tanımının parametreleri buarada context ten alinir.
                //use data passed with trigger - context.Trigger instead of context.JobDetails.JobDataMap

                if (map.ContainsKey("headerData"))
                {
                    var headerData = (string)context.Trigger.JobDataMap["headerData"];

                    if (!string.IsNullOrEmpty(headerData))
                    {
                        var headerInfo = headerData.XmlDeserializeFromString<HeaderData>();
                        //var headerInfo = JsonConvert.DeserializeObject<HeaderData>(headerData);

                        if (map.ContainsKey("operationParams"))
                        {
                            var operationParams = (string)context.Trigger.JobDataMap["operationParams"];

                            if (!string.IsNullOrEmpty(operationParams))
                            {
                                var paramss = operationParams.XmlDeserializeFromString<object[]>();

                                //Dynamic Service Invocation 
                                DynamicServiceInvoke(
                                    endpoint,
                                    method, paramss, headerInfo);
#if DEBUG
                                Console.WriteLine("");
                                Console.WriteLine(
                                    "DynamicServiceInvoke method with parameters is executed endpoint is {0}, method is {1} \n operationParams = \n {2} \n headerData {3} \n  ",
                                    endpoint,
                                    method, operationParams, headerInfo.DataAsXmlString);
                                Console.WriteLine("");
#endif
                            }
                        }
                        else
                        {
                            DynamicServiceInvoke(
                                 endpoint,
                                 method, null, headerInfo);
#if DEBUG
                            Console.WriteLine("");
                            Console.WriteLine(
                                "DynamicServiceInvoke method without parameters is executed endpoint is {0}, method is {1} \n headerData {2} \n ",
                                endpoint,
                                method, headerInfo.DataAsXmlString);
                            Console.WriteLine("");
#endif
                        }
                    }
                    else throw new ApplicationException("Request Header bilgisi alinamadi");
                }
            }
        }

        private static void DynamicServiceInvoke(string wsdlUri, string operationName, object[] operationParameters,
                                                 HeaderData headerData)
        {
            var factory = new DynamicProxyFactory(wsdlUri);
            var serviceContract = factory.Contracts.FirstOrDefault();
            // create the DynamicProxy for the contract  and perform operations

#if DEBUG
            Console.WriteLine("Creating DynamicProxy to GenericJobScheduler Service");
#endif
            if (serviceContract != null)
            {
                DynamicProxy dynamicProxy = factory.CreateProxy(serviceContract.Name);

                var innerChannel = dynamicProxy.GetProperty("InnerChannel") as IClientChannel;
                if (innerChannel != null)
                {
                    using (new OperationContextScope(innerChannel))
                    {
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(headerData.HeaderName, headerData.HeaderNamespace,
                                                                                                       headerData.DataAsXmlString));
                        var result = dynamicProxy.CallMethod(operationName, operationParameters);
                    }
                }
            }
        }
    }
}