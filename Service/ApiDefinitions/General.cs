using MagicOnion;
using System.Threading.Tasks;

namespace UnityResearch.Service.ApiDefinitions
{
    public interface IGeneralHubReceiver
    {
        Task OnReceiveMessageAsync(string message);
    }

    public interface IGeneralHub : IStreamingHub<IGeneralHub, IGeneralHubReceiver>
    {
        Task SendMessageAsync(string message);
    }

}
