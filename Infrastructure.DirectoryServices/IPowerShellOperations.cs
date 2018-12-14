using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Infrastructure.DirectoryServices
{
    public interface IPowerShellOperations
    {
        void GetPSObjects(Uri uri, string shellUri, out Runspace runspace, out PowerShell powershell, out PSCommand command);
    }
}