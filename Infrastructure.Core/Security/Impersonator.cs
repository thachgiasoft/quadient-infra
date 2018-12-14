using System;
using System.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Infrastructure.Core.Security
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class Impersonator : IImpersonator
    {
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;

		WindowsImpersonationContext impersonationContext;

		[DllImport("advapi32.dll")]
		public static extern int LogonUserA(String lpszUserName,
			String lpszDomain,
			String lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			ref IntPtr phToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DuplicateToken(IntPtr hToken,
			int impersonationLevel,
			ref IntPtr hNewToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool RevertToSelf();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern bool CloseHandle(IntPtr handle);

		public void RunImpersonatedOperation(string domain, string userName, string password, Action action)
		{
			if (ImpersonateValidUser(userName, password, domain))
			{
				try
				{
					if (action != null)
					{
						action();
					}
					else
					{
						throw new ApplicationException("Impersonation Failed: Action can not be null.");
					}
				}
				finally
				{
					UndoImpersonation();
				}
			}
			else
			{
				throw new SecurityException("Impersonation Logon Falied.");
			}
		}

		public bool ImpersonateValidUser(string userName, string password, string domain)
		{
			WindowsIdentity tempWindowsIdentity;
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

			if (RevertToSelf())
			{
				if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
					LOGON32_PROVIDER_DEFAULT, ref token) != 0)
				{
					if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
					{
						tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
						impersonationContext = tempWindowsIdentity.Impersonate();
						if (impersonationContext != null)
						{
							CloseHandle(token);
							CloseHandle(tokenDuplicate);
							return true;
						}
					}
				}
			}
			if (token != IntPtr.Zero)
				CloseHandle(token);
			if (tokenDuplicate != IntPtr.Zero)
				CloseHandle(tokenDuplicate);
			return false;
		}

		public void UndoImpersonation()
		{
			if (impersonationContext != null)
				impersonationContext.Undo();
		}

	}
}
