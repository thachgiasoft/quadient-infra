using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Cryptography;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Services.SettingService;


namespace Infrastructure.DirectoryServices
{
    public class ActiveDirectoryServices : IActiveDirectoryServices
    {
        private string _ldapDc = string.Empty;
        private string _ldapOu = string.Empty;
        private string _ldapPath = string.Empty;
        private string _activeDirectoryControlUser = string.Empty;
        private string _activeDirectoryControlUserPassword = string.Empty;
        private string _activeDirectorySchemaEntryName = string.Empty;
        private int _passwordExpirePeriod = 90;//default 90 days

        public ActiveDirectoryServices(string ldapDc, string ldapOu, string ldapPath, string userName, string password, string schemeEntryName, int passwordExpirePeriod)
        {
            _ldapDc = ldapDc;
            _ldapOu = ldapOu;
            _ldapPath = ldapPath;
            _activeDirectoryControlUser = userName;
            _activeDirectoryControlUserPassword = password;
            _activeDirectorySchemaEntryName = schemeEntryName;
            _passwordExpirePeriod = passwordExpirePeriod;
        }

        private const string DefaultDomain = "icisleri.gov.tr";

        private string ActiveDirectorySetPath
        {
            get
            {
                string s = ActiveDirectorySearchPath;
                s = string.Format("{0}/{1}/{2}/{3},{4}", s.Split('/').GetValue(0)
                                                                                             , s.Split('/').GetValue(1)
                                                                                             , s.Split('/').GetValue(2)
                                                                                             , _ldapOu
                                                                                             , s.Split('/').GetValue(3));
                return s;
            }
        }

        private string ActiveDirectorySetPathForOu(string pOuName)
        {

            string s = ActiveDirectorySearchPath;
            s = string.Format("{0}/{1}/{2}/{3},{4}", s.Split('/').GetValue(0)
                                                                                         , s.Split('/').GetValue(1)
                                                                                         , s.Split('/').GetValue(2)
                                                                                         , pOuName
                                                                                         , s.Split('/').GetValue(3));
            return s;
        }

        private string GetLdapGroupPath(string pGroupName)
        {
            string s = ActiveDirectorySearchPath;
            string ldapGroupPath = string.Format("{0}/{1}/{2}/{3},{4}", s.Split('/').GetValue(0)
                                                                                         , s.Split('/').GetValue(1)
                                                                                         , s.Split('/').GetValue(2)
                                                                                         , string.Format("cn={0}", pGroupName)
                                                                                         , s.Split('/').GetValue(3));
            return ldapGroupPath;
        }

        public string GetUserPrincipleName(string pUserName)
        {
            return string.Format("{0}@{1}", GetSAMAccountName(pUserName), string.IsNullOrEmpty(_ldapDc) ? DefaultDomain : _ldapDc);
        }

        public string GetSAMAccountName(string pUserName)
        {
            return pUserName.Split('@').GetValue(0).ToString();
        }

        private string ActiveDirectorySearchPath
        {
            get { return _ldapPath; }
        }

        private string ActiveDirectoryControlUserName
        {
            get { return _activeDirectoryControlUser; }
        }

        private string ActiveDirectoryControlUserPassword
        {
            get { return _activeDirectoryControlUserPassword; }
        }

        private string ActiveDirectorySchemaEntryName
        {
            get { return _activeDirectorySchemaEntryName; }
        }

        private DirectoryEntry ActiveDirectoryEntryToSet
        {
            get { return new DirectoryEntry(ActiveDirectorySetPath, ActiveDirectoryControlUserName, ActiveDirectoryControlUserPassword); }
        }

        private DirectoryEntry ActiveDirectoryEntryToSetForOu(string pOuName)
        {
            return new DirectoryEntry(ActiveDirectorySetPathForOu(pOuName+","+_ldapOu), ActiveDirectoryControlUserName, ActiveDirectoryControlUserPassword);
        }

        private DirectoryEntry ActiveDirectoryEntryToSearch
        {
            get { return new DirectoryEntry(ActiveDirectorySearchPath, ActiveDirectoryControlUserName, ActiveDirectoryControlUserPassword); }
        }

        private int PasswordExpirePeriod
        {
            get { return _passwordExpirePeriod; }
        }

        private DirectoryEntry GetUser(string pUserName)
        {
            SearchResult objResult = SearchUser(pUserName);

            if (objResult != null)
            {
                DirectoryEntry entryToUpdate = objResult.GetDirectoryEntry();
                return entryToUpdate;
            }
            else
            {
                ///
                ///Kullanıcı kaydı hatasının nereden geldiğini belli etmek için mesajın sonuna DC tarafından gelen exception "!" isareti, DB tarafından gelen exception ise "." konulur 
                /// </summary>
                throw new ApplicationException("Sistemde kullanıcı kaydınız bulunamadı ! Kullanıcı adınızı ve şifrenizi kontrol ediniz. Sorun devam ederse, lütfen sistem yöneticiniz ile görüşünüz!");
            }
        }

        public SearchResult SearchUser(string pUserName)
        {
            var sAMAccountName = GetSAMAccountName(pUserName);
            sAMAccountName = Microsoft.Security.Application.Encoder.LdapFilterEncode(sAMAccountName);

            using (var objRootEntry = ActiveDirectoryEntryToSearch)
            {
                using (var objAdSearcher = new DirectorySearcher(objRootEntry))
                {
                    objAdSearcher.Filter = string.Format("(sAMAccountName={0})", sAMAccountName);
                    objAdSearcher.PropertiesToLoad.Add("cn");
                    return objAdSearcher.FindOne();
                }
            }
        }

        private SearchResult SearchGroup(string pGroupName)
        {
            SearchResult objResult;
            using (var objRootEntry = ActiveDirectoryEntryToSearch)
            {
                using (var objAdSearcher = new DirectorySearcher(objRootEntry))
                {
                    pGroupName = Microsoft.Security.Application.Encoder.LdapFilterEncode(pGroupName);
                    objAdSearcher.Filter = string.Format("(&(objectClass=group)(cn={0}))", pGroupName);
                    objResult = objAdSearcher.FindOne();
                    return objResult;
                }
            }


            throw new ApplicationException("Grup adı sistemde bulunamadı!");
        }

        private List<string> SearchOrganizationalUnit()
        {
            List<string> orgUnits = new List<string>();
            using (var objRootEntry = ActiveDirectoryEntryToSearch)
            {
                using (var objAdSearcher = new DirectorySearcher(objRootEntry))
                {
                    objAdSearcher.Filter = "(objectCategory=organizationalUnit)";
                    foreach (SearchResult res in objAdSearcher.FindAll())
                    {
                        orgUnits.Add(res.Path);
                    }
                }
            }

            return orgUnits;
        }

        #region User Account Methods
        public void DeleteUserOnActiveDirectory(string pUserName)
        {
            DirectoryEntry adUserFolder = ActiveDirectoryEntryToSet;
            SearchResult objResult = SearchUser(pUserName);

            if (objResult != null)
            {
                DirectoryEntry user = objResult.GetDirectoryEntry();
                adUserFolder.Children.Remove(user);
                adUserFolder.CommitChanges();
                adUserFolder.Close();
            }
        }

        public void SetNewPassword(string pUserName, string pPassword)
        {
            var user = GetUser(pUserName);

            try
            {
                var ret = user.Invoke("SetPassword", pPassword);
                var val = (int)user.Properties["userAccountControl"].Value;
                val = val & ~UserAccountControlFlags.PASSWD_NOTREQD;
                val = val & ~UserAccountControlFlags.LOCKOUT;
                user.Properties["userAccountControl"].Value = val;
                user.CommitChanges();
            }
            catch (TargetInvocationException exc)
            {
                if (exc.InnerException.ToString().Contains("0x80072035"))	//The server is unwilling to process the request. (Exception from HRESULT: 0x80072035)
                {
                    throw new ApplicationException(@"<b>Aktif Dizin hata döndürdü !</b><br /><br /> 
													Muhtemelen, yeni şifreniz, şifre belirleme politikalarına uygun olmadığı için bu hata oluştu.<br />
													Lütfen şifre belirleme kurallarına dikkat ederek tekrar deneyiniz. 
													Sorun devam ederse proje yöneticinize veya çağrı merkezine bildiriniz.<br /><br />
													Şifrenizde, en az <b>1 adet harf</b>, en az <b>1 adet rakam</b> ve en az <b>1 adet sembol</b> olduğundan,
													en az <b>8 karakter</b>den oluştuğundan, adınız veya soyadınızı <u>içermediğinden</u> ve ardışık harf veya rakam dizilerinden <u>oluşmadığından</u> emin olunuz.");
                }
                throw;
            }
            finally
            {
                user.Close();
            }
        }

        public bool ChangeExpiredPasswordAndAuthorizeUserOnActiveDirectory(string pUserName, string pOldPasword, string pNewPasword)
        {
            if (IsPasswordExpired(pUserName))
            {
                var user = GetUser(pUserName);
                try
                {
                    var result = user.Invoke("ChangePassword", new object[] { pOldPasword, pNewPasword });
                    if (result == null)
                        return IsUserAuthorizedOnActiveDirectory(pUserName, pNewPasword);
                }
                catch (Exception exc)
                {
                    return false;
                }
            }
            return true;
        }

        public void EnableUserAccount(string pUserName)
        {
            var user = GetUser(pUserName);
            var val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val & ~UserAccountControlFlags.ACCOUNTDISABLE;

            user.CommitChanges();
            user.Close();
        }

        public void DisableUserAccount(string pUserName)
        {
            var user = GetUser(pUserName);
            var val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val | UserAccountControlFlags.ACCOUNTDISABLE;

            user.CommitChanges();
            user.Close();

            //Exchange pasif yap!!!!!!!!!!!!!!!!!!!
            /*****************************************************************/
        }

        public void LockUserAccount(string pUserName)
        {
            var user = GetUser(pUserName);
            var val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val | UserAccountControlFlags.LOCKOUT;

            user.CommitChanges();
            user.Close();
        }

        public void UnlockUserAccount(string pUserName)
        {
            var user = GetUser(pUserName);
            var val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val & ~UserAccountControlFlags.LOCKOUT;

            user.CommitChanges();
            user.Close();
        }

        public void MakePasswordNotRequired(string pUserName) // 544 - Account Enabled - Require user to change password at first logon
        {
            var user = GetUser(pUserName);
            var val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val | UserAccountControlFlags.PASSWD_NOTREQD;

            user.CommitChanges();
            user.Close();
        }

        public void MakePasswordRequired(string pUserName) // 32 - passwd_notreqd
        {
            var user = GetUser(pUserName);
            var val = (int)user.Properties["userAccountControl"].Value;
            user.Properties["userAccountControl"].Value = val & ~UserAccountControlFlags.PASSWD_NOTREQD;

            user.CommitChanges();
            user.Close();
        }

        #endregion

        #region Check Methods

        public bool IsUserExistOnActiveDirectory(string pUserName)
        {
            var objResult = SearchUser(pUserName);
            return (objResult != null ? true : false);
        }

        public bool IsUserActiveOnActiveDirectory(string pUserName)
        {

            var objResult = SearchUser(pUserName);
            if (objResult == null) return false; // User Not Found

            var user = objResult.GetDirectoryEntry();

            if (IsUserDisabled(user)) return false; // User Disabled
            if (IsUserLocked(user)) return false; // User Locked

            user.Close();
            return true;
        }

        public bool IsUserAuthorizedOnActiveDirectory(string pUserName, string pPasword)
        {
            var sAMAccountName = GetSAMAccountName(pUserName);
            sAMAccountName = Microsoft.Security.Application.Encoder.LdapFilterEncode(sAMAccountName);
            var entry = new DirectoryEntry(ActiveDirectorySearchPath, sAMAccountName, pPasword);

            try
            {
                //Kullanıcı adı ve/veya şifre hatalı ise null kontrolü sırasında hata oluşabilir. 
                //Bu durumda hata log'lanır ve geriye false döndürülür;
                if (entry.Name == null)
                {
                    return false;
                }
                else
                {
                    var userEntry = GetUser(pUserName);
                    if (IsUserDisabled(userEntry)) return false;
                    if (IsUserLocked(userEntry)) return false;
                    return true;
                }
            }
            catch (Exception exc)
            {
                //if (!(exc is System.Runtime.InteropServices.COMException) || ((System.Runtime.InteropServices.COMException) exc).ErrorCode == 2147943726) {
                //	string errMsg = string.Format("Login Hatası Oluştu: {0}", exc.ToString());
                //	EventLog.WriteEntry("Application", exc.ToString(), EventLogEntryType.Error, 0);
                //}
                if (!exc.Message.Contains("Logon failure"))
                {
                    string errMsg = string.Format("Login Hatası Oluştu: {0}", exc.ToString());
                    //EventLog.WriteEntry("Application", exc.ToString(), EventLogEntryType.Error, 0);
                }
                return false;
            }
            finally
            {
                // Close etmeyince eski şifreyi kabul ediyor...
                entry.Close();
            }
        }

        public bool IsUserLocked(DirectoryEntry userEntry)
        {
            if (userEntry.Properties["userAccountControl"] != null && userEntry.Properties["userAccountControl"].Value != null)
            {
                var flags = (int)userEntry.Properties["userAccountControl"].Value;
                var locked = Convert.ToBoolean(flags & UserAccountControlFlags.LOCKOUT);
                if (locked) return true; // User Locked
            }

            return false;
        }

        public bool IsPasswordExpired(string pUserName)
        {
            var user = GetUser(pUserName);

            return IsUserPasswordExpired(user);
        }

        public bool IsUserPasswordExpired(DirectoryEntry userEntry)
        {
            string attrName = "pwdLastSet";

            if (userEntry.Properties[attrName] != null && userEntry.Properties[attrName].Value != null)
            {
                var pls = ConvertADSLargeIntegerToInt64(userEntry.Properties[attrName].Value);
                var increment = (pls - (Int64)47966688000000000) / ((double)600000000);
                var dtPls = new DateTime(1753, 1, 1).AddMinutes(increment);
                var dtPwExpire = dtPls.AddDays(PasswordExpirePeriod);
                if (dtPwExpire < DateTime.Now.Date) return true; //Password Expired
            }

            return false;
        }

        public bool GetUsersIsDisable(string pUserName)
        {
            var user = GetUser(pUserName);

            return IsUserDisabled(user);
        }

        public bool IsUserDisabled(DirectoryEntry userEntry)
        {
            if (userEntry.Properties["userAccountControl"] != null && userEntry.Properties["userAccountControl"].Value != null)
            {
                var flags = (int)userEntry.Properties["userAccountControl"].Value;
                var disabled = Convert.ToBoolean(flags & UserAccountControlFlags.ACCOUNTDISABLE);
                if (disabled) return true; // User Disabled
            }

            return false;
        }

        public byte CheckAccountStatusForLogin(string pUserName, bool pCertificateLogin)
        {
            var objResult = SearchUser(pUserName);
            if (objResult == null) return 10; // User Not Found

            var user = objResult.GetDirectoryEntry();

            if (IsUserDisabled(user)) return 20; // User Disabled

            if (IsUserLocked(user)) return 30; // User Locked

            if (!pCertificateLogin && IsUserPasswordExpired(user)) return 40; //Password Expired

            user.Close();

            return 0;
        }

        public bool CheckOrganizationalUnitName(string pOuName)
        {
            bool re = false;
            var listOfOu = SearchOrganizationalUnit();
            if (listOfOu.Count > 0)
            {
                if (listOfOu.Any(str => str.Contains(string.Format("OU={0}", pOuName))))
                    re = true;
            }

            return re;
        }

        #endregion

        #region Get Select Methods

        public int GetBadPwdCount(string pUserName)
        {
            var user = GetUser(pUserName);
            var durum = (int?)user.Properties["badpwdcount"].Value;

            if (durum != null)
            {
                int val = (int)user.Properties["badpwdcount"].Value;
                user.Close();
                return val;
            }
            else
                return 0;
        }

        public long GetUsersLockStatus(string pUserName)
        {
            var user = GetUser(pUserName);
            var val = ConvertADSLargeIntegerToInt64(user.Properties["lockoutTime"].Value);

            return val;
        }

        public DirectoryEntry GetUserEntry(string userName)
        {
            var userEntry = GetUser(userName);

            return userEntry;
        }

        public SearchResult GetSearchResult(string userName)
        {
            var searchResult = SearchUser(userName);

            return searchResult;
        }

        public DirectoryEntry GetActiveDirectoryEntryToSet()
        {
            return ActiveDirectoryEntryToSet;
        }

        public DirectoryEntry GetActiveDirectoryEntryToSetForOu(string pOuName)
        {
            return ActiveDirectoryEntryToSetForOu(pOuName);
        }

        public string GetActiveDirectorySchemaEntryName()
        {
            return ActiveDirectorySchemaEntryName;
        }

        public int PasswordExpirationRemainingDays(string pUserName)
        {
            var user = GetUser(pUserName);
            var attrName = "pwdLastSet";

            if (user != null && user.Properties[attrName] != null && user.Properties[attrName].Value != null)
            {
                var pls = ConvertADSLargeIntegerToInt64(user.Properties[attrName].Value);
                var increment = (pls - (Int64)47966688000000000) / ((double)600000000);
                var dtPls = new DateTime(1753, 1, 1).AddMinutes(increment);
                var dtPwExpire = dtPls.AddDays(PasswordExpirePeriod);
                return (dtPwExpire - DateTime.Now.Date).Days;
            }

            return 1000;
        }

        private static long ConvertADSLargeIntegerToInt64(object pAdsLargeInteger)
        {
            if (pAdsLargeInteger == null) return 0;

            var highPart = (Int32?)pAdsLargeInteger.GetType().InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, pAdsLargeInteger, null);
            var lowPart = (Int32?)pAdsLargeInteger.GetType().InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, pAdsLargeInteger, null);

            return (Int32)highPart * ((Int64)UInt32.MaxValue + 1) + (Int32)lowPart;
        }

        private static DateTime? ConvertADSLargeIntegerToDateTime(object adsLargeInteger)
        {
            if (adsLargeInteger == null) return null;
            var pInt64 = ConvertADSLargeIntegerToInt64(adsLargeInteger);
            var increment = (pInt64 - (Int64)47966688000000000) / ((double)600000000);
            var dtPls = new DateTime(1753, 1, 1).AddMinutes(increment);
            return dtPls;
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
        #endregion

        #region Group Methods

        public void CreateNewGroup(string pGroupName)
        {
            string ldapGroupPath = GetLdapGroupPath(pGroupName);

            if (!DirectoryEntry.Exists(ldapGroupPath))
            {
                using (DirectoryEntry entry = ActiveDirectoryEntryToSet)
                {
                    using (DirectoryEntry group = entry.Children.Add(ldapGroupPath, "group"))
                    {
                        group.Properties["sAmAccountName"].Value = pGroupName;
                        group.CommitChanges();
                    }
                }
            }
            else
            {
                throw new ApplicationException("Grup adı sistemde zaten var!");
            }
        }

        public void DeleteGroup(string pGroupName)
        {
            using (DirectoryEntry entry = ActiveDirectoryEntryToSet)
            {
                using (DirectoryEntry group = SearchGroup(pGroupName).GetDirectoryEntry())
                {
                    entry.Children.Remove(group);
                    group.CommitChanges();
                }
            }
        }

        public void AddUserToGroup(string pUserName, string pGroupName)
        {
            using (DirectoryEntry dirEntry = SearchGroup(pGroupName).GetDirectoryEntry())
            {
                var user = GetUser(pUserName);
                dirEntry.Invoke("Add", new object[] { user.Path.ToString() });
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
        }

        public void RemoveUserFromGroup(string pUserName, string pGroupName)
        {
            using (DirectoryEntry dirEntry = SearchGroup(pGroupName).GetDirectoryEntry())
            {
                var user = GetUser(pUserName);
                dirEntry.Invoke("Remove", new object[] { user.Path.ToString() });
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
        }

        #endregion

    }
}
