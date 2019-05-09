using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using UnityResearch.Client.Network;
using UnityResearch.Service.ApiDefinitions;

namespace UnityResearch.Client
{
    public class ConnectTest : MonoBehaviour, IGeneralHubReceiver
    {
        class GeneralClient : NetworkClient<IGeneralHub, IGeneralHubReceiver>
        {
            public GeneralClient(IGeneralHubReceiver receiver) : base(receiver) { }
        }

        async void Start()
        {
            await ApplicationEntryPoint.WaitInitializationAsync();

            var client = new GeneralClient(this);
            while (!client.IsConnected.Value)
            {
                await UniTask.DelayFrame(1);
            }

            await client.Hub.SendMessageAsync("Nyanpasu");
        }

        public Task OnReceiveMessageAsync(string message)
        {
            Debug.Log($"OnReceiveMessageAsync: {message}");

            return Task.CompletedTask;
        }
    }
}