using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientTemplate
{
    namespace UIRegion
    {
        using UIRegion.UIInfo;
        public class UISystem : MonoBehaviour
        {
            public Canvas NormalUICanvas;
            public Canvas LoadingUICanvas;
            public Canvas AlertUICanvas;
            public Camera UICamera;
            public GameObject UILoadingWindow;
            public GameObject UIWaitingWindow;
            
            public void Init()
            {
                UILoadingWindow.gameObject.SetActive(false);
                UIWaitingWindow.gameObject.SetActive(false);
                    
                var NormalUICanvasScaler = NormalUICanvas.GetComponent<CanvasScaler>();
                var LoadingUICanvasScaler = LoadingUICanvas.GetComponent<CanvasScaler>();
                var AlertUICanvasScaler = AlertUICanvas.GetComponent<CanvasScaler>();
                Vector2 ScreenResolution = new Vector2(Screen.width, Screen.height);
                    
                NormalUICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                NormalUICanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                NormalUICanvasScaler.referenceResolution = ScreenResolution;
                NormalUICanvasScaler.matchWidthOrHeight = 0;
                    
                LoadingUICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                LoadingUICanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                LoadingUICanvasScaler.referenceResolution = ScreenResolution;
                LoadingUICanvasScaler.matchWidthOrHeight = 0;
                
                AlertUICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                AlertUICanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                AlertUICanvasScaler.referenceResolution = ScreenResolution;
                AlertUICanvasScaler.matchWidthOrHeight = 0;
            }

            public void Release()
            {

            }

            public Transform GetUICanvasTransform(UICanvasType type)
            {
                switch (type)
                {
                    case UICanvasType.Normal:
                    {
                        return NormalUICanvas.transform;
                    }
                    case UICanvasType.Alert:
                    {
                        return AlertUICanvas.transform;
                    }
                    case UICanvasType.Loading:
                    {
                        return LoadingUICanvas.transform;
                    }
                }

                return NormalUICanvas.transform;
            }

            public void SetActiveLoadingWindow(bool active)
            {
                UILoadingWindow.SetActive(active);
            }

            public void SetActiveWaitingWindow(bool active)
            {
                UIWaitingWindow.SetActive(active);
            }
        }
    }
}
