using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Infrastructure.ServiceModel.ServiceHosting.Behaviors;

namespace Infrastructure.ServiceModel
{
    public class DynamicServiceInvokeEventArgs : EventArgs
    {
        public string WsdlUri { get; set; }
        public string OperationName { get; set; }
        public string ContractName { get; set; }
    }
    [Serializable]
    public class ServiceInvokeResult
    {
        public long ResponseTime { get; set; }
        public string ResultDescription { get; set; }
        public object Result { get; set; }
        public string Address { get; set; }
        public string BindingName { get; set; }
    }
    public class DynamicServiceInvoke
    {
        public delegate void InvokeEventHandler(object sender, DynamicServiceInvokeEventArgs e);
        // For MEX endpoints use a MEX address and a 
        // mexMode of .MetadataExchange
        const MetadataExchangeClientMode MexMode = MetadataExchangeClientMode.HttpGet;
        public InvokeEventHandler InvokeStartEvent;
        public InvokeEventHandler InvokeEndEvent;
        private static readonly ConcurrentDictionary<string, CompilerResults> CompilerResultCache = new ConcurrentDictionary<string, CompilerResults>();
        private static readonly ConcurrentDictionary<string, IEnumerable<ContractDescription>> ContractDescriptionCache = new ConcurrentDictionary<string, IEnumerable<ContractDescription>>();
        private static readonly ConcurrentDictionary<string, Dictionary<string, IEnumerable<ServiceEndpoint>>> ServiceEndpointCache = new ConcurrentDictionary<string, Dictionary<string, IEnumerable<ServiceEndpoint>>>();
        public IList<ServiceInvokeResult> InvokeOperation(string wsdlUri, string operationName)
        {
            return InvokeOperation(wsdlUri, null, operationName);
        }
        public IList<ServiceInvokeResult> InvokeOperation(string wsdlUri, string operationName, object[] operationParameters)
        {
            return InvokeOperation(wsdlUri, null, operationName, operationParameters);
        }

        public IList<ServiceInvokeResult> InvokeOperation(string wsdlUri, string contractName, string operationName)
        {
            var key = string.Format("{0}_{1}_{2}", wsdlUri, contractName, operationName);
            CompilerResults results = null;
            var compilerResult = CompilerResultCache.TryGetValue(key, out results);
            IEnumerable<ContractDescription> contracts = null;
            ContractDescriptionCache.TryGetValue(key, out contracts);
            Dictionary<string, IEnumerable<ServiceEndpoint>> endpointsForContracts = null;
            ServiceEndpointCache.TryGetValue(key, out endpointsForContracts);
            if (endpointsForContracts == null)
                endpointsForContracts = new Dictionary<string, IEnumerable<ServiceEndpoint>>();

            var resultList = new List<ServiceInvokeResult>();

            if (!compilerResult)
            {
                // Define the metadata address, contract name, operation name, 
                // and parameters. 
                // You can choose between MEX endpoint and HTTP GET by 
                // changing the address and enum value.
                var mexAddress = new Uri(wsdlUri);


                //var contractName = GetInterfaceFromUri(mexAddress.AbsoluteUri);
                // const string operationName = "Ping";
                // var operationParameters = new object[] { null };

                // Get the metadata file from the service.
                var mexClient = new MetadataExchangeClient(mexAddress, MexMode) { ResolveMetadataReferences = true };
                MetadataSet metaSet = mexClient.GetMetadata();

                // Import all contracts and endpoints
                var importer = new WsdlImporter(metaSet);
                contracts = importer.ImportAllContracts();
                ContractDescriptionCache.TryAdd(key, contracts);

                var allEndpoints = importer.ImportAllEndpoints();

                //ServisKullaniminiLogla metodu icin header bilgilerinin alinmasini saglar.

                
                foreach (var endpoint in allEndpoints)
                {
                    endpoint.Behaviors.Add(new BackendClientMessageInspectorBehavior());
                }

                // Generate type information for each contract
                var generator = new ServiceContractGenerator();

                foreach (ContractDescription contract in contracts)
                {
                    generator.GenerateServiceContractType(contract);
                    // Keep a list of each contract's endpoints
                    endpointsForContracts[contract.Name] = allEndpoints.Where(
                        se => se.Contract.Name == contract.Name).ToList();
                }

                if (generator.Errors.Count != 0)
                    throw new Exception("There were errors during code compilation.");

                ServiceEndpointCache.TryAdd(key, endpointsForContracts);

                // Generate a code file for the contracts 
                var options = new CodeGeneratorOptions { BracingStyle = "C" };
                var codeDomProvider = CodeDomProvider.CreateProvider("C#");

                // Compile the code file to an in-memory assembly
                // Don't forget to add all WCF-related assemblies as references
                var compilerParameters = new CompilerParameters(
                    new[]
                        {
                            "System.dll", "System.ServiceModel.dll",
                            "System.Runtime.Serialization.dll"
                        });
                compilerParameters.GenerateInMemory = true;
                results = codeDomProvider.CompileAssemblyFromDom(compilerParameters, generator.TargetCompileUnit);
                CompilerResultCache.TryAdd(key, results);
            }

            if (results.Errors.Count > 0)
            {
                throw new Exception("There were errors during generated code compilation");
            }
            else
            {
                // Find the proxy type that was generated for the specified contract
                // (identified by a class that implements 
                // the contract and ICommunicationbject)
                if (string.IsNullOrEmpty(contractName))
                    contractName = contracts.First().Name;
                Type clientProxyType = results.CompiledAssembly.GetTypes().First(
                    t => t.IsClass &&
                         t.GetInterface(contractName) != null &&
                         t.GetInterface(typeof(ICommunicationObject).Name) != null);
                if (clientProxyType == null) throw new ArgumentNullException(@"clientProxyType nesnesi oluşturulamadı.");

                // Get the first service endpoint for the contract
                var se = endpointsForContracts[contractName].FirstOrDefault(e => e.Address.Uri.Scheme == "net.tcp") ?? //net.tcp varsa once o kullanilsin cunku daha hizli.
                         endpointsForContracts[contractName].FirstOrDefault();

                // Create an instance of the proxy
                // Pass the endpoint's binding and address as parameters
                // to the ctor
                object instance = results.CompiledAssembly.CreateInstance(
                    clientProxyType.Name,
                    false,
                    System.Reflection.BindingFlags.CreateInstance,
                    null,
                    new object[] { se.Binding, se.Address },
                    CultureInfo.CurrentCulture, null);

                // Get the operation's mereflecthod, invoke it, and get the return value
                if (instance != null)
                {
                    var minfo = instance.GetType().GetMethod(operationName);
                    if (minfo == null) throw new ArgumentNullException(string.Format("{0} adlı operation bulunamadı.", operationName));
                    var watch = new Stopwatch();
                    watch.Start();
                    if (InvokeStartEvent != null)
                        InvokeStartEvent(this, new DynamicServiceInvokeEventArgs() { ContractName = contractName, OperationName = operationName, WsdlUri = wsdlUri });
                    var result = minfo.Invoke(instance, null);
                    watch.Stop();
                    if (InvokeEndEvent != null)
                        InvokeEndEvent(this, new DynamicServiceInvokeEventArgs() { ContractName = contractName, OperationName = operationName, WsdlUri = wsdlUri });
                    var item = new ServiceInvokeResult();
                    item.ResponseTime = watch.ElapsedMilliseconds;
                    item.ResultDescription = "Success";
                    item.Result = result;
                    item.Address = se.Address.Uri.AbsoluteUri;
                    item.BindingName = se.Binding.Name;
                    resultList.Add(item);
                    CloseChannel(instance);
                }
                else
                {
                    throw new ArgumentNullException(string.Format("{0} adlı instance oluşturulamadı.", clientProxyType.Name));
                }

            }
            return resultList;
        }

        


        public IList<ServiceInvokeResult> InvokeOperation(string wsdlUri, string contractName, string operationName, object[] operationParameters)
        {
            var key = string.Format("{0}_{1}_{2}", wsdlUri, contractName, operationName);
            CompilerResults results = null;
            var compilerResult = CompilerResultCache.TryGetValue(key, out results);
            IEnumerable<ContractDescription> contracts = null;
            ContractDescriptionCache.TryGetValue(key, out contracts);
            Dictionary<string, IEnumerable<ServiceEndpoint>> endpointsForContracts = null;

            ServiceEndpointCache.TryGetValue(key, out endpointsForContracts);
            if (endpointsForContracts == null)
                endpointsForContracts = new Dictionary<string, IEnumerable<ServiceEndpoint>>();

            var resultList = new List<ServiceInvokeResult>();

            if (!compilerResult)
            {
                // Define the metadata address, contract name, operation name, 
                // and parameters. 
                // You can choose between MEX endpoint and HTTP GET by 
                // changing the address and enum value.
                var mexAddress = new Uri(wsdlUri);


                //var contractName = GetInterfaceFromUri(mexAddress.AbsoluteUri);
                // const string operationName = "Ping";
                // var operationParameters = new object[] { null };

                // Get the metadata file from the service.
                var mexClient = new MetadataExchangeClient(mexAddress, MexMode) { ResolveMetadataReferences = true };
                MetadataSet metaSet = mexClient.GetMetadata();

                // Import all contracts and endpoints
                var importer = new WsdlImporter(metaSet);
                contracts = importer.ImportAllContracts();
                ContractDescriptionCache.TryAdd(key, contracts);
                var allEndpoints = importer.ImportAllEndpoints();

                //ServisKullaniminiLogla metodu icin header bilgilerinin alinmasini saglar.
                foreach (var endpoint in allEndpoints)
                {
                    endpoint.Behaviors.Add(new BackendClientMessageInspectorBehavior());
                }

                // Generate type information for each contract
                var generator = new ServiceContractGenerator();

                foreach (ContractDescription contract in contracts)
                {

                    generator.GenerateServiceContractType(contract);
                    // Keep a list of each contract's endpoints
                    endpointsForContracts[contract.Name] = allEndpoints.Where(
                        se => se.Contract.Name == contract.Name).ToList();
                }

                if (generator.Errors.Count != 0)
                    throw new Exception("There were errors during code compilation.");

                ServiceEndpointCache.TryAdd(key, endpointsForContracts);

                // Generate a code file for the contracts 
                var options = new CodeGeneratorOptions { BracingStyle = "C" };
                var codeDomProvider = CodeDomProvider.CreateProvider("C#");

                // Compile the code file to an in-memory assembly
                // Don't forget to add all WCF-related assemblies as references
                var compilerParameters = new CompilerParameters(
                    new[]
                        {
                            "System.dll", "System.ServiceModel.dll",
                            "System.Runtime.Serialization.dll"
                        });
                compilerParameters.GenerateInMemory = true;
                results = codeDomProvider.CompileAssemblyFromDom(compilerParameters, generator.TargetCompileUnit);
                CompilerResultCache.TryAdd(key, results);
            }

            if (results.Errors.Count > 0)
            {
                throw new Exception("There were errors during generated code compilation");
            }
            else
            {
                // Find the proxy type that was generated for the specified contract
                // (identified by a class that implements 
                // the contract and ICommunicationbject)
                if (string.IsNullOrEmpty(contractName))
                    contractName = contracts.First().Name;
                Type clientProxyType = results.CompiledAssembly.GetTypes().First(
                    t => t.IsClass &&
                         t.GetInterface(contractName) != null &&
                         t.GetInterface(typeof(ICommunicationObject).Name) != null);
                if (clientProxyType == null) throw new ArgumentNullException(@"clientProxyType nesnesi oluşturulamadı.");

                // Get the first service endpoint for the contract
                var se = endpointsForContracts[contractName].FirstOrDefault(e => e.Address.Uri.Scheme == "net.tcp") ?? //net.tcp varsa once o kullanilsin cunku daha hizli.
                         endpointsForContracts[contractName].FirstOrDefault();

                // Create an instance of the proxy
                // Pass the endpoint's binding and address as parameters
                // to the ctor
                object instance = results.CompiledAssembly.CreateInstance(
                    clientProxyType.Name,
                    false,
                    System.Reflection.BindingFlags.CreateInstance,
                    null,
                    new object[] { se.Binding, se.Address },
                    CultureInfo.CurrentCulture, null);

                // Get the operation's mereflecthod, invoke it, and get the return value
                if (instance != null)
                {
                    var minfo = instance.GetType().GetMethod(operationName);
                    if (minfo == null) throw new ArgumentNullException(string.Format("{0} adlı operation bulunamadı.", operationName));
                    var watch = new Stopwatch();
                    watch.Start();
                    if (InvokeStartEvent != null)
                        InvokeStartEvent(this, new DynamicServiceInvokeEventArgs() { ContractName = contractName, OperationName = operationName, WsdlUri = wsdlUri });

                    var result = minfo.Invoke(instance, operationParameters);

                    watch.Stop();
                    if (InvokeEndEvent != null)
                        InvokeEndEvent(this, new DynamicServiceInvokeEventArgs() { ContractName = contractName, OperationName = operationName, WsdlUri = wsdlUri });
                    var item = new ServiceInvokeResult();
                    item.ResponseTime = watch.ElapsedMilliseconds;
                    item.ResultDescription = "Success";
                    item.Result = result;
                    item.Address = se.Address.Uri.AbsoluteUri;
                    item.BindingName = se.Binding.Name;
                    resultList.Add(item);
                    CloseChannel(instance);
                }
                else
                {
                    throw new ArgumentNullException(string.Format("{0} adlı instance oluşturulamadı.", clientProxyType.Name));
                }

            }
            return resultList;
        }


        private void CloseChannel(object channel)
        {
            var channel2 = channel as ICommunicationObject;
            if (channel2 == null) return;
            try
            {
                if (channel2.State == CommunicationState.Faulted)
                {
                    channel2.Abort();
                }
                else
                {
                    channel2.Close();
                }
            }
            catch (TimeoutException)
            {
                channel2.Abort();
            }
            catch (CommunicationException)
            {
                channel2.Abort();
            }
            catch (Exception)
            {
                channel2.Abort();
                throw;
            }
        }

    }
}
