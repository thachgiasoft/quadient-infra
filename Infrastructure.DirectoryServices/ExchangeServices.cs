using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Services.SettingService;


namespace Infrastructure.DirectoryServices
{
    public class ExchangeServices : IExchangeServices
    {
        private readonly string _liveIdConnectionUri=string.Empty;
        private readonly string _microsoftExchangeSchema = string.Empty;
        public ExchangeServices(string liveIdConnectionUri,string microsoftExchangeSchema)
        {
            _liveIdConnectionUri = liveIdConnectionUri;
            _microsoftExchangeSchema = microsoftExchangeSchema;
        }

        private static string GetExchangeServerDbName(string pUserAcountName)
        {
            string firstLetter = pUserAcountName.Substring(0, 1);

            switch (firstLetter)
            {
                case "a": return "MailBoxes_A";
                case "b": return "MailBoxes_B";
                case "c": return "MailBoxes_CD";
                case "d": return "MailBoxes_CD";
                case "e": return "MailBoxes_E";
                case "f": return "MailBoxes_F";
                case "g": return "MailBoxes_G";
                case "h": return "MailBoxes_H";
                case "i": return "MailBoxes_I";
                case "j": return "MailBoxes_JKL";
                case "k": return "MailBoxes_JKL";
                case "l": return "MailBoxes_JKL";
                case "m": return "MailBoxes_M";
                case "n": return "MailBoxes_N";
                case "o": return "MailBoxes_O";
                case "p": return "MailBoxes_PR";
                case "r": return "MailBoxes_PR";
                case "s": return "MailBoxes_S";
                case "t": return "MailBoxes_TUVX";
                case "u": return "MailBoxes_TUVX";
                case "v": return "MailBoxes_TUVX";
                case "y": return "MailBoxes_YZ";
                case "z": return "MailBoxes_YZ";
                case "q": return "MailBoxes_O";
                case "w": return "MailBoxes_TUVX";
                case "x": return "MailBoxes_TUVX";
                default:
                    return "MailBoxes_A";
            }
        }

        private string LiveIdConnectionUri
        {
            get { return _liveIdConnectionUri; }
        }

        private string ShellUri
        {
            get { return _microsoftExchangeSchema; }
        }

        #region [Exchange Server] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public void CreateNewUserOnMailBox(string pUserCommonName, string pUserLogonName, ref ProcessInfo pProcessInfo)
        {
            Runspace runspace;
            PowerShell powershell;
            PSCommand command;
            var uri = (new Uri(LiveIdConnectionUri, UriKind.Absolute));

            EngineContext.Current.Resolve<IPowerShellOperations>().GetPSObjects(uri, ShellUri, out runspace, out powershell, out command);

            if (IsMailBoxAlreadyExist(pUserCommonName, runspace, powershell, command))
            {
                pProcessInfo = ProcessInfo.UserAlreadyExistOnMailBox;
                return;
            }

            command.AddCommand("Enable-Mailbox");
            command.AddParameter("Identity", pUserCommonName);
            command.AddParameter("Alias", pUserLogonName);
            command.AddParameter("Database", GetExchangeServerDbName(pUserLogonName));

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

        public bool IsMailBoxAlreadyExist(string pUserCommonName, Runspace pRunspace, PowerShell pPowerShell, PSCommand pPSCommand)
        {
            pPSCommand.AddCommand("Get-Mailbox");
            pPSCommand.AddParameter("Identity", pUserCommonName);

            pPowerShell.Commands = pPSCommand;

            try
            {
                pRunspace.Open();
                pPowerShell.Runspace = pRunspace;
                Collection<PSObject> user = pPowerShell.Invoke();

                if (user == null || user.Count == 0) return false;

                PowerShellOperations.CheckError(pPowerShell);

                PSMemberInfo item = user[0].Properties.SingleOrDefault(property => property.Name == "RecipientType");

                if (item != null)
                {
                    if (string.Equals(item.Value.ToString(), "UserMailbox", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                pRunspace.Dispose();
                pRunspace = null;
            }
            return false;
        }

        #endregion
    }
}
