using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
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
        public GameObject[] LoadingImagesGroup;
        public Slider LoadingProcessSlider;
        public GameObject GoToStoreBackground;
        public GameObject DownloadNoticeBackground;
        public Button GoToStoreButton;
        public Button AcceptAssetDownloadButton;
        public Button DenyAssetDownloadButton;
        public TMP_Text DownloadAssetDescriptionText;
        
        private void Start()
        {
            for (int i = 0; i < LoadingImagesGroup.Length; i++)
            {
                LoadingImagesGroup[i].SetActive(false);
            }
            LoadingProcessSlider.gameObject.SetActive(false);
            GoToStoreBackground.gameObject.SetActive(false);
            DownloadNoticeBackground.gameObject.SetActive(false);
            SetButtons();
            StartCoroutine(Sequencing());
        }

        IEnumerator Sequencing()
        {
            int index = 0;
            while (true)
            {
                SetActiveLoadingImage(index);
                yield return new WaitForSeconds(0.3f);
                index++;
                if (index >= LoadingImagesGroup.Length)
                {
                    index = 0;
                }
            }
        }

        private void SetButtons()
        {
            GoToStoreButton.OnClickAsObservable().Subscribe(_ =>
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

            DenyAssetDownloadButton.OnClickAsObservable().Subscribe(_ =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        private void SetActiveLoadingImage(int index)
        {
            for (int i = 0; i < LoadingImagesGroup.Length; i++)
            {
                if (i == index)
                {
                    LoadingImagesGroup[i].SetActive(true);
                }
                else
                {
                    LoadingImagesGroup[i].SetActive(false);
                }
            }
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
            float mySize = size;
            SizeType type = SizeType.bytes;
            while (mySize > 1024.0f)
            {
                mySize = mySize / 1024.0f;
                type++;
            }

            result = $"{mySize.ToString("F2")}{type}";
            return result;
        }
    }
}                                                                                                                  
