using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace YellowPanda.EnumUtilitys
{
    public static class EnumFileGenerator
    {
        const string DEFAULT_FOLDER_OUTPUT = "Assets/Sourse/Scripts/GeneratedEnums/";
        const string ROOT_NAMESPACE = "YellowPanda.GeneratedEnums";

        [System.Serializable]
        public struct GenerationCallback
        {
            public string enumName;
            public string enumPath;
        }

        /// <summary>
        /// Generates a C# enum file from a list of string entries.
        /// </summary>
        /// <param name="enumsEntrys">List of string values to be used as enum entries.</param>
        /// <param name="enumName">The name of the enum to be generated.</param>
        /// <param name="folderOutput">The output folder path where the enum file will be created. Defaults to DEFAULT_FOLDER_OUTPUT.</param>
        /// <exception cref="DuplicateNameException">Thrown when duplicate names are found in the enumsEntrys list.</exception>
        public static GenerationCallback GenerateEnumFile(List<string> enumsEntrys, string enumName, string aditionalNamespace = "", string folderOutput = DEFAULT_FOLDER_OUTPUT)
        {
            if (HasDuplicatedNames(enumsEntrys))
                throw new DuplicateNameException("Cant Generate Enum with duplicated names");

            if (string.IsNullOrEmpty(folderOutput))
                folderOutput = DEFAULT_FOLDER_OUTPUT;

            StringBuilder sb = new();

            sb.AppendLine("// This file was generated automatically, don't change.");
            sb.AppendLine();
            sb.AppendLine($"namespace {ROOT_NAMESPACE}{(string.IsNullOrEmpty(aditionalNamespace) ? ' ' : '.')}{aditionalNamespace}");
            sb.AppendLine("    {");
            sb.AppendLine("            public enum " + enumName);
            sb.AppendLine("    {");

            foreach (var name in enumsEntrys)
            {
                string enumValue = SanitizeEnumName(name);
                sb.AppendLine("    " + enumValue + ",");
            }

            sb.AppendLine("}");
            sb.AppendLine("}");


            if (!Directory.Exists(folderOutput))
                Directory.CreateDirectory(folderOutput);

            string filePath = Path.Combine(folderOutput, enumName + ".cs");
            if (!File.Exists(filePath))
                File.CreateText(filePath);

            File.WriteAllText(filePath, sb.ToString());

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            return new()
            {
                enumName = enumName,
                enumPath = filePath,
            };
        }

        /// <summary>
        /// Generates a C# enum file from an array of Unity objects (typically ScriptableObjects).
        /// </summary>
        /// <typeparam name="ObjectType">The type of Unity objects to extract names from. Must derive from UnityEngine.Object.</typeparam>
        /// <param name="objects">Array of Unity objects whose names will be used as enum entries.</param>
        /// <param name="overridName">Optional custom name for the enum. If not provided, defaults to "{ObjectType}Ids".</param>
        /// <param name="folderOutput">The output folder path where the enum file will be created. Defaults to DEFAULT_FOLDER_OUTPUT.</param>
        public static GenerationCallback GenerateEnumFile<ObjectType>(ObjectType[] objects, string overridName = "", string aditionalNamespace = "", string folderOutput = DEFAULT_FOLDER_OUTPUT) where ObjectType : UnityEngine.Object
        {
            if (objects == null || objects.Length == 0)
            {
                Debug.LogError("No ScriptableObjects in the list!");
                return default;
            }

            List<string> enumEntryNames =
                objects
                    .Select(x => x.name)
                    .ToList();

            string enumName = string.IsNullOrEmpty(overridName) ? $"{typeof(ObjectType).Name}Ids" : overridName;

            return GenerateEnumFile(enumEntryNames, enumName: enumName, aditionalNamespace: aditionalNamespace, folderOutput: folderOutput);
        }

        private static bool HasDuplicatedNames(List<string> names)
        {
            var seen = new HashSet<string>();

            foreach (var name in names)
            {
                if (!seen.Add(name))
                    return true;
            }

            return false;
        }


        private static readonly Regex InvalidCharsRegex = new(@"[^a-zA-Z0-9_]", RegexOptions.Compiled);
        private static string SanitizeEnumName(string name)
        {
            string sanitized = InvalidCharsRegex.Replace(name, "");

            if (char.IsDigit(sanitized[0]))
                sanitized = "_" + sanitized;

            return sanitized;
        }
    }
}
