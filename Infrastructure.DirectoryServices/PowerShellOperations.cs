using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using Infrastructure.Core.Cryptography;
using Infrastructure.Services.SettingService;

namespace Infrastructure.DirectoryServices
{
    public class PowerShellOperations : IPowerShellOperations
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly ICoreCryptography _coreCryptography;

        public PowerShellOperations(IApplicationSettingService applicationSettingService, ICoreCryptography coreCryptography)
        {
            _applicationSettingService = applicationSettingService;
            _coreCryptography = coreCryptography;
        }

        private string ActiveDirectoryControlUserPassword
        {
            get { return _coreCryptography.Decrypt(this._applicationSettingService.GetSetting<string>("ActiveDirectoryControlUserPassword")); }
        }

        private string ActiveDirectoryControlUserName
        {
            get { return _coreCryptography.Decrypt(this._applicationSettingService.GetSetting<string>("ActiveDirectoryControlUser")); }
        }

        private SecureString GetActiveDirectoryControlPasswordAsSecureString()
        {
            var password = new SecureString();
            foreach (char x in ActiveDirectoryControlUserPassword)
            {
                password.AppendChar(x);
            }
            return password;
        }

        public void GetPSObjects(Uri uri, string shellUri, out Runspace runspace, out PowerShell powershell, out PSCommand command)
        {
            SecureString password = GetActiveDirectoryControlPasswordAsSecureString();
            var credential = new PSCredential(ActiveDirectoryControlUserName, password);
            var connectionInfo = new WSManConnectionInfo(uri, shellUri, credential);
            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            connectionInfo.SkipCACheck = true;
            connectionInfo.SkipCNCheck = true;
            runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            powershell = PowerShell.Create();
            command = new PSCommand();
        }

        public static void CheckError(PowerShell pPowerShell)
        {
            var errorList = new StringBuilder();
            if (pPowerShell.Streams.Error.Count > 0)
            {
                foreach (object item in pPowerShell.Streams.Error.ReadAll())
                {
                    errorList.Append(item.ToString());
                    errorList.Append(System.Environment.NewLine);
                }
                throw new ApplicationException(errorList.ToString());
            }
        }
    }
}
