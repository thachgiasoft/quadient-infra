namespace Infrastructure.DirectoryServices
{
    public interface ISkypeServices
    {
        void CreateNewUserOnSkype(string pUserCommonName, ref ActiveDirectoryServices.ProcessInfo pProcessInfo);
    }
}