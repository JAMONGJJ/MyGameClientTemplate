using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UtilityFunctions;
using UnityEngine;

namespace ClientTemplate
{
    public class GameEntryManager : MonoManager<GameEntryManager>
    {
        private UIGameEntryWindow GameEntryWindow;
        
        public override void Init()
        {
            GameEntryWindow = GameObject.FindWithTag("GameEntryWindow").GetComponent<UIGameEntryWindow>();
        }

        public override void Release()
        {
            
        }

        public void GameEntry()
        {
            GameEntryWindow.Init();
            CheckAvailableAppVersion();
        }

        public void SetActivePlayButton(bool active)
        {
            GameEntryWindow.SetActivePlayButton(active);
        }

        public void NoticeBundleSize(long bundleSize)
        {
            GameEntryWindow.SetActiveAssetDownload(true);
            GameEntryWindow.SetAssetDownloadText(bundleSize);
        }

        public void LoadAssetBundles()
        {
            GameEntryWindow.SetActiveDownloadSlider(true);
            GameEntryWindow.SetActiveAssetDownload(false);
            Core.System.Resource.LoadAddressablesAssets();
            StartCoroutine(AssetDownloadProgress());
        }

        public void OpenStoreLink()
        {
            string link = Core.System.Version.GetStoreLink();
            if (string.IsNullOrEmpty(link) == true)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"Store link is null!");
                return;
            }

            Application.OpenURL(link);
        }

        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        private void CheckAvailableAppVersion()
        {
            VersionType versionType = Core.System.Version.GetVersionType();
            switch (versionType)
            {
                case VersionType.Play:  // 앱 플레이 가능
                {
                    StateMachine.NextState(new InitialDataLoadState());
                }
                    break;
                case VersionType.Update:    // 앱 업데이트 필요(스토어로 이동)
                {
                    GameEntryWindow.SetActiveGoToStore(true);
                }
                    break;
                case VersionType.Inspect:   // 앱 점검 중(진입 불가)
                {
                    GameEntryWindow.SetActiveInspect(true);
                }
                    break;
                default:
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, $"Unexpected version type! -> {versionType}");
                }
                    break;
            }
        }

        private IEnumerator AssetDownloadProgress()
        {
            yield return null;
            while (Core.System.Resource.IsDownloadingAssetBundles() == true)
            {
                float progress = Core.System.Resource.GetAssetBundleDownloadProgress();
                GameEntryWindow.SetLoadingProgress(progress);
            }
        }
    }
}
