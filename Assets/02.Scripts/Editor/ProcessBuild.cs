using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;

namespace LastArk
{
    #if UNITY_EDITOR
    public class ProcessBuild
    {
        private const string APP_NAME = "LastArk";
        private const string KEYSTORE_PASSWORD = "wear123!";
        private const string BUILD_PATH = "Build/";
        private const string ANDROID_BUILD_PATH = BUILD_PATH + "Android/";
        private const string IOS_BUILD_PATH = BUILD_PATH + "iOS/";

        private const string ADDRESSABLES_BUILD_SCRIPT = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        private const string ADDRESSABLES_BUILD_SETTINGS = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        private const string ADDRESSABLES_PROFILE_DEV = "Dev";
        private const string ADDRESSABLES_PROFILE_TEST = "Test";
        private const string ADDRESSABLES_PROFILE_LIVE = "Live";
        private static AddressableAssetSettings settings;

        [MenuItem("Builder/Build/Android build")]
        public static void PerformAndroidBuild()
        {
            string fileName = SetPlayerSettingsForAndroid();

            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            buildOption.locationPathName = ANDROID_BUILD_PATH + fileName;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.Android;
            BuildPipeline.BuildPlayer(buildOption);
        }

        [MenuItem("Builder/Build/iOS build")]
        public static void PerformIOSBuild()
        {
            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.iOS;
            buildOption.locationPathName = IOS_BUILD_PATH;
            BuildPipeline.BuildPlayer(buildOption);
        }

        [MenuItem("Builder/Build/Addressable build/Dev")]
        public static bool PerformAddressablesBuild_Dev()
        {
            getSettingsObject(ADDRESSABLES_BUILD_SETTINGS);
            setProfile(ADDRESSABLES_PROFILE_DEV);
            IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(ADDRESSABLES_BUILD_SCRIPT) as IDataBuilder;

            if (builderScript == null) {
                LogManager.LogError(LogManager.LogType.EXCEPTION, ADDRESSABLES_BUILD_SCRIPT + " couldn't be found or isn't a build script.");
                return false;
            }

            setBuilder(builderScript);

            return buildAddressableContent();
        }

        [MenuItem("Builder/Build/Addressable update/Dev")]
        public static void PerformAddressablesUpdate_Dev()
        {
            
        }

        [MenuItem("Builder/Build/Addressable build/Test")]
        public static bool PerformAddressablesBuild_Test()
        {
            getSettingsObject(ADDRESSABLES_BUILD_SETTINGS);
            setProfile(ADDRESSABLES_PROFILE_TEST);
            IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(ADDRESSABLES_BUILD_SCRIPT) as IDataBuilder;

            if (builderScript == null) {
                LogManager.LogError(LogManager.LogType.EXCEPTION, ADDRESSABLES_BUILD_SCRIPT + " couldn't be found or isn't a build script.");
                return false;
            }

            setBuilder(builderScript);

            return buildAddressableContent();
        }

        [MenuItem("Builder/Build/Addressable update/Test")]
        public static void PerformAddressablesUpdate_Test()
        {

        }

        [MenuItem("Builder/Build/Addressable build/Live")]
        public static bool PerformAddressablesBuild_Live()
        {
            getSettingsObject(ADDRESSABLES_BUILD_SETTINGS);
            setProfile(ADDRESSABLES_PROFILE_LIVE);
            IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(ADDRESSABLES_BUILD_SCRIPT) as IDataBuilder;

            if (builderScript == null) {
                LogManager.LogError(LogManager.LogType.EXCEPTION, ADDRESSABLES_BUILD_SCRIPT + " couldn't be found or isn't a build script.");
                return false;
            }

            setBuilder(builderScript);

            return buildAddressableContent();
        }

        [MenuItem("Builder/Build/Addressable update/Live")]
        public static void PerformAddressablesUpdate_Live()
        {

        }


        static void getSettingsObject(string settingsAsset) {
            // This step is optional, you can also use the default settings:
            //settings = AddressableAssetSettingsDefaultObject.Settings;

            settings
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                    as AddressableAssetSettings;

            if (settings == null)
                Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                               $"a settings object.");
        }

        static void setProfile(string profile) {
            string profileId = settings.profileSettings.GetProfileId(profile);
            if (String.IsNullOrEmpty(profileId))
                Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                                 $"using current profile instead.");
            else
                settings.activeProfileId = profileId;
        }

        static void setBuilder(IDataBuilder builder) {
            int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

            if (index > 0)
                settings.ActivePlayerDataBuilderIndex = index;
            else
                Debug.LogWarning($"{builder} must be added to the " +
                                 $"DataBuilders list before it can be made " +
                                 $"active. Using last run builder instead.");
        }

        static bool buildAddressableContent() {
            AddressableAssetSettings
                .BuildPlayerContent(out AddressablesPlayerBuildResult result);
            bool success = string.IsNullOrEmpty(result.Error);

            if (!success) {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            return success;
        }

        [MenuItem("Builder/OpenBuildDirectory")]
        public static void OpenBuildDirectory()
        {
            OpenFileBrowser(Path.GetFullPath(BUILD_PATH));
        }

        private static void OpenFileBrowser(string path)
        {
            bool openInsidesOfFolder = false;

            if(Directory.Exists(path))
            {
                openInsidesOfFolder = true;
            }

            string arg = (openInsidesOfFolder ? "" : "-R ") + path;
            try
            {
                System.Diagnostics.Process.Start("open", arg);
            }
            catch(Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"Failed to open build file path : {e}");
            }
        }

        private static string[] GetBuildSceneList()
        {
            EditorBuildSettingsScene[] scenes  = UnityEditor.EditorBuildSettings.scenes;

            List<string> listScenePath = new List<string>();

            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].enabled == true)
                {
                    listScenePath.Add(scenes[i].path);
                }
            }

            return listScenePath.ToArray();
        }

        private static string SetPlayerSettingsForAndroid()
        {
            PlayerSettings.Android.keystorePass = KEYSTORE_PASSWORD;
            PlayerSettings.Android.keyaliasPass = KEYSTORE_PASSWORD;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"{APP_NAME}_v{PlayerSettings.bundleVersion}_{timeStamp}.apk";
            return fileName;
        }

#if UNITY_IOS
#else
#endif
    }
    #endif
}
