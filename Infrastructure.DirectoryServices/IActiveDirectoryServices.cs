using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Infrastructure.DirectoryServices
{
    public interface IActiveDirectoryServices
    {
        /// <summary>
        /// DO NOT USE for Authentication!!!
        /// Use only for authenticated user or system task...
        /// This method uses special authorized user credentials, defined in configuration file...
        /// </summary>
        /// <param name="pUserName">Username like user.name@domain.com</param>
        /// <returns>System.DirectoryService SearchResult object, which is nullable</returns>
        SearchResult SearchUser(string pUserName);
        void DeleteUserOnActiveDirectory(string pUserName);
        void SetNewPassword(string pUserName, string pPassword);
        bool IsPasswordExpired(string pUserName);
        int PasswordExpirationRemainingDays(string pUserName);
        bool IsUserExistOnActiveDirectory(string pUserName);
        bool IsUserActiveOnActiveDirectory(string pUserName);
        bool IsUserAuthorizedOnActiveDirectory(string pUserName, string pPasword);
        bool ChangeExpiredPasswordAndAuthorizeUserOnActiveDirectory(string pUserName, string pOldPasword, string pNewPasword);
        void AddUserToGroup(string pUserName, string pGroupName);
        void RemoveUserFromGroup(string pUserName, string pGroupName);
        void DeleteGroup(string pGroupName);
        void CreateNewGroup(string pGroupName);
        void EnableUserAccount(string pUserName);
        void DisableUserAccount(string pUserName);
        void LockUserAccount(string pUserName);
        void UnlockUserAccount(string pUserName);
        void MakePasswordNotRequired(string pUserName); // 544 - Account Enabled - Require user to change password at first logon
        void MakePasswordRequired(string pUserName); // 32 - passwd_notreqd
        int GetBadPwdCount(string pUserName);
        Int64 GetUsersLockStatus(string pUserName);
        bool GetUsersIsDisable(string pUserName);
        byte CheckAccountStatusForLogin(string pUserName, bool pCertificateLogin);
        bool IsUserDisabled(DirectoryEntry userEntry);
        bool IsUserLocked(DirectoryEntry userEntry);
        bool IsUserPasswordExpired(DirectoryEntry userEntry);
        DirectoryEntry GetUserEntry(string userName);
        SearchResult GetSearchResult(string userName);
        DirectoryEntry GetActiveDirectoryEntryToSet();
        string GetActiveDirectorySchemaEntryName();
        string GetUserPrincipleName(string pUserCommonName);
        bool CheckOrganizationalUnitName(string pOuName);
        DirectoryEntry GetActiveDirectoryEntryToSetForOu(string pOuName);
    }
}