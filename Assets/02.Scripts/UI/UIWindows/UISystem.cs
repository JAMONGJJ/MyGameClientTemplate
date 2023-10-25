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
            public CanvasScaler LetterBoxCanvas;
            public CanvasScaler NormalUICanvas;
            public CanvasScaler LoadingUICanvas;
            public CanvasScaler AlertUICanvas;
            public Camera UICamera;
            public GameObject UILoadingWindow;
            public GameObject UIWaitingWindow;
            public AspectRatioFitter Mask;

            private Transform NormalUIParentTransform;
            private Transform LoadingUIParentTransform;
            private Transform AlertUIParentTransform;
            
            public void Init()
            {
                LetterBoxCanvas.gameObject.SetActive(false);
                UILoadingWindow.gameObject.SetActive(false);
                UIWaitingWindow.gameObject.SetActive(false);

                float aspectRatio = 1920f / 1080f;
                Vector2 pivotResolution = new Vector2(1920, 1080);
                Vector2 deviceResolution = new Vector2(Screen.width, Screen.height);
                float matchWidthOrHeight =
                    (pivotResolution.x / deviceResolution.x) < (pivotResolution.y / deviceResolution.y) ? 1f : 0f; 
                    
                NormalUICanvas.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                NormalUICanvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                NormalUICanvas.referenceResolution = pivotResolution;
                NormalUICanvas.matchWidthOrHeight = matchWidthOrHeight;
                NormalUIParentTransform = NormalUICanvas.transform;
                    
                LoadingUICanvas.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                LoadingUICanvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                LoadingUICanvas.referenceResolution = pivotResolution;
                LoadingUICanvas.matchWidthOrHeight = matchWidthOrHeight;
                LoadingUIParentTransform = LoadingUICanvas.transform;
                
                AlertUICanvas.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                AlertUICanvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                AlertUICanvas.referenceResolution = pivotResolution;
                AlertUICanvas.matchWidthOrHeight = matchWidthOrHeight;
                AlertUIParentTransform = AlertUICanvas.transform;

                if (Core.System.Settings.ResolutionType == ResolutionType.Fixed)
                {
                    GameObject fitterGameObject = new GameObject("AspectRatioFitter");
                    var fitter = fitterGameObject.AddComponent<AspectRatioFitter>();
                    fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    fitter.aspectRatio = aspectRatio;

                    LetterBoxCanvas.gameObject.SetActive(true);
                    Mask.aspectRatio = aspectRatio;
                    
                    GameObject normalUIChild = Instantiate(fitterGameObject, NormalUICanvas.transform);
                    NormalUIParentTransform = normalUIChild.transform;
                        
                    GameObject loadingUIChild = Instantiate(fitterGameObject, LoadingUICanvas.transform);
                    LoadingUIParentTransform = loadingUIChild.transform;
                    UILoadingWindow.transform.SetParent(LoadingUIParentTransform);
                    UIWaitingWindow.transform.SetParent(LoadingUIParentTransform);
                        
                    GameObject alertUIChild = Instantiate(fitterGameObject, AlertUICanvas.transform);
                    AlertUIParentTransform = alertUIChild.transform;
                }
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
                        return NormalUIParentTransform;
                    }
                    case UICanvasType.Loading:
                    {
                        return LoadingUIParentTransform;
                    }
                    case UICanvasType.Alert:
                    {
                        return AlertUIParentTransform;
                    }
                }

                return NormalUIParentTransform;
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
