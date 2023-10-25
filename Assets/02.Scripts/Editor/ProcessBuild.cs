using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Herma;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using Amazon;
using Amazon.S3.Transfer;
using NUnit.Framework.Internal;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

namespace ClientTemplate
{
#if UNITY_EDITOR
    public class ProcessBuild
    {
        private enum ProfileType
        {
            None,
            Dev,
            Test,
            Live,
            
        }
        
        private const string BUILD_PATH = "Build/";
        private const string ANDROID_BUILD_PATH = BUILD_PATH + "Android/";
        private const string IOS_BUILD_PATH = BUILD_PATH + "iOS/";

        private const string ADDRESSABLES_BUILD_SCRIPT = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        private const string ADDRESSABLES_BUILD_SETTINGS = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        private const string ADDRESSABLES_PROFILE_DEV = "Dev";
        private const string ADDRESSABLES_PROFILE_TEST = "Test";
        private const string ADDRESSABLES_PROFILE_LIVE = "Live";

        
        private static AddressableAssetSettings settings;
        
        private static string AwsCredentialInfoPath = "Assets/NotIncludedInBuild/AwsCredential.asset";

        #region Android

        public static void JenkinsBuild()
        {
            string exp = GetArg("-export");
            string targ = GetArg("-target");
            string prof = GetArg("-profile");

            bool export = exp.ToLower().Contains("y");
            BuildTarget target;
            if (targ.ToLower().Contains("android") == true)
            {
                target = BuildTarget.Android;
            }
            else if (targ.ToLower().Contains("ios") == true)
            {
                target = BuildTarget.iOS;
            }
            else
            {
                Debug.LogError($"Unexpected build target! {targ}");
                return;
            }

            ProfileType profile;
            if (prof.ToLower().Contains("dev") == true)
            {
                profile = ProfileType.Dev;
            }
            else if (prof.ToLower().Contains("test") == true)
            {
                profile = ProfileType.Test;
            }
            else if (prof.ToLower().Contains("live") == true)
            {
                profile = ProfileType.Live;
            }
            else
            {
                Debug.LogError($"Unexpected profile! {prof}");
                return;
            }

            switch (target)
            {
                case BuildTarget.Android:
                {
                    switch (profile)
                    {
                        case ProfileType.Dev:
                        {
                            BuildAndroidAndAddressablesDev(export);
                        }
                            break;
                        case ProfileType.Test:
                        {
                            BuildAndroidAndAddressablesTest(export);
                        }
                            break;
                        case ProfileType.Live:
                        {
                            BuildAndroidAndAddressablesLive(export);
                        }
                            break;
                    }
                }
                    break;
                case BuildTarget.iOS:
                {
                    switch (profile)
                    {
                        case ProfileType.Dev:
                        {
                            BuildIOSAndAddressablesDev();
                        }
                            break;
                        case ProfileType.Test:
                        {
                            BuildIOSAndAddressablesTest();
                        }
                            break;
                        case ProfileType.Live:
                        {
                            BuildIOSAndAddressablesLive();
                        }
                            break;
                    }
                }
                    break;
            }
        }
        
        [MenuItem("JimmyTools/Builder/Build/Android/App and Addressables build/Dev")]
        public static void BuildAndroidAndAddressablesDev()
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformAndroidAddressablesBuildDev();
            PerformAndroidBuild();
        }
        
        public static void BuildAndroidAndAddressablesDev(bool export)
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformAndroidAddressablesBuildDev();
            PerformAndroidBuild(export);
        }
        
        [MenuItem("JimmyTools/Builder/Build/Android/App and Addressables build/Test")]
        public static void BuildAndroidAndAddressablesTest()
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformAndroidAddressablesBuildTest();
            PerformAndroidBuild();
        }
        
        public static void BuildAndroidAndAddressablesTest(bool export)
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformAndroidAddressablesBuildTest();
            PerformAndroidBuild(export);
        }
        
        [MenuItem("JimmyTools/Builder/Build/Android/App and Addressables build/Live")]
        public static void BuildAndroidAndAddressablesLive()
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformAndroidAddressablesBuildLive();
            PerformAndroidBuildForLive();
        }
        
        public static void BuildAndroidAndAddressablesLive(bool export)
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformAndroidAddressablesBuildLive();
            PerformAndroidBuildForLive(export);
        }

        [MenuItem("JimmyTools/Builder/Build/Android/App build/Others")]
        public static void PerformAndroidBuild()
        {
            string fileName = SetPlayerSettingsForAndroid();
            
            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            string buildPath = GetAppBuildLocation();
            if (string.IsNullOrEmpty(buildPath) == true)
            {
                buildPath = ANDROID_BUILD_PATH;
            }
            buildOption.locationPathName = buildPath + fileName;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.Android;
            BuildPipeline.BuildPlayer(buildOption);
        }

        public static void PerformAndroidBuild(bool export)
        {
            EditorUserBuildSettings.exportAsGoogleAndroidProject = export;
            string fileName = SetPlayerSettingsForAndroid();
            
            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            string buildPath = GetAppBuildLocation();
            if (string.IsNullOrEmpty(buildPath) == true)
            {
                buildPath = ANDROID_BUILD_PATH;
            }
            buildOption.locationPathName = buildPath + fileName;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.Android;
            BuildPipeline.BuildPlayer(buildOption);
        }

        [MenuItem("JimmyTools/Builder/Build/Android/App build/Live")]
        public static void PerformAndroidBuildForLive()
        {
            SetDefineSymbolsForLiveBuild(NamedBuildTarget.Android);
            string fileName = SetPlayerSettingsForAndroid();
            
            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            string buildPath = GetAppBuildLocation();
            if (string.IsNullOrEmpty(buildPath) == true)
            {
                buildPath = ANDROID_BUILD_PATH;
            }
            buildOption.locationPathName = buildPath + fileName;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.Android;
            BuildPipeline.BuildPlayer(buildOption);
        }

        public static void PerformAndroidBuildForLive(bool export)
        {
            EditorUserBuildSettings.exportAsGoogleAndroidProject = export;
            SetDefineSymbolsForLiveBuild(NamedBuildTarget.Android);
            string fileName = SetPlayerSettingsForAndroid();
            
            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            string buildPath = GetAppBuildLocation();
            if (string.IsNullOrEmpty(buildPath) == true)
            {
                buildPath = ANDROID_BUILD_PATH;
            }
            buildOption.locationPathName = buildPath + fileName;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.Android;
            BuildPipeline.BuildPlayer(buildOption);
        }
        
        #region Addressables

        #region Dev
        
        [MenuItem("JimmyTools/Builder/Build/Android/Addressables build/Dev")]
        public static void PerformAndroidAddressablesBuildDev()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            ClearAddressablesDirectory(BuildTarget.Android, ProfileType.Dev);
            PerformAddressablesBuild(ADDRESSABLES_PROFILE_DEV);
            UploadAndroidAddressablesDev();
        }

        [MenuItem("JimmyTools/Builder/Build/Android/Addressables update/Dev")]
        public static void PerformAndroidAddressablesUpdateDev()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            PerformAddressablesUpdate(ADDRESSABLES_PROFILE_DEV);
            UploadAndroidAddressablesDev();
        }
        
        private static void UploadAndroidAddressablesDev()
        {
            AwsCredential credential = GetCredential(AwsCredentialInfoPath);
            Transfer(credential, BuildTarget.Android, ProfileType.Dev);
        }
        
        #endregion

        #region Test
        
        [MenuItem("JimmyTools/Builder/Build/Android/Addressables build/Test")]
        public static void PerformAndroidAddressablesBuildTest()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            PerformAddressablesBuild(ADDRESSABLES_PROFILE_TEST);
            UploadAndroidAddressablesTest();
        }

        [MenuItem("JimmyTools/Builder/Build/Android/Addressables update/Test")]
        public static void PerformAndroidAddressablesUpdateTest()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            PerformAddressablesUpdate(ADDRESSABLES_PROFILE_TEST);
            UploadAndroidAddressablesTest();
        }
        
        private static void UploadAndroidAddressablesTest()
        {
            AwsCredential credential = GetCredential(AwsCredentialInfoPath);
            Transfer(credential, BuildTarget.Android, ProfileType.Test);
        }
        
        #endregion

        #region Live
        
        [MenuItem("JimmyTools/Builder/Build/Android/Addressables build/Live")]
        public static void PerformAndroidAddressablesBuildLive()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            PerformAddressablesBuild(ADDRESSABLES_PROFILE_LIVE);
            UploadAndroidAddressablesLive();
        }

        [MenuItem("JimmyTools/Builder/Build/Android/Addressables update/Live")]
        public static void PerformAndroidAddressablesUpdateLive()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            PerformAddressablesUpdate(ADDRESSABLES_PROFILE_LIVE);
            UploadAndroidAddressablesLive();
        }
        
        private static void UploadAndroidAddressablesLive()
        {
            AwsCredential credential = GetCredential(AwsCredentialInfoPath);
            Transfer(credential, BuildTarget.Android, ProfileType.Live);
        }
        
        #endregion
        
        #endregion
        
        #endregion
        
        #region iOS
        
        [MenuItem("JimmyTools/Builder/Build/iOS/App and Addressables build/Dev")]
        public static void BuildIOSAndAddressablesDev()
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformIOSAddressablesBuildDev();
            PerformIOSBuild();
        }
        
        [MenuItem("JimmyTools/Builder/Build/iOS/App and Addressables build/Test")]
        public static void BuildIOSAndAddressablesTest()
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformIOSAddressablesBuildTest();
            PerformIOSBuild();
        }
        
        [MenuItem("JimmyTools/Builder/Build/iOS/App and Addressables build/Live")]
        public static void BuildIOSAndAddressablesLive()
        {
            SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer);
            PerformIOSAddressablesBuildLive();
            PerformIOSBuildForLive();
        }
        
        [MenuItem("JimmyTools/Builder/Build/iOS/App build/Others")]
        public static void PerformIOSBuild()
        {
            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            string buildPath = GetAppBuildLocation();
            if (string.IsNullOrEmpty(buildPath) == true)
            {
                buildPath = IOS_BUILD_PATH;
            }
            buildOption.locationPathName = buildPath;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.iOS;
            BuildPipeline.BuildPlayer(buildOption);
        }
        
        [MenuItem("JimmyTools/Builder/Build/iOS/App build/Live")]
        public static void PerformIOSBuildForLive()
        {
            SetDefineSymbolsForLiveBuild(NamedBuildTarget.iOS);
            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            string buildPath = GetAppBuildLocation();
            if (string.IsNullOrEmpty(buildPath) == true)
            {
                buildPath = IOS_BUILD_PATH;
            }
            buildOption.locationPathName = buildPath;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.iOS;
            BuildPipeline.BuildPlayer(buildOption);
        }
        
        #region Addressables

        #region Dev
        
        [MenuItem("JimmyTools/Builder/Build/iOS/Addressables build/Dev")]
        public static void PerformIOSAddressablesBuildDev()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,BuildTarget.iOS);
            PerformAddressablesBuild(ADDRESSABLES_PROFILE_DEV);
            UploadIOSAddressablesDev();
        }

        [MenuItem("JimmyTools/Builder/Build/iOS/Addressables update/Dev")]
        public static void PerformIOSAddressablesUpdateDev()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,BuildTarget.iOS);
            PerformAddressablesUpdate(ADDRESSABLES_PROFILE_DEV);
            UploadIOSAddressablesDev();
        }
        
        private static void UploadIOSAddressablesDev()
        {
            AwsCredential credential = GetCredential(AwsCredentialInfoPath);
            Transfer(credential, BuildTarget.iOS, ProfileType.Dev);
        }
        
        #endregion

        #region Test
        
        [MenuItem("JimmyTools/Builder/Build/iOS/Addressables build/Test")]
        public static void PerformIOSAddressablesBuildTest()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,BuildTarget.iOS);
            PerformAddressablesBuild(ADDRESSABLES_PROFILE_TEST);
            UploadIOSAddressablesTest();
        }

        [MenuItem("JimmyTools/Builder/Build/iOS/Addressables update/Test")]
        public static void PerformIOSAddressablesUpdateTest()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,BuildTarget.iOS);
            PerformAddressablesUpdate(ADDRESSABLES_PROFILE_TEST);
            UploadIOSAddressablesTest();
        }
        
        private static void UploadIOSAddressablesTest()
        {
            AwsCredential credential = GetCredential(AwsCredentialInfoPath);
            Transfer(credential, BuildTarget.iOS, ProfileType.Test);
        }
        
        #endregion

        #region Live
        
        [MenuItem("JimmyTools/Builder/Build/iOS/Addressables build/Live")]
        public static void PerformIOSAddressablesBuildLive()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,BuildTarget.iOS);
            PerformAddressablesBuild(ADDRESSABLES_PROFILE_LIVE);
            UploadIOSAddressablesLive();
        }

        [MenuItem("JimmyTools/Builder/Build/iOS/Addressables update/Live")]
        public static void PerformIOSAddressablesUpdateLive()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,BuildTarget.iOS);
            PerformAddressablesUpdate(ADDRESSABLES_PROFILE_LIVE);
            UploadIOSAddressablesLive();
        }
        
        private static void UploadIOSAddressablesLive()
        {
            AwsCredential credential = GetCredential(AwsCredentialInfoPath);
            Transfer(credential, BuildTarget.iOS, ProfileType.Live);
        }
        
        #endregion
        
        #endregion
        
        #endregion

        private static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(name) == true && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }

            return string.Empty;
        }

        private static string GetAppBuildLocation()
        {
            BoppinBuildSettings settings = AssetDatabase.LoadAssetAtPath<BoppinBuildSettings>("Assets/Settings/BoppinBuildSettings.asset");
            if (string.IsNullOrEmpty(settings.BuildLocation) == true)
            {
                return null;
            }
            
            string result = Path.GetFullPath($"{settings.BuildLocation}/");
            if (Path.IsPathFullyQualified(result) == false)
            {
                LogManager.Log(LogManager.LogType.DEFAULT, $"{result} is invalid build location!");
                result = null;
            }
            return result;
        }

        private static void SetDefineSymbolsForLiveBuild(NamedBuildTarget buildTarget)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] symbols);
            RemoveDefineSymbol("TEST_BUILD", ref symbols);
            AddDefineSymbol("INTEGRATING_NATIVE", ref symbols);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, symbols);
        }

        private static void SetDefineSymbolsForDevelopment(NamedBuildTarget buildTarget)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] symbols);
            AddDefineSymbol("TEST_BUILD", ref symbols);
            RemoveDefineSymbol("INTEGRATING_NATIVE", ref symbols);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, symbols);
        }

        private static void RemoveDefineSymbol(string removingDefine, ref string[] defines)
        {
            List<string> tmpDefines = defines.ToList();
            foreach (string define in tmpDefines)
            {
                if (define == removingDefine)
                {
                    tmpDefines.Remove(define);
                    break;
                }
            }

            defines = tmpDefines.ToArray();
        }

        private static void AddDefineSymbol(string addingDefine, ref string[] defines)
        {
            bool integratingNativeFound = false;
            foreach (string define in defines)
            {
                if (define == addingDefine)
                {
                    integratingNativeFound = true;
                    break;
                }
            }

            if (integratingNativeFound == false)
            {
                List<string> tmpDefines = defines.ToList();
                tmpDefines.Add(addingDefine);
                defines = tmpDefines.ToArray();
            }
        }

        private static AwsCredential GetCredential(string path)
        {
            AwsCredential result = AssetDatabase.LoadAssetAtPath<AwsCredential>(path);
            if (result == null)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"AwsCredential is null! Check {path}");
                return null;
            }
            
            if (string.IsNullOrEmpty(result.AwsBucketName) == true)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"AwsBucketName is null! Check {path}");
                return null;
            }
            
            if (string.IsNullOrEmpty(result.AwsAccessKey) == true)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"AwsAccessKey is null! Check {path}");
                return null;
            }
            
            if (string.IsNullOrEmpty(result.AwsSecretKey) == true)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"AwsSecretKey is null! Check {path}");
                return null;
            }

            return result;
        }

        private static async void Transfer(AwsCredential credential, BuildTarget target, ProfileType profile)
        {
            if (settings != null)
            {
                if (settings.BuildRemoteCatalog == false)
                {
                    LogManager.Log(LogManager.LogType.DEFAULT, "Addressables setting BuildRemoteCatalog is false. Abort uploading to s3.");
                    return;
                }
            }
            
            if (target != BuildTarget.Android && target != BuildTarget.iOS)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, "Unexpected build target!");
                return;
            }

            if (profile == ProfileType.None)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, "Unexpected profile type!");
                return;
            }

            var fileTransferUtility = new TransferUtility(credential.AwsAccessKey, credential.AwsSecretKey, RegionEndpoint.APNortheast2);
            string filePath = Application.dataPath.Replace("Assets", "") + $"ServerData/{profile}/{target}";
            string bucketName = $"{credential.AwsBucketName}/{profile}/{target}";
            string[] files = Directory.GetFiles(filePath);
            for (int i = 0; i < files.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Uploading.....", files[i], i / (float)files.Length);
                LogManager.Log(LogManager.LogType.UPLOAD_TO_S3, files[i]);
                try {
                    await fileTransferUtility.UploadAsync(files[i], bucketName);
                }
                catch (Exception e) {
                    Debug.Log(e);
                    EditorUtility.ClearProgressBar();
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private static void ClearAddressablesDirectory(BuildTarget target, ProfileType profile)
        {
            if (target != BuildTarget.Android && target != BuildTarget.iOS)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, "Unexpected build target!");
                return;
            }

            if (profile == ProfileType.None)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, "Unexpected profile type!");
                return;
            }
            
            string filePath = Application.dataPath.Replace("Assets", "") + $"ServerData/{profile}/{target}";
            if (Directory.Exists(filePath) == true)
            {
                string[] files = Directory.GetFiles(filePath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
        }

        private static void SetBuildAddressablesWithPlayerBuildOption(AddressableAssetSettings.PlayerBuildOption option)
        {
            if (settings == null)
            {
                getSettingsObject(ADDRESSABLES_BUILD_SETTINGS);
            }

            settings.BuildAddressablesWithPlayerBuild = option;
        }

        private static bool PerformAddressablesBuild(string profile)
        {
            getSettingsObject(ADDRESSABLES_BUILD_SETTINGS);
            setProfile(profile);
            IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(ADDRESSABLES_BUILD_SCRIPT) as IDataBuilder;
            
            if (builderScript == null) {
                LogManager.LogError(LogManager.LogType.EXCEPTION, ADDRESSABLES_BUILD_SCRIPT + " couldn't be found or isn't a build script.");
                return false;
            }

            setBuilder(builderScript);
            builderScript.ClearCachedData();

            return buildAddressableContent();
        }

        private static bool PerformAddressablesUpdate(string profile)
        {
            getSettingsObject(ADDRESSABLES_BUILD_SETTINGS);
            setProfile(profile);
            return updateAddressableContent();
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

        static bool updateAddressableContent()
        {
            var result = ContentUpdateScript.BuildContentUpdate(settings, $"{settings.ContentStateBuildPath}/addressables_content_state.bin");
            
            bool success = string.IsNullOrEmpty(result.Error);
            
            if (!success) {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            return success;
        }

        [MenuItem("JimmyTools/Builder/OpenBuildDirectory")]
        public static void OpenBuildDirectory()
        {
            OpenFileBrowser(Path.GetFullPath(BUILD_PATH));
        }

        private static void OpenFileBrowser(string path)
        {
#if UNITY_EDITOR_WIN
            if (Directory.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"Failed to open build file path : {path}");
            }
#elif UNITY_EDITOR_OSX
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
#endif
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
            // PlayerSettings.Android.keystorePass = KEYSTORE_PASSWORD;
            // PlayerSettings.Android.keyaliasPass = KEYSTORE_PASSWORD;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"{Application.productName}_v{PlayerSettings.bundleVersion}_{timeStamp}.apk";
            return fileName;
        }

#if UNITY_IOS
#else
#endif
    }
#endif
}
