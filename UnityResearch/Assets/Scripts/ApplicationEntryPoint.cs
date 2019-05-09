using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityResearch.Client.Network;

namespace UnityResearch.Client
{
    public static class ApplicationEntryPoint
    {
        private static UniTaskCompletionSource _source = new UniTaskCompletionSource();
        public static UniTask WaitInitializationAsync() => _source.Task;

        [RuntimeInitializeOnLoadMethod]
        private static void Main()
        {
            MainCoreAsync().ConfigureAwait(PlayerLoopTiming.Update).Forget();
        }

        private static async UniTask MainCoreAsync()
        {
#if UNITY_STANDALONE
            Screen.SetResolution(640, 1136, false);
#endif
            
            MainThreadDispatcher.Initialize();
            while(!MainThreadDispatcher.IsInitialized)
            {
                await UniTask.DelayFrame(1);
            }

            MessagePack.Resolvers.CompositeResolver.RegisterAndSetAsDefault
            (
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.BuiltinResolver.Instance,
                MessagePack.Resolvers.PrimitiveObjectResolver.Instance,
                Resolvers.MagicOnionResolver.Instance
            );

            await NetworkConfig.Current.LoadAsync();

            _source.TrySetResult();
        }
    }
}
