using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Infrastructure.DirectoryServices
{
    public interface IExchangeServices
    {
        void CreateNewUserOnMailBox(string pUserCommonName, string pUserLogonName, ref ProcessInfo pProcessInfo);
        bool IsMailBoxAlreadyExist(string pUserCommonName, Runspace pRunspace, PowerShell pPowerShell, PSCommand pPSCommand);
    }
}