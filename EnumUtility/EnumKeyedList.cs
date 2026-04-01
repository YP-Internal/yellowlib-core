using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YellowPanda.EnumUtilitys
{
    [Serializable]
    public class EnumKeyedList<UnityObject> where UnityObject : UnityEngine.Object
    {
        [Serializable]
        public class InternalEnumKeyedList<T> : IList<T> where T : UnityEngine.Object
        {
            internal string output;
            internal bool autoSave = true;
            internal string overrideName;
            internal string path;

            public List<T> items = new();

            public T this[int index]
            {
                get => items[index];
                set => items[index] = value;
            }

            public T this[Enum enumValue]
            {
                get
                {
                    int index = Convert.ToInt32(enumValue);
                    if (index >= 0 && index < items.Count)
                        return items[index];

                    Debug.LogError($"Enum value {enumValue} (index {index}) is out of range. Total items: {items.Count}");
                    return null;
                }
                set
                {
                    int index = Convert.ToInt32(enumValue);
                    if (index >= 0 && index < items.Count)
                        items[index] = value;
                    else
                        Debug.LogError($"Enum value {enumValue} (index {index}) is out of range. Total items: {items.Count}");
                }
            }

            public int Count => items.Count;

            public bool IsReadOnly => false;


            bool CanAdd(T item)
            {
                if (items.Exists(x => x.name == item.name))
                {
                    Debug.LogError("Item with same name aready added");
                    return false;
                }
                return true;
            }
            public void Add(T item)
            {
                if (!CanAdd(item)) return;

                items.Add(item);
                UpdateEnum();
            }

            public void Clear()
            {
                items.Clear();
                UpdateEnum();
            }
            public bool Contains(T item) => items.Contains(item);

            public void CopyTo(T[] array, int arrayIndex)
            {
                items.CopyTo(array, arrayIndex);
                UpdateEnum();
            }

            public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

            public int IndexOf(T item) => items.IndexOf(item);

            public void Insert(int index, T item)
            {
                if (!CanAdd(item)) return;

                items.Insert(index, item);
                UpdateEnum();
            }

            public bool Remove(T item)
            {
                var result = items.Remove(item);
                UpdateEnum();

                return result;
            }

            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
                UpdateEnum();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }


            void UpdateEnum()
            {
                if (!autoSave)
                    return;

                GenerateEnum();
            }
            public void GenerateEnum()
            {
                if (items == null || items.Count == 0)
                    return;

                var itemsArray = items.ToArray();
                enumResult = EnumFileGenerator.GenerateEnumFile(itemsArray, overridName: overrideName, folderOutput: output);
            }
            public EnumFileGenerator.GenerationCallback enumResult;
        }
        string GetEnumType { get => $"Enum Type: {items.enumResult.enumName}"; }
        string GetEnumFilePath { get => $"Enum File Path: {items.enumResult.enumPath}"; }

        [BoxGroup("@" + nameof(GetEnumType))]
        [PropertyOrder(-1)]
        [Button]
        public void ForceGenerateEnum()
        {
            UpdateSettings();
            items.GenerateEnum();
        }

#if UNITY_EDITOR
        [BoxGroup("@" + nameof(GetEnumFilePath))]
        [PropertyOrder(-1)]
        [Button]
        public void PingEnumFile()
        {
            var enumFile = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(items.enumResult.enumPath);

            EditorGUIUtility.PingObject(enumFile);
        }

        [ToggleLeft]
        [BoxGroup("Settings")]
        [LabelText("Auto-Save Enum File")]
        [Tooltip("Automatically generate the enum file when items are modified")]
        [OnValueChanged(nameof(UpdateSettings))]
#endif
        [SerializeField] private bool autoSave = true;

#if UNITY_EDITOR

        [BoxGroup("Settings")]
        [LabelText("Output Folder")]
        [Tooltip("Folder path where the enum file will be generated (relative to project root)")]
        [FolderPath]
        [OnValueChanged(nameof(UpdateSettings))]
#endif
        [SerializeField] private string folderOutput;

#if UNITY_EDITOR

        [BoxGroup("Settings")]
        [LabelText("Enum Name")]
        [Tooltip("Name of the generated enum (leave empty to use default naming)")]
        [OnValueChanged(nameof(UpdateSettings))]
        [SerializeField] private string enumName;
#endif

        public InternalEnumKeyedList<UnityObject> items = new();

#if UNITY_EDITOR
        private void UpdateSettings()
        {
            if (items != null)
            {
                items.autoSave = autoSave;
                items.output = folderOutput;
                items.overrideName = enumName;
            }
        }
#endif
        public UnityObject this[Enum enumValue]
        {
            get => items[enumValue];
            set => items[enumValue] = value;
        }

    }
}
