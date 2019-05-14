#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace UnityResearch
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::MagicOnion;
    using global::MagicOnion.Client;

    public static partial class MagicOnionInitializer
    {
        static bool isRegistered = false;

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            if(isRegistered) return;
            isRegistered = true;


            StreamingHubClientRegistry<UnityResearch.Service.ApiDefinitions.IGeneralHub, UnityResearch.Service.ApiDefinitions.IGeneralHubReceiver>.Register((a, _, b, c, d, e) => new UnityResearch.Service.ApiDefinitions.IGeneralHubClient(a, b, c, d, e));
        }
    }
}

#pragma warning restore 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 612
#pragma warning restore 618
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace UnityResearch.Resolvers
{
    using System;
    using MessagePack;

    public class MagicOnionResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new MagicOnionResolver();

        MagicOnionResolver()
        {

        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                var f = MagicOnionResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class MagicOnionResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static MagicOnionResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(0)
            {
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 612
#pragma warning restore 618
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace UnityResearch.Service.ApiDefinitions {
    using Grpc.Core;
    using Grpc.Core.Logging;
    using MagicOnion;
    using MagicOnion.Client;
    using MessagePack;
    using System;
    using System.Threading.Tasks;

    public class IGeneralHubClient : StreamingHubClientBase<global::UnityResearch.Service.ApiDefinitions.IGeneralHub, global::UnityResearch.Service.ApiDefinitions.IGeneralHubReceiver>, global::UnityResearch.Service.ApiDefinitions.IGeneralHub
    {
        static readonly Method<byte[], byte[]> method = new Method<byte[], byte[]>(MethodType.DuplexStreaming, "IGeneralHub", "Connect", MagicOnionMarshallers.ThroughMarshaller, MagicOnionMarshallers.ThroughMarshaller);

        protected override Method<byte[], byte[]> DuplexStreamingAsyncMethod { get { return method; } }

        readonly global::UnityResearch.Service.ApiDefinitions.IGeneralHub __fireAndForgetClient;

        public IGeneralHubClient(CallInvoker callInvoker, string host, CallOptions option, IFormatterResolver resolver, ILogger logger)
            : base(callInvoker, host, option, resolver, logger)
        {
            this.__fireAndForgetClient = new FireAndForgetClient(this);
        }
        
        public global::UnityResearch.Service.ApiDefinitions.IGeneralHub FireAndForget()
        {
            return __fireAndForgetClient;
        }

        protected override void OnBroadcastEvent(int methodId, ArraySegment<byte> data)
        {
            switch (methodId)
            {
                case 919260368: // OnReceiveMessageAsync
                {
                    var result = LZ4MessagePackSerializer.Deserialize<string>(data, resolver);
                    receiver.OnReceiveMessageAsync(result); break;
                }
                default:
                    break;
            }
        }

        protected override void OnResponseEvent(int methodId, object taskCompletionSource, ArraySegment<byte> data)
        {
            switch (methodId)
            {
                case -601690414: // SendMessageAsync
                {
                    var result = LZ4MessagePackSerializer.Deserialize<Nil>(data, resolver);
                    ((TaskCompletionSource<Nil>)taskCompletionSource).TrySetResult(result);
                    break;
                }
                default:
                    break;
            }
        }
   
        public global::System.Threading.Tasks.Task SendMessageAsync(string message)
        {
            return WriteMessageWithResponseAsync<string, Nil>(-601690414, message);
        }


        class FireAndForgetClient : global::UnityResearch.Service.ApiDefinitions.IGeneralHub
        {
            readonly IGeneralHubClient __parent;

            public FireAndForgetClient(IGeneralHubClient parentClient)
            {
                this.__parent = parentClient;
            }

            public global::UnityResearch.Service.ApiDefinitions.IGeneralHub FireAndForget()
            {
                throw new NotSupportedException();
            }

            public Task DisposeAsync()
            {
                throw new NotSupportedException();
            }

            public Task WaitForDisconnect()
            {
                throw new NotSupportedException();
            }

            public global::System.Threading.Tasks.Task SendMessageAsync(string message)
            {
                return __parent.WriteMessageAsync<string>(-601690414, message);
            }

        }
    }
}

#pragma warning restore 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
