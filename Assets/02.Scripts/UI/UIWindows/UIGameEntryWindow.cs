using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        public Slider LoadingProcessSlider;
        public GameObject GoToStoreBackground;
        public GameObject DownloadNoticeBackground;
        public Button GoToStoreButton;
        public Button AcceptAssetDownloadButton;
        public Button DenyAssetDownloadButton;
        public TMP_Text DownloadAssetDescriptionText;
        
        private void Start()
        {
            LoadingProcessSlider.gameObject.SetActive(false);
            GoToStoreBackground.gameObject.SetActive(false);
            DownloadNoticeBackground.gameObject.SetActive(false);
            SetButtons();
        }

        private void SetButtons()
        {
            GoToStoreButton.onClick.AddListener(() =>
            {
                VersionsDataTable version = Data.Table.GetVersionInfo();
#if UNITY_ANDROID
                Application.OpenURL($"{version.playStoreLink}");
#elif UNITY_IOS
                Application.OpenURL($"{version.appStoreLink}");
#else
                LogManager.LogError(LogManager.LogType.DEFAULT, "unexpected platform!");
#endif
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
            LoadingProcessSlider.gameObject.SetActive(active);
        }

        public void SetActiveGoToStore(bool active)
        {
            GoToStoreBackground.gameObject.SetActive(active);
        }

        public void SetActiveAssetDownload(bool active)
        {
            DownloadNoticeBackground.gameObject.SetActive(active);
        }

        public void SetAssetDownloadText(long size)
        {
            string format = "You need to download {0} of additional data!";
            DownloadAssetDescriptionText.text = string.Format(format, ConvertSize(size));
        }

        public void SetLoadingSliderValue(float percentage)
        {
            LoadingProcessSlider.value = percentage;
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
