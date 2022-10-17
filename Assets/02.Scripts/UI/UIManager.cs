using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTemplate.ResourceInfo;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ClientTemplate
{
    using UIInfo;

    public class UIManager : MonoManager<UIManager>
    {
        private IUIWindowContainer _uiWindowContainer;
        private IUIWindowAssetTypeContainer _uiWindowAssetTypeContainer;
        private IUIDataInfoContainer _uiDataInfoContainer;
        private Canvas _normalUiCanvas;
        private Canvas _loadingUiCanvas;
        private Canvas _alertUiCanvas;
        private Camera _uiCamera;
        private GameObject _uiWaitingWindow;
        private GameObject _uiLoadingWindow;

        public MainHud MyMainHud { get; private set; }

        public void SetUIWindowContainer(IUIWindowContainer container)
        {
            _uiWindowContainer = container;
        }
        
        public void SetUIWindowAssetTypeContainer(IUIWindowAssetTypeContainer container)
        {
            _uiWindowAssetTypeContainer = container;
        }
        
        public void SetUIDataInfoContainer(IUIDataInfoContainer container)
        {
            _uiDataInfoContainer = container;
        }

        public override void Init()
        {
            _normalUiCanvas = GameObject.FindWithTag("NormalUICanvas").GetComponent<Canvas>();
            _loadingUiCanvas = GameObject.FindWithTag("LoadingUICanvas").GetComponent<Canvas>();
            _alertUiCanvas = GameObject.FindWithTag("AlertUICanvas").GetComponent<Canvas>();
            _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            _uiWaitingWindow = GameObject.FindWithTag("WaitingWindow");
            _uiWaitingWindow.SetActive(false);
            _uiLoadingWindow = GameObject.FindWithTag("LoadingWindow");
            _uiLoadingWindow.SetActive(false);

            _uiWindowAssetTypeContainer.Add(UIWindowType.TestModalessUIWindow, UICanvasType.Normal, UIWindowAssetType.TestModalessUIWindow, false);
            _uiWindowAssetTypeContainer.Add(UIWindowType.TestModalessUIWindow2, UICanvasType.Normal, UIWindowAssetType.TestModalessUIWindow2, false);
            _uiWindowAssetTypeContainer.Add(UIWindowType.MainHud, UICanvasType.Normal, UIWindowAssetType.MainHud, true);
            _uiWindowAssetTypeContainer.Add(UIWindowType.NoticeWindow, UICanvasType.Normal, UIWindowAssetType.NoticeWindow, true);
        }
        
        public override void Release()
        {
            _uiWindowContainer = null;
            _uiWindowAssetTypeContainer = null;
            _uiDataInfoContainer = null;
            _normalUiCanvas = null;
            _loadingUiCanvas = null;
            _alertUiCanvas = null;
            _uiCamera = null;
            _uiWaitingWindow = null;
            _uiLoadingWindow = null;
        }
        
        public override void ReSet()
        {
            Release();
            Init();
        }

        public void SetOverlayCamera()
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_uiCamera);
        }
        
        /// <summary>
        /// 입력받은 파라미터를 이용해 그에 맞는 UI Window 프리팹을 생성.
        /// Address를 이용해 프리팹을 로드해오기 때문에 async로 동작함.
        /// </summary>
        /// <param name="windowType"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public async void OpenWindow<T>(UIWindowType windowType, UIData data = null) where T : UIWindow
        {
            try
            {
                if (_uiDataInfoContainer.Contains(windowType) == true)
                {
                    throw new Exception("Entered ui window is already instantiated!");
                }
                
                UIWindowAssetType assetType = _uiWindowAssetTypeContainer.GetAssetType(windowType);
                if (assetType == UIWindowAssetType.None)
                {
                    throw new Exception("Entered ui window type does not exist!");
                }
                
                UICanvasType canvasType = _uiWindowAssetTypeContainer.GetCanvasType(windowType);
                string assetAddress = Core.System.Resource.GetAddressByType(assetType);
                
                if (string.IsNullOrEmpty(assetAddress) == true)
                {
                    throw new Exception("Entered asset address does not exist!");
                }
                
                AsyncOperationHandle<GameObject> handle = Core.System.Resource.LoadGameObject(assetAddress);
                OpenWaitingWindow();
                while (handle.IsDone == false)
                {
                    await Task.Delay(10);
                }
                CloseWaitingWindow();
                
                GameObject windowGameObject = Instantiate(handle.Result, GetUICanvas(canvasType));
                if (windowGameObject == null)
                {
                    throw new Exception("Entered ui window does not exist in the path!");
                }
                
                T windowScript = windowGameObject.GetComponent<T>();
                windowScript.SetWindowType(windowType);
                bool isModal = _uiWindowAssetTypeContainer.GetModalType(windowType);
                windowScript.SetIsModal(isModal);
                _uiWindowContainer.Add(windowScript);
                _uiDataInfoContainer.Add(windowType, data);
                LogManager.Log(LogManager.LogType.DEFAULT, $"{windowType} init!");
                windowScript.Init(data);
                Addressables.Release(handle);
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }
        
        private Transform GetUICanvas(UICanvasType canvasType)
        {
            switch (canvasType)
            {
                case UICanvasType.Normal:
                {
                    return _normalUiCanvas.transform;
                }
                case UICanvasType.Alert:
                {
                    return _alertUiCanvas.transform;
                }
                case UICanvasType.Loading:
                {
                    return _loadingUiCanvas.transform;
                }
            }

            return _normalUiCanvas.transform;
        }

        /// <summary>
        /// 맨 위에 노출되어 있는 UI window를 닫는 메소드.
        /// </summary>
        public void CloseWindow(UIWindowType type)
        {
            try
            {
                UIWindow lastWindow = _uiWindowContainer.RemoveAtLast(type);
                _uiDataInfoContainer.Remove(type);
                if (lastWindow == null)
                {
                    throw new Exception("Entered ui window does not exist in the container!");
                }
                
                LogManager.Log(LogManager.LogType.DEFAULT, $"{lastWindow.WindowType} release!");
                lastWindow.Release();
                Destroy(lastWindow.gameObject);
                ExecuteOnTop();
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }

        public void RefreshUIData(UIWindowType type, UIData data)
        {
            if (_uiDataInfoContainer.Refresh(type, data) == false)
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, "Entered ui window type does not exist in the container!");
            }
        }

        /// <summary>
        /// UI window마다 자신이 맨 위에 노출되었을때 수행해야하는 작업이 있다면,
        /// 해당 작업을 수행하게끔 하는 메소드.
        /// </summary>
        private void ExecuteOnTop()
        {
            try
            {
                IUIWindows windowsOnTop = _uiWindowContainer.AtLast();
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
                        UIData data = _uiDataInfoContainer.GetUIData(windowOnTop.WindowType);
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
                        
                    UIData data = _uiDataInfoContainer.GetUIData(windowOnTop.WindowType);
                    LogManager.Log(LogManager.LogType.DEFAULT, $"{windowOnTop.WindowType} on top!");
                    windowOnTop.OnTop(data);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }
        
        /// <summary>
        /// MainHud 오브젝트 생성
        /// </summary>
        public async void CreateMainHud()
        {
            try
            {
                UIWindowAssetType assetType = _uiWindowAssetTypeContainer.GetAssetType(UIWindowType.MainHud);
                if(assetType == UIWindowAssetType.None)
                {
                    throw new Exception("MainHud window type does not exist!");
                }
                
                string assetAddress = Core.System.Resource.GetAddressByType(assetType);
                if (string.IsNullOrEmpty(assetAddress) == true)
                {
                    throw new Exception("MainHud asset address does not exist!");
                }
                
                AsyncOperationHandle<GameObject> handle = Core.System.Resource.LoadGameObject(assetAddress);
                OpenWaitingWindow();
                while (handle.IsDone == false)
                {
                    await Task.Delay(10);
                }
                CloseWaitingWindow();
                GameObject windowGameObject = Instantiate(handle.Result, GetUICanvas(UICanvasType.Normal));
                if (windowGameObject == null)
                {
                    throw new Exception("MainHud window does not exist in the path!");
                }
                
                MainHud windowScript = windowGameObject.GetComponent<MainHud>();
                windowScript.SetWindowType(UIWindowType.MainHud);
                windowScript.Init();
                MyMainHud = windowScript;
                LogManager.Log(LogManager.LogType.DEFAULT, $"{UIWindowType.MainHud} init!");

                Addressables.Release(handle);
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }
        
        /// <summary>
        /// MainHud 오브젝트 삭제
        /// </summary>
        public void DestroyMainHud()
        {
            try
            {
                if (MyMainHud == null)
                {
                    throw new Exception("MyMainHud is null!");
                }

                MyMainHud.Release();
                Destroy(MyMainHud.gameObject);
                MyMainHud = null;
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }

        /// <summary>
        /// 대기 화면 표시
        /// </summary>
        public void OpenWaitingWindow()
        {
            _uiWaitingWindow.SetActive(true);
        }

        /// <summary>
        /// 대기 화면 삭제
        /// </summary>
        public void CloseWaitingWindow()
        {
            _uiWaitingWindow.SetActive(false);
        }

        /// <summary>
        /// 로딩 화면 표시
        /// </summary>
        public void OpenLoadingWindow()
        {
            _uiLoadingWindow.SetActive(true);
        }

        /// <summary>
        /// 로딩 화면 삭제
        /// </summary>
        public void CloseLoadingWindow()
        {
            _uiLoadingWindow.SetActive(false);
        }
    }
}
