using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTemplate.ResourceInfo;
using ClientTemplate.UtilityFunctions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace ClientTemplate
{
    using UIInfo;

    public class UIManager : MonoManager<UIManager>
    {
        private IUIWindowContainer UIWindowContainer;
        private IUIWindowAssetTypeContainer UIWindowAssetTypeContainer;
        private IUIDataInfoContainer UIDataInfoContainer;
        private Canvas NormalUICanvas;
        private Canvas LoadingUICanvas;
        private Canvas AlertUICanvas;
        private Camera UICamera;
        private GameObject UIWaitingWindow;
        private GameObject UILoadingWindow;

        public Vector2 ScreenResolution { get; private set; }
        
        public MainHud MyMainHud { get; private set; }

        public void SetUIWindowContainer(IUIWindowContainer container)
        {
            UIWindowContainer = container;
        }
        
        public void SetUIWindowAssetTypeContainer(IUIWindowAssetTypeContainer container)
        {
            UIWindowAssetTypeContainer = container;
        }
        
        public void SetUIDataInfoContainer(IUIDataInfoContainer container)
        {
            UIDataInfoContainer = container;
        }

        public override void Init()
        {
            UIWindowAssetTypeContainer.Add(UIWindowType.TestModalessUIWindow, UICanvasType.Normal, UIWindowAssetType.TestModalessUIWindow, false);
            UIWindowAssetTypeContainer.Add(UIWindowType.TestModalessUIWindow2, UICanvasType.Normal, UIWindowAssetType.TestModalessUIWindow2, false);
            UIWindowAssetTypeContainer.Add(UIWindowType.MainHud, UICanvasType.Normal, UIWindowAssetType.MainHud, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.NoticeWindow, UICanvasType.Normal, UIWindowAssetType.NoticeWindow, true);
        }
        
        public override void Release()
        {
            UIWindowContainer = null;
            UIWindowAssetTypeContainer = null;
            UIDataInfoContainer = null;
        }
        
        public override void ReSet()
        {
            Release();
            Init();
        }

        public void PushToMainCameraStack()
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);
        }

        public void LoadUISystem()
        {
            Utility.Functions.Exception.Process(() =>
            {
                LogManager.Log(LogManager.LogType.DEFAULT, "UISystem load start!");
                GameObject tmpObject = Core.System.Resource.LoadAssets(UIWindowAssetType.UISystem);

                Instantiate(tmpObject);
                FindUIObjects();

                LogManager.Log(LogManager.LogType.DEFAULT, "UISystem load completed!");
            });
        }

        private void FindUIObjects()
        {
            NormalUICanvas = GameObject.FindWithTag("NormalUICanvas").GetComponent<Canvas>();
            LoadingUICanvas = GameObject.FindWithTag("LoadingUICanvas").GetComponent<Canvas>();
            AlertUICanvas = GameObject.FindWithTag("AlertUICanvas").GetComponent<Canvas>();
            UICamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            UIWaitingWindow = GameObject.FindWithTag("WaitingWindow");
            UIWaitingWindow.SetActive(false);
            UILoadingWindow = GameObject.FindWithTag("LoadingWindow");
            UILoadingWindow.gameObject.SetActive(false);
                
            var NormalUICanvasScaler = NormalUICanvas.GetComponent<CanvasScaler>();
            var LoadingUICanvasScaler = LoadingUICanvas.GetComponent<CanvasScaler>();
            var AlertUICanvasScaler = AlertUICanvas.GetComponent<CanvasScaler>();
            ScreenResolution = new Vector2(Screen.width, Screen.height);
                
            NormalUICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            NormalUICanvasScaler.referenceResolution = ScreenResolution;
            NormalUICanvasScaler.matchWidthOrHeight = 0;
                
            LoadingUICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            LoadingUICanvasScaler.referenceResolution = ScreenResolution;
            LoadingUICanvasScaler.matchWidthOrHeight = 0;
                
            AlertUICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            AlertUICanvasScaler.referenceResolution = ScreenResolution;
            AlertUICanvasScaler.matchWidthOrHeight = 0;
        }
        
        /// <summary>
        /// 입력받은 파라미터를 이용해 그에 맞는 UI Window 프리팹을 생성.
        /// Address를 이용해 프리팹을 로드해오기 때문에 async로 동작함.
        /// </summary>
        /// <param name="windowType"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void OpenWindow<T>(UIWindowType windowType, UIData data = null) where T : UIWindow
        {
            Utility.Functions.Exception.Process(() =>
            {
                if (UIDataInfoContainer.Contains(windowType) == true)
                {
                    throw new Exception($"{windowType} is already instantiated!");
                }
                
                UIWindowAssetType assetType = UIWindowAssetTypeContainer.GetAssetType(windowType);
                if (assetType == UIWindowAssetType.None)
                {
                    throw new Exception("Entered ui window type does not exist!");
                }
                
                UICanvasType canvasType = UIWindowAssetTypeContainer.GetCanvasType(windowType);
                GameObject tmpObject = Core.System.Resource.LoadAssets(assetType);
                
                GameObject windowGameObject = Instantiate(tmpObject, GetUICanvas(canvasType));
                if (windowGameObject == null)
                {
                    throw new Exception($"{windowType} object does not exist in the path!");
                }
                
                T windowScript = windowGameObject.GetComponent<T>();
                windowScript.SetWindowType(windowType);
                bool isModal = UIWindowAssetTypeContainer.GetModalType(windowType);
                windowScript.SetIsModal(isModal);
                UIWindowContainer.Add(windowScript);
                UIDataInfoContainer.Add(windowType, data);
                LogManager.Log(LogManager.LogType.DEFAULT, $"{windowType} init!");
                windowScript.Init(data);
            });
        }
        
        private Transform GetUICanvas(UICanvasType canvasType)
        {
            switch (canvasType)
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

        /// <summary>
        /// 맨 위에 노출되어 있는 UI window를 닫는 메소드.
        /// </summary>
        public void CloseWindow(UIWindowType type)
        {
            Utility.Functions.Exception.Process(() =>
            {
                UIWindow lastWindow = UIWindowContainer.RemoveAtLast(type);
                UIDataInfoContainer.Remove(type);
                if (lastWindow == null)
                {
                    throw new Exception("Entered ui window does not exist in the container!");
                }
                
                LogManager.Log(LogManager.LogType.DEFAULT, $"{lastWindow.WindowType} release!");
                lastWindow.Release();
                Destroy(lastWindow.gameObject);
                ExecuteOnTop();
            });
        }

        public void CloseAllWindows()
        {
            Utility.Functions.Exception.Process(() =>
            {
                while (UIWindowContainer.Count > 0)
                {
                    UIWindow lastWindow = UIWindowContainer.RemoveAtLast();
                    UIDataInfoContainer.Remove(lastWindow.WindowType);
                    UIDataInfoContainer = new UIDataInfoContainer();
                    LogManager.Log(LogManager.LogType.DEFAULT, $"{lastWindow.WindowType} release!");
                    lastWindow.Release();
                    Destroy(lastWindow.gameObject);
                }
            });
        }

        public void RefreshUIData(UIWindowType type, UIData data)
        {
            Utility.Functions.Exception.Process(() =>
            {
                if (UIDataInfoContainer.Refresh(type, data) == false)
                {
                    throw new Exception( "Entered ui window type does not exist in the container!");
                }
                else
                {
                    ExecuteOnTop();
                }
            });
        }

        /// <summary>
        /// UI window마다 자신이 맨 위에 노출되었을때 수행해야하는 작업이 있다면,
        /// 해당 작업을 수행하게끔 하는 메소드.
        /// </summary>
        private void ExecuteOnTop()
        {
            Utility.Functions.Exception.Process(() =>
            {
                IUIWindows windowsOnTop = UIWindowContainer.AtLast();
                if (windowsOnTop == null)
                {
                    throw new Exception("There is no UIWindow exists in the container!");
                }
                
                if (windowsOnTop.IsModalessContainer() == true)
                {
                    ModalessUIWindowContainer container = windowsOnTop as ModalessUIWindowContainer;
                    if (container == null)
                    {
                        throw new Exception("ModalessUIWindowContainer is null");
                    }
                        
                    foreach (UIWindow windowOnTop in container.ModalessWindowsList)
                    {
                        UIData data = UIDataInfoContainer.GetUIData(windowOnTop.WindowType);
                        LogManager.Log(LogManager.LogType.DEFAULT, $"{windowOnTop.WindowType} on top!");
                        windowOnTop.OnTop(data);
                    }
                }
                else
                {
                    UIWindow windowOnTop = windowsOnTop as UIWindow;
                    if (windowOnTop == null)
                    {
                        throw new Exception("UIWindow is null");
                    }
                        
                    UIData data = UIDataInfoContainer.GetUIData(windowOnTop.WindowType);
                    LogManager.Log(LogManager.LogType.DEFAULT, $"{windowOnTop.WindowType} on top!");
                    windowOnTop.OnTop(data);
                }
            });
        }
        
        /// <summary>
        /// MainHud 오브젝트 생성
        /// </summary>
        public void CreateMainHud()
        {
            Utility.Functions.Exception.Process(() =>
            {
                UIWindowAssetType assetType = UIWindowAssetTypeContainer.GetAssetType(UIWindowType.MainHud);
                if(assetType == UIWindowAssetType.None)
                {
                    throw new Exception($"{UIWindowType.MainHud} window type does not exist!");
                }
                
                GameObject tmpObject = Core.System.Resource.LoadAssets(assetType);
                GameObject windowGameObject = Instantiate(tmpObject, GetUICanvas(UICanvasType.Normal));
                if (windowGameObject == null)
                {
                    throw new Exception($"{UIWindowType.MainHud} window does not exist in the path!");
                }
                
                MainHud windowScript = windowGameObject.GetComponent<MainHud>();
                windowScript.SetWindowType(UIWindowType.MainHud);
                windowScript.Init();
                MyMainHud = windowScript;
                LogManager.Log(LogManager.LogType.DEFAULT, $"{UIWindowType.MainHud} init!");
            });
        }
        
        /// <summary>
        /// MainHud 오브젝트 삭제
        /// </summary>
        public void DestroyMainHud()
        {
            Utility.Functions.Exception.Process(() =>
            {
                if (MyMainHud == null)
                {
                    throw new Exception("MyMainHud is null!");
                }

                MyMainHud.Release();
                Destroy(MyMainHud.gameObject);
                MyMainHud = null;
            });
        }

        public void SetActiveMainHud()
        {
            MyMainHud.gameObject.SetActive(true);
        }

        public void SetInActiveMainHud()
        {
            MyMainHud.gameObject.SetActive(false);
        }

        /// <summary>
        /// 대기 화면 표시
        /// </summary>
        public void OpenWaitingWindow()
        {
            UIWaitingWindow.SetActive(true);
        }

        /// <summary>
        /// 대기 화면 삭제
        /// </summary>
        public void CloseWaitingWindow()
        {
            UIWaitingWindow.SetActive(false);
        }

        /// <summary>
        /// 로딩 화면 표시
        /// </summary>
        public void OpenLoadingWindow()
        {
            UILoadingWindow.SetActive(true);
        }

        /// <summary>
        /// 로딩 화면 삭제
        /// </summary>
        public void CloseLoadingWindow()
        {
            UILoadingWindow.SetActive(false);
        }
    }
}
