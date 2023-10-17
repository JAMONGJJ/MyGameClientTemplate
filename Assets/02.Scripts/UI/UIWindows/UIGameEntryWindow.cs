using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace ClientTemplate
{
    public enum SizeType
    {
        bytes,
        Kbs,
        Mbs,
        Gbs,
        
    }

    public class UIGameEntryWindow : MonoBehaviour
    {
        public GameObject LoadingBackground;
        public GameObject GoToStoreBackground;
        public GameObject DownloadNoticeBackground;
        public TMP_Text LoadingProgressText;
        public Slider LoadingProgressSlider;
        public Button PlayButton;
        public Button GoToStoreButton;
        public Button AcceptAssetDownloadButton;
        public Button DenyAssetDownloadButton;
        public TMP_Text DownloadAssetDescriptionText;

        private string bundleSize;

        public void Init()
        {
            LoadingBackground.SetActive(true);
            LoadingProgressSlider.gameObject.SetActive(false);
            PlayButton.gameObject.SetActive(false);
            GoToStoreBackground.SetActive(false);
            DownloadNoticeBackground.SetActive(false);
            SetButtons();
        }

        public void Release()
        {
            
        }

        private void SetButtons()
        {
            PlayButton.onClick.AddListener(() =>
            {
                GameEntryManager.Instance.DestroySelf();
                StateMachine.NextState(new LobbyState());
            });
            
            GoToStoreButton.onClick.AddListener(() =>
            {
                VersionsDataTable version = Info.Table.GetVersionInfo();
#if UNITY_ANDROID
                Application.OpenURL($"{version.playStoreLink}");
#elif UNITY_IOS
                Application.OpenURL($"{version.appStoreLink}");
#else
                LogManager.LogError(LogManager.LogType.DEFAULT, "unexpected platform!");
#endif
            });

            AcceptAssetDownloadButton.onClick.AddListener(() =>
            {
                GameEntryManager.Instance.GameEntryWindow.SetActiveDownloadSlider(true);
                GameEntryManager.Instance.GameEntryWindow.SetActiveAssetDownload(false);
                Core.System.Resource.LoadAddressablesAssets();
            });

            DenyAssetDownloadButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        public void SetActiveDownloadSlider(bool active)
        {
            LoadingProgressSlider.gameObject.SetActive(active);
        }

        public void SetActiveGoToStore(bool active)
        {
            GoToStoreBackground.gameObject.SetActive(active);
        }

        public void SetActiveAssetDownload(bool active)
        {
            DownloadNoticeBackground.gameObject.SetActive(active);
        }

        public void SetActivePlayButton(bool active)
        {
            PlayButton.gameObject.SetActive(active);
        }

        public void SetAssetDownloadText(long size)
        {
            string format = "You need to download {0} of additional data!";
            bundleSize = ConvertSize(size);
            DownloadAssetDescriptionText.text = string.Format(format, bundleSize);
        }

        public void SetLoadingSliderValue(object args, float percentage)
        {
            LoadingProgressSlider.value = percentage;
        }

        private void SetLoadingProgressText()
        {
            string format = "Download progress : {0} / {1}";
            LoadingProgressText.text = string.Format(format, "", bundleSize);
        }

        private string ConvertSize(long size)
        {
            string result = "";
            float tmpSize = size;
            SizeType type = SizeType.bytes;
            while (tmpSize > 1024.0f)
            {
                tmpSize = tmpSize / 1024.0f;
                type++;
            }

            result = $"{tmpSize.ToString("F2")}{type}";
            return result;
        }
    }
}                                                                                                                  
