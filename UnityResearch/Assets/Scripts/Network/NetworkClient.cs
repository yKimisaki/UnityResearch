using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using System;
using UniRx;
using UniRx.Async;

namespace UnityResearch.Client.Network
{
    public abstract class NetworkClient<TStreamingHub, TReceiver> : IConnectableClient, IDisposable where TStreamingHub : IStreamingHub<TStreamingHub, TReceiver>
    {
        private static ChannelConnetor _globalChannelConnector;

        public TStreamingHub Hub { get; private set; }
        private TReceiver _receiver;

        private bool _isDisposing;
        private ReactiveProperty<bool> _isConnected;
        public IReadOnlyReactiveProperty<bool> IsConnected => _isConnected;

        private static async UniTask CreateChannelConnectorAsync()
        {
            if (_globalChannelConnector != null)
            {
                return;
            }

            var options = new[]
            {
                new ChannelOption("grpc.keepalive_permit_without_calls", 1),
            };

            UnityEngine.Debug.Log($"AppConfig: {NetworkConfig.Current.ServiceUrl}:{NetworkConfig.Current.Port}");
            var channel = new Channel(NetworkConfig.Current.ServiceUrl, NetworkConfig.Current.Port, ChannelCredentials.Insecure, options);

            _globalChannelConnector = new ChannelConnetor(channel);

            Observable.OnceApplicationQuit()
                .Subscribe(_ => _globalChannelConnector.DisposeAsync().Forget());

            await _globalChannelConnector.StartConnectAsync();
        }

        protected NetworkClient(TReceiver receiver)
        {
            this._receiver = receiver;

            this._isConnected = new ReactiveProperty<bool>();

            CreateChannelConnectorAsync()
                .ContinueWith(() => _globalChannelConnector.BindAsync(this))
                .Forget();
        }

        void IDisposable.Dispose()
        {
            _globalChannelConnector.UnbindAsync(this).Forget();
        }

        async UniTask IConnectableClient.ConnectHubAsync(Channel channel)
        {
            while (this._isConnected.Value)
            {
                if (this._isDisposing)
                {
                    await UniTask.DelayFrame(1);
                    continue;
                }

                return;
            }

            this.Hub = StreamingHubClient.Connect<TStreamingHub, TReceiver>(channel, this._receiver);
            channel.RegisterStreamingSubscription(this);

            this._isConnected.Value = true;
        }

        async UniTask IConnectableClient.DisconnectHubAsync()
        {
            if (this._isDisposing || this.Hub == null)
            {
                return;
            }
            this._isDisposing = true;

            await this.Hub.DisposeAsync().ConfigureAwait(false);

            this._isConnected.Value = false;

            this._isDisposing = false;
        }
    }
}
