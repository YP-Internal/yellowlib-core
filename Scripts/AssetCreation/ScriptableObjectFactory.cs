using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YellowPanda.Core.AssetCreation
{
    public static class ScriptableObjectFactory
    {
#if UNITY_EDITOR
        public static T Create<T>(string path) where T : ScriptableObject
        {
            T instance = ScriptableObject.CreateInstance<T>();

            EnsureDirectoryExists(path);

            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();

            return instance;
        }


        public static string GetEditorAssetPath()
        {
            string directoryPath = "Assets/Resources/Settings";

            EnsureDirectoryExists(directoryPath);

            string path = EditorUtility.SaveFilePanelInProject(
                "Create SO",
                "NewData",
                "asset",
                "Enter asset name",
                directoryPath
            );

            return path;
        }


        public static void EnsureDirectoryExists(string filePath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Debug.Log($"Creating directory: {directoryPath}");
                    Directory.CreateDirectory(directoryPath);
                    AssetDatabase.Refresh();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create directory for path: {filePath}\nError: {ex.Message}");
            }
        }
#endif
    }
}