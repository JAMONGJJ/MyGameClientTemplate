using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UtilityFunctions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace ClientTemplate
{
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

        private long bundleSize;
        private string bundleSizeTextFormat;
        
        public void Init()
        {
            LoadingBackground.SetActive(true);
            LoadingProgressSlider.gameObject.SetActive(false);
            PlayButton.gameObject.SetActive(false);
            GoToStoreBackground.SetActive(false);
            DownloadNoticeBackground.SetActive(false);
            SetTextFormats();
            SetButtons();
        }

        public void Release()
        {
            
        }

        private void SetTextFormats()
        {
            bundleSizeTextFormat = "You need to download {0} of additional data!";
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
                GameEntryManager.Instance.OpenStoreLink();
            });

            AcceptAssetDownloadButton.onClick.AddListener(() =>
            {
                GameEntryManager.Instance.LoadAssetBundles();
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
            bundleSize = size;
            string convertedSize = Utility.Functions.ConvertByteLongToString(size);
            DownloadAssetDescriptionText.text = string.Format(bundleSizeTextFormat, convertedSize);
        }

        public void SetLoadingProgress(float percentage)
        {
            LoadingProgressSlider.value = percentage;
        }
    }
}                                                                                                                  
