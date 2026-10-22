using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;

namespace AnkleBreaker.Utils.UniversalTypes.Editor.Tests
{
    public class I2LocAsmdefSetupTests
    {
        private const string FixtureRoot = "Assets/I2";
        private const string ScriptsPath = FixtureRoot + "/Localization/Scripts";
        private bool _ownsFixture;

        [SetUp]
        public void CreateIsolatedI2Folders()
        {
            if (Directory.Exists(FixtureRoot))
                Assert.Ignore("This fixture needs an empty Assets/I2 folder location");

            AssetDatabase.CreateFolder("Assets", "I2");
            _ownsFixture = true;
            AssetDatabase.CreateFolder(FixtureRoot, "Localization");
            AssetDatabase.CreateFolder(FixtureRoot + "/Localization", "Scripts");
            AssetDatabase.CreateFolder(ScriptsPath, "Editor");
        }

        [TearDown]
        public void RemoveOwnedFixture()
        {
            if (_ownsFixture)
            {
                Assert.IsTrue(AssetDatabase.DeleteAsset(FixtureRoot));
                _ownsFixture = false;
            }
        }

        [Test]
        public void Setup_IsNotRegisteredForAutomaticInitialization()
        {
            Assert.IsNull(typeof(I2LocAsmdefSetup).GetCustomAttribute<InitializeOnLoadAttribute>(),
                "I2 setup must be an explicit choice rather than a prompt on every session");
        }

        [Test]
        public void LoadingSetup_WithoutPreviousDismissal_DoesNotSchedulePromptOrWriteFiles()
        {
            const string legacySessionKey = "AB_I2LocAsmdefSetup_Asked";
            bool previousAsked = SessionState.GetBool(legacySessionKey, false);
            var previousDelayCall = EditorApplication.delayCall;
            try
            {
                SessionState.SetBool(legacySessionKey, false);
                typeof(I2LocAsmdefSetup).TypeInitializer?.Invoke(null, null);
                Assert.AreEqual(previousDelayCall, EditorApplication.delayCall);
                Assert.IsFalse(SessionState.GetBool(legacySessionKey, false));
                Assert.IsFalse(File.Exists(ScriptsPath + "/I2.Loc.asmdef"));
                Assert.IsFalse(File.Exists(ScriptsPath + "/Editor/I2.Loc.Editor.asmdef"));
            }
            finally
            {
                // This synchronous test restores only the callback list captured before its probe.
                EditorApplication.delayCall = previousDelayCall;
                SessionState.SetBool(legacySessionKey, previousAsked);
            }
        }

        [Test]
        public void ExplicitMenu_CreatesBothAssemblyDefinitions()
        {
            Assert.IsTrue(EditorApplication.ExecuteMenuItem(
                "AnkleBreaker/UniversalTypes/Create I2L Assembly Definitions"));
            string runtimeJson = File.ReadAllText(ScriptsPath + "/I2.Loc.asmdef");
            string editorJson = File.ReadAllText(ScriptsPath + "/Editor/I2.Loc.Editor.asmdef");
            StringAssert.Contains("\"name\": \"I2.Loc\"", runtimeJson);
            StringAssert.Contains("\"name\": \"I2.Loc.Editor\"", editorJson);
            StringAssert.Contains("\"I2.Loc\"", editorJson);
            StringAssert.Contains("\"Editor\"", editorJson);
        }
    }
}
