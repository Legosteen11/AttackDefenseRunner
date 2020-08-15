using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AttackDefenseRunner.Hubs
{
    public class MonitorHub : Hub
    {
        public async Task SendInfo(string key, string value)
        {
            await Clients.All.SendAsync("ReceiveInfo", key, value);
        }
    }
}