using MagicOnion.Server.Hubs;
using System;
using System.Threading.Tasks;
using UnityResearch.Service.ApiDefinitions;

namespace UnityResearch.Server.Hubs
{
    public class GerneralHub : StreamingHubBase<IGeneralHub, IGeneralHubReceiver>, IGeneralHub
    {
        private static IGroup _room;
        private const string _roomName = "UnityResearch";

        public async Task SendMessageAsync(string message)
        {
            Console.WriteLine($"SendMessageAsync: {message}");

            await this.Broadcast(_room).OnReceiveMessageAsync(message);
        }

        protected override async ValueTask OnConnecting()
        {
            if (_room == null)
            {
                _room = await this.Group.AddAsync(_roomName);
            }
            else
            {
                await _room.AddAsync(this.Context);
            }
        }

        protected override async ValueTask OnDisconnected()
        {
            await (_room?.RemoveAsync(this.Context) ?? new ValueTask<bool>());
        }
    }
}
