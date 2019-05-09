using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Async;

namespace UnityResearch.Client.Network
{
    internal interface IConnectableClient : IDisposable
    {
        UniTask ConnectHubAsync(Channel channel);
        UniTask DisconnectHubAsync();
    }

    internal class ChannelConnetor
    {
        private global::Grpc.Core.Channel _channel;

        private ReactiveProperty<global::Grpc.Core.ChannelState> _channelState;
        public IReadOnlyReactiveProperty<global::Grpc.Core.ChannelState> ChannelState => this._channelState;

        private HashSet<IConnectableClient> _clientList;

        private bool _isDisposed;

        public ChannelConnetor(Channel channel)
        {
            this._channelState = new ReactiveProperty<ChannelState>();
            this._clientList = new HashSet<IConnectableClient>();
            this._channel = channel;
        }

        public async UniTask StartConnectAsync()
        {
            this.ConnectLoopAsync().Forget();

            await this._channel.ConnectAsync().ConfigureAwait(false);
        }

        private async UniTask ConnectLoopAsync()
        {
            while (this._channel.State != global::Grpc.Core.ChannelState.Shutdown)
            {
                var state = this._channel.State;
                UnityEngine.Debug.Log($"ChannelState: {state}");
                if (await this._channel.TryWaitForStateChangedAsync(state).ConfigureAwait(false))
                {
                    this._channelState.Value = this._channel.State;

                    switch (this._channel.State)
                    {
                        case global::Grpc.Core.ChannelState.Connecting:
                        case global::Grpc.Core.ChannelState.Ready:
                            break;
                        case global::Grpc.Core.ChannelState.TransientFailure:
                        case global::Grpc.Core.ChannelState.Idle:
                            goto Retry;
                        default:
                            return;
                    }
                }

                continue;

            Retry:
                await UniTask.WhenAll(this._clientList
                    .Select(x => x.DisconnectHubAsync())
                    .ToArray());

                await this._channel.ConnectAsync().ConfigureAwait(false);

                await UniTask.WhenAll(this._clientList
                    .Select(x => x.ConnectHubAsync(this._channel))
                    .ToArray());
            }
        }

        public async UniTask BindAsync(IConnectableClient client)
        {
            if (this._clientList.Add(client))
            {
                await client.ConnectHubAsync(this._channel);
            }
        }

        public async UniTask UnbindAsync(IConnectableClient client)
        {
            if (this._clientList.Remove(client))
            {
                await client.DisconnectHubAsync();
            }
        }

        public async UniTask DisposeAsync()
        {
            if (this._isDisposed)
            {
                return;
            }
            this._isDisposed = true;

            await this._channel.ShutdownAsync().ConfigureAwait(false);
        }
    }
}
