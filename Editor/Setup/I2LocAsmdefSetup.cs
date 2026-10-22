using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.UniversalTypes.Editor
{
    /// <summary>Creates I2 Localization assembly definitions only through an explicit menu action.</summary>
    public static class I2LocAsmdefSetup
    {
        /// <summary>
        /// Creates I2.Loc.asmdef and I2.Loc.Editor.asmdef in the I2 Localization Scripts folder.
        /// Can be called manually from menu.
        /// </summary>
        [MenuItem("AnkleBreaker/UniversalTypes/Create I2L Assembly Definitions")]
        public static void CreateI2LocAsmdefs()
        {
            string scriptsPath = FindI2ScriptsPath();
            if (string.IsNullOrEmpty(scriptsPath))
            {
                EditorUtility.DisplayDialog("Error", "Could not find I2 Localization Scripts folder.", "OK");
                return;
            }

            // Detect which optional references I2L needs
            var runtimeRefs = new System.Collections.Generic.List<string>();
            if (IsPackageInstalled("com.unity.textmeshpro") || IsPackageInstalled("com.unity.ugui"))
                runtimeRefs.Add("Unity.TextMeshPro");
            if (IsPackageInstalled("com.unity.inputsystem"))
                runtimeRefs.Add("Unity.InputSystem");

            string refsJson = runtimeRefs.Count > 0
                ? "\"" + string.Join("\",\"", runtimeRefs) + "\""
                : "";

            // Runtime asmdef
            string runtimeAsmdef = $@"{{
    ""name"": ""I2.Loc"",
    ""rootNamespace"": ""I2.Loc"",
    ""references"": [{refsJson}],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";

            string runtimeAsmdefPath = Path.Combine(scriptsPath, "I2.Loc.asmdef");
            File.WriteAllText(runtimeAsmdefPath, runtimeAsmdef);

            // Editor asmdef
            string editorAsmdef = $@"{{
    ""name"": ""I2.Loc.Editor"",
    ""rootNamespace"": ""I2.Loc"",
    ""references"": [""I2.Loc""{(runtimeRefs.Count > 0 ? ",\"" + string.Join("\",\"", runtimeRefs) + "\"" : "")}],
    ""includePlatforms"": [""Editor""],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";

            string editorDir = Path.Combine(scriptsPath, "Editor");
            if (Directory.Exists(editorDir))
            {
                string editorAsmdefPath = Path.Combine(editorDir, "I2.Loc.Editor.asmdef");
                File.WriteAllText(editorAsmdefPath, editorAsmdef);
            }

            AssetDatabase.Refresh();
            Debug.Log("[UniversalTypes] Created I2.Loc assembly definitions. Recompiling...");
        }

        private static string FindI2ScriptsPath()
        {
            string[] possiblePaths =
            {
                Path.Combine(Application.dataPath, "I2", "Localization", "Scripts"),
                Path.Combine(Application.dataPath, "Plugins", "I2", "Localization", "Scripts"),
            };

            foreach (string path in possiblePaths)
            {
                if (Directory.Exists(path))
                    return path;
            }

            return null;
        }

        private static bool IsPackageInstalled(string packageName)
        {
            string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            if (!File.Exists(manifestPath)) return false;
            string manifest = File.ReadAllText(manifestPath);
            return manifest.Contains(packageName);
        }
    }
}
