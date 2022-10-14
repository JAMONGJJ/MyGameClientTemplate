using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using KlayLand.ResourceInfo;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace KlayLand
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
            _normalUiCanvas = GameObject.FindWithTag("NormalUiCanvas").GetComponent<Canvas>();
            _loadingUiCanvas = GameObject.FindWithTag("LoadingUiCanvas").GetComponent<Canvas>();
            _alertUiCanvas = GameObject.FindWithTag("AlertUiCanvas").GetComponent<Canvas>();
            _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            _uiWaitingWindow = GameObject.FindWithTag("WaitingWindow");
            _uiWaitingWindow.SetActive(false);
            _uiLoadingWindow = GameObject.FindWithTag("LoadingWindow");
            _uiLoadingWindow.SetActive(false);

            _iuiWindowAssetTypeContainer.Add(UIWindowType.MainHud, UICanvasType.Normal, UIWindowAssetType.MainHud);
            _iuiWindowAssetTypeContainer.Add(UIWindowType.CustomizeWindow, UICanvasType.Normal, UIWindowAssetType.CustomizeWindow);
            _iuiWindowAssetTypeContainer.Add(UIWindowType.MyPet, UICanvasType.Normal, UIWindowAssetType.MyPet);
            _iuiWindowAssetTypeContainer.Add(UIWindowType.NoticeWindow, UICanvasType.Normal, UIWindowAssetType.NoticeWindow);
            _iuiWindowAssetTypeContainer.Add(UIWindowType.MyProfile, UICanvasType.Normal, UIWindowAssetType.MyProfile);
        }
        
        public override void Release()
        {

        }
        
        public override void ReSet()
        {

        }

        public void SetOverlayCamera()
        {
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(_uiCamera);
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
                        _iuiWindowContainer.Add(windowScript);
                        LogManager.Log(LogManager.LogType.DEFAULT, $"{windowType} init!");
                        if (data == null)
                        {
                            windowScript.Init();
                        }
                        else
                        {
                            windowScript.Init(data);
                        }
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
        public void CloseWindow()
        {
            UIWindow lastWindow = _iuiWindowContainer.RemoveAtLast();
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
            }

            ExecuteOnTop();
        }

        /// <summary>
        /// window template T를 이용해 맨 위가 아닌 특정 UI window를 닫는 메소드.
        /// GetComponent를 사용하도록 구현돼있어서 performance를 많이 잡아먹습니다.
        /// 최대한 사용을 덜하는 방향으로 작업해야 할 듯합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseWindow<T>() where T : UIWindow
        {
            UIWindow window = _iuiWindowContainer.Remove<T>();
            if (window == null)
            {
                Debug.LogError("Entered ui window does not exist in the container!");
                return;
            }
            else
            {
                LogManager.Log(LogManager.LogType.DEFAULT, $"{window.WindowType} release!");
                window.Release();
                Destroy(window.gameObject);
            }
        }

        /// <summary>
        /// window template T를 이용해 특정 UI window를 찾는 메소드.
        /// 찾는 window가 없으면 null 리턴합니다.
        /// GetComponent를 사용하도록 구현돼있어서 performance를 많이 잡아먹습니다.
        /// 최대한 사용을 덜하는 방향으로 작업해야 할 듯합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public UIWindow GetWindow<T>() where T : UIWindow
        {
            return _iuiWindowContainer.Find<T>();
        }
        
        /// <summary>
        /// UI window마다 자신이 맨 위에 노출되었을때 수행해야하는 작업이 있다면,
        /// 해당 작업을 수행하게끔 하는 메소드.
        /// </summary>
        private void ExecuteOnTop()
        {
            UIWindow windowOnTop = _iuiWindowContainer.AtLast();
            if (windowOnTop != null)
            {
                windowOnTop.OnTop();
            }
        }
        
        /// <summary>
        /// MainHud prefab 생성
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
        /// 대기 화면 생성
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
        /// 로딩 화면 생성
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
