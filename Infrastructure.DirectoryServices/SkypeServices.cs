using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Services.SettingService;

namespace Infrastructure.DirectoryServices
{
    public class SkypeServices : ISkypeServices
    {
        private readonly IApplicationSettingService _applicationSettingService;

        public SkypeServices(IApplicationSettingService applicationSettingServices)
        {
            _applicationSettingService = applicationSettingServices;
        }

        private string SkypeConnectionUri
        {
            get { return _applicationSettingService.GetSetting<string>("SkypeConnectionUri"); }
        }
        private string SkypeRegistryPool
        {
            get { return _applicationSettingService.GetSetting<string>("SkypeRegistryPool"); }
        }

        #region [Skype Server] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public void CreateNewUserOnSkype(string pUserCommonName, ref ActiveDirectoryServices.ProcessInfo pProcessInfo)
        {
            Runspace runspace;
            PowerShell powershell;
            PSCommand command;
            var uri = (new Uri(SkypeConnectionUri, UriKind.Absolute));

            EngineContext.Current.Resolve<IPowerShellOperations>().GetPSObjects(uri, string.Empty, out runspace, out powershell, out command);

            if (IsSkypeAccountAlreadyExist(pUserCommonName, runspace, powershell, command))
            {
                pProcessInfo = ActiveDirectoryServices.ProcessInfo.UserAlreadyExistOnSkype;
                return;
            }

            command.AddCommand("Enable-CsUser");
            command.AddParameter("Identity", pUserCommonName);
            command.AddParameter("RegistrarPool", SkypeRegistryPool);
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

        private bool IsSkypeAccountAlreadyExist(string pUserCommonName, Runspace pRunspace, PowerShell pPowerShell, PSCommand pPSCommand)
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
