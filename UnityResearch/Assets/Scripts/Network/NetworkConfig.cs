using System;
using UniRx.Async;
using UnityEngine;

namespace UnityResearch.Client.Network
{
    public class NetworkConfig
    {
        private static NetworkConfig _current;
        public static NetworkConfig Current
        {
            get { return _current = _current ?? new NetworkConfig(); }
        }

        private AppConfigInternal _internal;

        public string ServiceUrl => this._internal.ServiceUrl;
        public int Port => this._internal.Port;

        private NetworkConfig() { }

        public async UniTask LoadAsync()
        {
            var isEditor = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;
            var operation = Resources.LoadAsync<TextAsset>(isEditor ? "config.editor" : "config");
            await operation;

            this._internal = JsonUtility.FromJson<AppConfigInternal>((operation.asset as TextAsset).text);
        }

        [Serializable]
        private  class AppConfigInternal
        {
            public string ServiceUrl = string.Empty;
            public int Port = 0;
        }
    }
}
