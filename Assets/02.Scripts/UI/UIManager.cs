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
        private IUIWindowContainer _iuiWindowContainer;
        private IUIWindowAssetTypeContainer _iuiWindowAssetTypeContainer;
        private Canvas _normalUiCanvas;
        private Canvas _loadingUiCanvas;
        private Canvas _alertUiCanvas;
        private Camera _uiCamera;
        private GameObject _uiWaitingWindow;
        private GameObject _uiLoadingWindow;

        public MainHud MyMainHud { get; private set; }

        public void SetUIWindowContainer(IUIWindowContainer container)
        {
            _iuiWindowContainer = container;
        }
        
        public void SetUIWindowAssetTypeContainer(IUIWindowAssetTypeContainer container)
        {
            _iuiWindowAssetTypeContainer = container;
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

            _iuiWindowAssetTypeContainer.Add(UIWindowType.MainHud, UICanvasType.Normal, UIWindowAssetType.MainHud, true);
            _iuiWindowAssetTypeContainer.Add(UIWindowType.NoticeWindow, UICanvasType.Normal, UIWindowAssetType.NoticeWindow, true);
        }
        
        public override void Release()
        {

        }
        
        public override void ReSet()
        {

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
            UIWindowAssetType assetType = _iuiWindowAssetTypeContainer.GetAssetType(windowType);
            if(assetType == UIWindowAssetType.None)
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, "Entered ui window type does not exist!");
                return;
            }
            else
            {
                UICanvasType canvasType = _iuiWindowAssetTypeContainer.GetCanvasType(windowType);
                string assetAddress = Core.System.Resource.GetAddressByType(assetType);
                if (string.IsNullOrEmpty(assetAddress) == true)
                {
                    LogManager.LogError(LogManager.LogType.DEFAULT, "Entered asset address does not exist!");
                    return;
                }
                else
                {
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
                        Debug.LogError("Entered ui window does not exist in the path!");
                        return;
                    }
                    else
                    {
                        T windowScript = windowGameObject.GetComponent<T>();
                        windowScript.SetWindowType(windowType);
                        bool isModal = _iuiWindowAssetTypeContainer.GetModalType(windowType);
                        windowScript.SetIsModal(isModal);
                        _iuiWindowContainer.Add(windowScript);
                        LogManager.Log(LogManager.LogType.DEFAULT, $"{windowType} init!");
                        windowScript.Init(data);
                    }

                    Addressables.Release(handle);
                }
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
            UIWindow lastWindow = _iuiWindowContainer.RemoveAtLast(type);
            if (lastWindow == null)
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, "Entered ui window does not exist in the container!");
                return;
            }
            else
            {
                LogManager.Log(LogManager.LogType.DEFAULT, $"{lastWindow.WindowType} release!");
                lastWindow.Release();
                Destroy(lastWindow.gameObject);
                ExecuteOnTop();
            }
        }
        
        /// <summary>
        /// UI window마다 자신이 맨 위에 노출되었을때 수행해야하는 작업이 있다면,
        /// 해당 작업을 수행하게끔 하는 메소드.
        /// </summary>
        private void ExecuteOnTop()
        {
            IUIWindows windowsOnTop = _iuiWindowContainer.AtLast();
            if (windowsOnTop != null)
            {
                if (windowsOnTop.IsModalessContainer() == true)
                {
                    ModalessUIWindowContainer container = windowsOnTop as ModalessUIWindowContainer;
                    foreach (UIWindow window in container.ModalessWindowsList)
                    {
                        window.OnTop();
                    }
                }
                else
                {
                    UIWindow windowOnTop = windowsOnTop as UIWindow;
                    windowOnTop.OnTop();
                }
            }
        }
        
        /// <summary>
        /// MainHud 오브젝트 생성
        /// </summary>
        public async void CreateMainHud()
        {
            UIWindowAssetType assetType = _iuiWindowAssetTypeContainer.GetAssetType(UIWindowType.MainHud);
            if(assetType == UIWindowAssetType.None)
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, "MainHud window type does not exist!");
                return;
            }
            else
            {
                string assetAddress = Core.System.Resource.GetAddressByType(assetType);
                if (string.IsNullOrEmpty(assetAddress) == true)
                {
                    LogManager.LogError(LogManager.LogType.DEFAULT, "MainHud asset address does not exist!");
                    return;
                }
                else
                {
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
                        Debug.LogError("MainHud window does not exist in the path!");
                        return;
                    }
                    else
                    {
                        MainHud windowScript = windowGameObject.GetComponent<MainHud>();
                        windowScript.SetWindowType(UIWindowType.MainHud);
                        windowScript.Init();
                        MyMainHud = windowScript;
                        LogManager.Log(LogManager.LogType.DEFAULT, $"{UIWindowType.MainHud} init!");
                    }

                    Addressables.Release(handle);
                }
            }
        }
        
        /// <summary>
        /// MainHud 오브젝트 삭제
        /// </summary>
        public void DestroyMainHud()
        {
            if (MyMainHud != null)
            {
                MyMainHud.Release();
                Destroy(MyMainHud.gameObject);
                MyMainHud = null;
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
