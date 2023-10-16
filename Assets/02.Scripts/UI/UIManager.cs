using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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
        // private Camera UICamera;
        private GameObject UILoadingWindow;
        private GameObject UIWaitingWindow;

        public Rect UIBackgroundSize { get; private set; }

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

        /// <summary>
        /// 보핀에서 사용되는 유니티 UI들의 WindowType, CanvasType, WindowAssetType, isModal 변수 값을 미리 지정하는 함수.
        /// 설정된 값들은 UIWindowAssetTypeContainer에 저장됨.
        /// </summary>
        public void Init()
        {
            UIWindowAssetTypeContainer.Add(UIWindowType.TestModalessUIWindow, UICanvasType.Normal, UIWindowAssetType.TestModalessUIWindow, false);
            UIWindowAssetTypeContainer.Add(UIWindowType.TestModalessUIWindow2, UICanvasType.Normal, UIWindowAssetType.TestModalessUIWindow2, false);
            UIWindowAssetTypeContainer.Add(UIWindowType.MainHud, UICanvasType.Normal, UIWindowAssetType.MainHud, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.NoticeWindow, UICanvasType.Normal, UIWindowAssetType.NoticeWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.CharacterCustomizingWindow, UICanvasType.Normal, UIWindowAssetType.CharacterCustomizingWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.LobbyWindow, UICanvasType.Normal, UIWindowAssetType.LobbyWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.RoomCustomizingWindow, UICanvasType.Normal, UIWindowAssetType.RoomCustomizingWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.FourCutsWindow, UICanvasType.Normal, UIWindowAssetType.FourCutsWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.CharacterCustomizingPresetOptionWindow, UICanvasType.Normal, UIWindowAssetType.CharacterCustomizingPresetOptionWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.EditLobbyWindow, UICanvasType.Normal, UIWindowAssetType.EditLobbyWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.MyStatusWindow, UICanvasType.Normal, UIWindowAssetType.MyStatusWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.IndividualRoomWindow, UICanvasType.Normal, UIWindowAssetType.IndividualRoomWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.AvatarCheckWindow, UICanvasType.Normal, UIWindowAssetType.AvatarCheckWindow, true);
            UIWindowAssetTypeContainer.Add(UIWindowType.RythmCatWindow, UICanvasType.Normal, UIWindowAssetType.RythmCatWindow, true);
            
        }
        
        /// <summary>
        /// 데이터 컨테이너들 초기화
        /// </summary>
        public void Release()
        {
            UIWindowContainer = null;
            UIWindowAssetTypeContainer = null;
            UIDataInfoContainer = null;
        }
        
        public void ReSet()
        {
            Release();
            Init();
        }

        /// <summary>
        /// 프로젝트 초기에는 UI용 카메라가 별도로 존재해서, 메인 카메라의 Stack에 추가해서 사용함.
        /// 그런데 리액트 네이티브와 연동하는 과정에서 Vulkan을 쓸 수 없게 됐는데, Vulkan을 사용하지 않으니 OS 버전이 낮은 기기에서 Screen space - Camera로 설정된 UI Canvas가 렌더링이 안 되는 현상이 발견됨.
        /// 그래서 Screen space - overlay로 변경하고 UI용 카메라를 비활성화함.
        /// </summary>
        public void PushToMainCameraStack()
        {
            // Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);
        }

        /// <summary>
        /// UI 프리팹들이 배치될 부모 오브젝트
        /// UISystem이라는 이름의 프리팹을 만들어 DontDestroyOnLoad 설정 한 후에 UI 프리팹을 사전에 설정된 타입에 따라 NormalUICanvas, LoadingUICanvas, AlertUICanvas에 나눠서 배치함. 
        /// </summary>
        public void LoadUISystem()
        {
            try
            {
                LogManager.Log(LogManager.LogType.DEFAULT, "UISystem load start!");
                GameObjectManager.Instance.InstantiateGameObject(UIWindowAssetType.UISystem,Vector3.zero, Vector3.zero);
                FindUIObjects();
                LogManager.Log(LogManager.LogType.DEFAULT, "UISystem load completed!");
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }

        private void FindUIObjects()
        {
            NormalUICanvas = GameObject.FindWithTag("NormalUICanvas").GetComponent<Canvas>();
            LoadingUICanvas = GameObject.FindWithTag("LoadingUICanvas").GetComponent<Canvas>();
            AlertUICanvas = GameObject.FindWithTag("AlertUICanvas").GetComponent<Canvas>();
            // UICamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            UILoadingWindow = GameObject.FindWithTag("LoadingWindow");
            UILoadingWindow.gameObject.SetActive(false);
            UIWaitingWindow = GameObject.FindWithTag("WaitingWindow");
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
        
        /// <summary>
        /// 입력받은 파라미터를 이용해 그에 맞는 UI Window 프리팹을 생성.
        /// </summary>
        /// <param name="windowType">UI 타입</param>
        /// <param name="data">UI를 보여주는데 필요한 데이터가 있을 경우, UIData를 상속받은 클래스에 데이터를 넣어서 같이 파라미터로 전달</param>
        /// <typeparam name="T">UI 스크립트</typeparam>
        /// <returns>생성된 UI 스크립트, UI가 생성되지 않았다면 null</returns>
        /// <exception cref="Exception"></exception>
        public T OpenWindow<T>(UIWindowType windowType, UIData data = null) where T : UIWindow
        {
            try
            {
                if (UIDataInfoContainer.Contains(windowType) == true)
                {
                    throw new Exception($"{windowType} is already instantiated!");
                }
                
                UIWindowAssetType assetType = UIWindowAssetTypeContainer.GetAssetType(windowType);
                if (assetType == UIWindowAssetType.None)
                {
                    throw new Exception($"Entered ui window type({windowType}) does not exist!");
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
                return windowScript;
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                return null;
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
        /// T 타입의 UI가 현재 생성되어 있다면 해당 UI 오브젝트를 반환하는 함수.
        /// </summary>
        /// <typeparam name="T">찾고자 하는 UI의 스크립트 타입</typeparam>
        /// <returns>입력 받은 UI가 현재 생성되어 있다면, 해당 UI 스크립트 타입 반환함. 혹시 생성되어 있지 않다면 null 반환.</returns>
        public T GetWindow<T>() where T : UIWindow
        {
            return UIWindowContainer.GetWindow<T>();
        }

        /// <summary>
        /// 입력 받은 type의 UI를 생성할 때 사용된 UIData를 반환하는 함수.
        /// </summary>
        /// <param name="type">찾고자 하는 UIData가 사용된 UI의 타입</param>
        /// <typeparam name="T">UIData를 상속받은 스크립트 타입</typeparam>
        /// <returns>입력 받은 타입에 맞는 UIData가 있다면 반환. 없다면 null 반환. 입력 받은 UI가 생성되어 있어도 UIData를 사용하지 않았다면 null 반환함.</returns>
        public T GetUIData<T>(UIWindowType type) where T : UIData
        {
            return UIDataInfoContainer.GetUIData(type) as T;
        }

        /// <summary>
        /// 맨 위에 노출되어 있는 UI window를 닫는 메소드.
        /// </summary>
        public void CloseWindow(UIWindowType type)
        {
            try
            {
                UIWindow lastWindow = UIWindowContainer.RemoveAtLast(type);
                if (lastWindow == null)
                {
                    LogManager.LogError(LogManager.LogType.DEFAULT, $"Entered ui window(type : {type}) does not exist in the container!");
                    return;
                }
                UIDataInfoContainer.Remove(type);
                
                LogManager.Log(LogManager.LogType.DEFAULT, $"{lastWindow.WindowType} release!");
                lastWindow.Release();
                Destroy(lastWindow.gameObject);
                ExecuteOnTop();
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                return;
            }
        }

        /// <summary>
        /// 현재 생성되어 있는 모든 UI 삭제
        /// </summary>
        public void CloseAllWindows()
        {
            try
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
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }

        /// <summary>
        /// 생성되어 있는 UI의 UIData를 갱신함.
        /// 현재 생성되어 있는 UI에 표시된 정보가 바뀌었을 때 주로 사용됨.
        /// </summary>
        /// <param name="type">UIData를 갱신하고자 하는 UI의 타입</param>
        /// <param name="data">새로 갱신할 UIData</param>
        /// <exception cref="Exception"></exception>
        public void RefreshUIData(UIWindowType type, UIData data)
        {
            try
            {
                if (UIDataInfoContainer.Refresh(type, data) == false)
                {
                    throw new Exception( $"Entered ui window type({type}) does not exist in the container!");
                }
                else
                {
                    ExecuteOnTop();
                }
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
            }
        }

        /// <summary>
        /// UI window마다 자신이 맨 위에 노출되었을때 수행해야하는 작업이 있다면, 해당 작업을 수행하게끔 하는 메소드.
        /// UI가 삭제될 때, 혹은 UIData를 갱신했을 때 자동으로 호출됨.
        /// </summary>
        private void ExecuteOnTop()
        {
            try
            {
                IUIWindows windowsOnTop = UIWindowContainer.AtLast();
                if (windowsOnTop == null)
                {
                    LogManager.Log(LogManager.LogType.DEFAULT, "There is no UIWindow exists in the container!");
                    return;
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
                        UIData data = UIDataInfoContainer.GetModifiedUIData(windowOnTop.WindowType);
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
                        
                    UIData data = UIDataInfoContainer.GetModifiedUIData(windowOnTop.WindowType);
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
        /// 로딩 화면 표시
        /// </summary>
        public void OpenLoadingWindow()
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Open Loading Window!");
            StartCoroutine(_OpenLoadingWindow());
        }

        private IEnumerator _OpenLoadingWindow()
        {
            yield return null;
            UILoadingWindow.SetActive(true);
        }

        /// <summary>
        /// 로딩 화면 삭제
        /// </summary>
        public void CloseLoadingWindow()
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Close Loading Window!");
            StartCoroutine(_CloseLoadingWindow());
        }

        private IEnumerator _CloseLoadingWindow()
        {
            yield return null;
            UILoadingWindow.SetActive(false);
        }

        /// <summary>
        /// 대기 화면 표시
        /// </summary>
        public void OpenWaitingWindow()
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Open Waiting Window!");
            StartCoroutine(_OpenWaitingWindow());
        }

        private IEnumerator _OpenWaitingWindow()
        {
            yield return null;
            UIWaitingWindow.SetActive(true);
        }

        /// <summary>
        /// 대기 화면 삭제
        /// </summary>
        public void CloseWaitingWindow()
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Close Waiting Window!");
            StartCoroutine(_CloseWaitingWindow());
        }

        private IEnumerator _CloseWaitingWindow()
        {
            yield return null;
            UIWaitingWindow.SetActive(false);
        }
    }
}
