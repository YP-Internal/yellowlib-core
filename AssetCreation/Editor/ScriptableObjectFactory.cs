using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YellowPanda.AssetCreation
{
    public static class ScriptableObjectFactory
    {
        public static T Create<T>(string path) where T : ScriptableObject
        {
            T instance = ScriptableObject.CreateInstance<T>();

            EnsureDirectoryExists(path);

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
#endif

            return instance;
        }

        public static string GetEditorAssetPath()
        {
#if UNITY_EDITOR
            string directoryPath = "Assets/Resources/Settings";

            EnsureDirectoryExists(directoryPath);
            string path = EditorUtility.SaveFilePanelInProject(
                "Create SO",
                "NewData",
                "asset",
                "Enter asset name",
                directoryPath
            );
#else
            string path = Application.persistentDataPath;
#endif

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
#if UNITY_EDITOR
                    AssetDatabase.Refresh();
#endif
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create directory for path: {filePath}\nError: {ex.Message}");
            }
        }
    }
}