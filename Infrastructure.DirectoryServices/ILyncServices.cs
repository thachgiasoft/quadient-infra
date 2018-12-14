namespace Infrastructure.DirectoryServices
{
    public interface ILyncServices
    {
        void CreateNewUserOnLync(string pUserCommonName, ref ProcessInfo pProcessInfo);
    }
}