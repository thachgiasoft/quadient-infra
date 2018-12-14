namespace Infrastructure.ServiceModel
{
    public sealed class DefaultServiceContextInfoProvider : ServiceContextInfoProvider
    {

        public override ServiceContextInfo GetServiceContextInfo()
        {
            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var contextinfo = new DefaultServiceContextInfo() { ClientIp = System.Environment.MachineName };
            if (windowsIdentity != null)
            {
                contextinfo.UserId = windowsIdentity.Name;
            }
            return contextinfo;
        }
    }
}
