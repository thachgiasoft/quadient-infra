using Infrastructure.Core.Caching;

namespace Infrastructure.ServiceModel
{
    public class ServerNotify
    {
        public ServerNotify(ServerInfo serverInfo)
        {
            ServerInfo = serverInfo;
        }
        public ServerInfo ServerInfo { get; set; }
        public string NotifyType { get; set; }
    }
}
