using System.IO;
using UnityEditor;
using UnityEngine;

namespace Minamo.Cliend.Editor.Features
{
    public class AppConfigWindow : EditorWindow
    {
        [MenuItem("Window/AppConfig")]
        private static void Init()
        {
            GetWindow<AppConfigWindow>("AppConfig");
        }

        public enum AppConfigType
        {
            Local,
            Development,
        }

        private string _currentServiceUrl;

        private static string ResourcesDirectoryPath => Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/");
        private static string LocalJsonFilePath => Path.Combine(ResourcesDirectoryPath, "config.local.json");
        private static string DevelopmentJsonFilePath => Path.Combine(ResourcesDirectoryPath, "config.development.json");
        private static string EditorJsonFilePath => Path.Combine(ResourcesDirectoryPath, "config.editor.json");

        public void OnGUI()
        {
            if (string.IsNullOrWhiteSpace(this._currentServiceUrl))
            {
                this._currentServiceUrl = this.GetCurrentUrl();
            }

            EditorGUILayout.LabelField($"Service Url: {this._currentServiceUrl}");

            if (GUILayout.Button("local"))
            {
                this.CopyConfig(LocalJsonFilePath);
            }
            if (GUILayout.Button("development"))
            {
                this.CopyConfig(DevelopmentJsonFilePath);
            }
        }

        private void CopyConfig(string filePath)
        {
            File.Copy(Path.Combine(ResourcesDirectoryPath, filePath), EditorJsonFilePath, true);

            this._currentServiceUrl = this.GetCurrentUrl();

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private string GetCurrentUrl()
        {
            if (!File.Exists(EditorJsonFilePath))
            {
                CopyConfig(LocalJsonFilePath);
            }

            foreach(var line in File.ReadAllLines(EditorJsonFilePath))
            {
                if (line.Contains("ServiceUrl"))
                {
                    return line.Split(':')[1];
                }
            }

            return string.Empty;
        }
    }
}
