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

        public void SetOverlayCamera()
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);
        }

        public void LoadUISystem()
        {
            Utility.Functions.Async.SetIsProcessing(true);
            Utility.Functions.Exception.Process(() =>
            {
                LogManager.Log(LogManager.LogType.DEFAULT, "UISystem load start!");
                string assetAddress = Core.System.Resource.GetAddressByType(UIWindowAssetType.UISystem);
                if (string.IsNullOrEmpty(assetAddress) == true)
                {
                    throw new Exception("UISystem asset address is null!");
                }

                var handle = Core.System.Resource.LoadGameObject(assetAddress);
                handle.Completed += _ =>
                {
                    if (handle.Status != AsyncOperationStatus.Succeeded)
                    {
                        throw new Exception($"UISystem{assetAddress} asset load has failed!");
                    }

                    Instantiate(handle.Result);
                    FindUIObjects();

                    Addressables.Release(handle);
                    Utility.Functions.Async.SetIsProcessing(false);
                    LogManager.Log(LogManager.LogType.DEFAULT, "UISystem load completed!");
                };
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
                if (UIDataInfoContainer.Contains(windowType) == true)
                {
                    throw new Exception("Entered ui window is already instantiated!");
                }
                
                UIWindowAssetType assetType = UIWindowAssetTypeContainer.GetAssetType(windowType);
                if (assetType == UIWindowAssetType.None)
                {
                    throw new Exception("Entered ui window type does not exist!");
                }
                
                UICanvasType canvasType = UIWindowAssetTypeContainer.GetCanvasType(windowType);
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
                bool isModal = UIWindowAssetTypeContainer.GetModalType(windowType);
                windowScript.SetIsModal(isModal);
                UIWindowContainer.Add(windowScript);
                UIDataInfoContainer.Add(windowType, data);
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
            try
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
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }

        public void RefreshUIData(UIWindowType type, UIData data)
        {
            try
            {
                if (UIDataInfoContainer.Refresh(type, data) == false)
                {
                    throw new Exception( "Entered ui window type does not exist in the container!");
                }
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
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
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }
        
        /// <summary>
        /// MainHud 오브젝트 생성
        /// </summary>
        public void CreateMainHud()
        {
            Utility.Functions.Async.SetIsProcessing(true);
            Utility.Functions.Exception.Process(() =>
            {
                UIWindowAssetType assetType = UIWindowAssetTypeContainer.GetAssetType(UIWindowType.MainHud);
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
                handle.Completed += _ =>
                {
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
                    Utility.Functions.Async.SetIsProcessing(false);
                };
            });
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
