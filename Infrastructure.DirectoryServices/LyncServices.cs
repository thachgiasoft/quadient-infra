using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Services.SettingService;

namespace Infrastructure.DirectoryServices
{
    public class LyncServices : ILyncServices
    {
        private readonly string _lyncConnectionUri = string.Empty;
        private readonly string _lyncRegistryPool = string.Empty;
        public LyncServices(string lyncConnectionUri,string lyncRegistryPool)
        {
            _lyncConnectionUri = lyncConnectionUri;
            _lyncRegistryPool = lyncRegistryPool;
        }

        private string LyncConnectionUri
        {
            get { return _lyncConnectionUri; }
        }
        private string LyncRegistryPool
        {
            get { return _lyncRegistryPool; }
        }

        #region [Lync Server] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public void CreateNewUserOnLync(string pUserCommonName, ref ProcessInfo pProcessInfo)
        {
            Runspace runspace;
            PowerShell powershell;
            PSCommand command;
            var uri = (new Uri(LyncConnectionUri, UriKind.Absolute));

            EngineContext.Current.Resolve<IPowerShellOperations>().GetPSObjects(uri, string.Empty, out runspace, out powershell, out command);

            if (IsLyncAccountAlreadyExist(pUserCommonName, runspace, powershell, command))
            {
                pProcessInfo = ProcessInfo.UserAlreadyExistOnLync;
                return;
            }

            command.AddCommand("Enable-CsUser");
            command.AddParameter("Identity", pUserCommonName);
            command.AddParameter("RegistrarPool", LyncRegistryPool);
            command.AddParameter("SipAddressType", "UserPrincipalName");

            powershell.Commands = command;

            try
            {
                runspace.Open();
                powershell.Runspace = runspace;
                powershell.Invoke();
                PowerShellOperations.CheckError(powershell);
            }
            finally
            {
                runspace.Dispose();
                runspace = null;
                powershell.Dispose();
                powershell = null;
            }
        }

        private bool IsLyncAccountAlreadyExist(string pUserCommonName, Runspace pRunspace, PowerShell pPowerShell, PSCommand pPSCommand)
        {
            pPSCommand.AddCommand("Get-CsUser");
            pPSCommand.AddParameter("Identity", pUserCommonName);

            pPowerShell.Commands = pPSCommand;

            try
            {
                pRunspace.Open();
                pPowerShell.Runspace = pRunspace;
                Collection<PSObject> user = pPowerShell.Invoke();

                if (user == null || user.Count == 0) return false; else return true;
            }
            finally
            {
                pRunspace.Dispose();
                pRunspace = null;
            }
        }

        #endregion
    }
}
