using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace UIInfo
    {
        public enum UIWindowType
        {
            None,
            TestModalessUIWindow,
            TestModalessUIWindow2,
            MainHud,
            NoticeWindow,
            

        }

        public enum UICanvasType
        {
            None,
            Normal,
            Alert,
            Loading,
            
        }
        
        public enum UIWindowAssetType
        {
            None,
            UISystem,
            TestModalessUIWindow,
            TestModalessUIWindow2,
            MainHud,
            NoticeWindow,
            
        }

        public class UIData
        {

        }

        public interface IUIWindows
        {
            bool IsModalessContainer();
        }

        public interface IUIWindowAssetTypeContainer
        {
            void Add(UIWindowType windowType, UICanvasType canvasType, UIWindowAssetType assetType, bool isModal);
            UICanvasType GetCanvasType(UIWindowType type);
            UIWindowAssetType GetAssetType(UIWindowType type);
            bool GetModalType(UIWindowType type);
        }

        public class UIWindowAssetTypeContainer : IUIWindowAssetTypeContainer
        {
            private Dictionary<UIWindowType, UICanvasType> _canvasTypeMap;
            private Dictionary<UIWindowType, UIWindowAssetType> _assetTypeMap;
            private Dictionary<UIWindowType, bool> _isModalMap;

            public UIWindowAssetTypeContainer()
            {
                _assetTypeMap = new Dictionary<UIWindowType, UIWindowAssetType>();
                _canvasTypeMap = new Dictionary<UIWindowType, UICanvasType>();
                _isModalMap = new Dictionary<UIWindowType, bool>();
            }

            public void Add(UIWindowType windowType, UICanvasType canvasType, UIWindowAssetType assetType, bool isModal)
            {
                if (_assetTypeMap.ContainsKey(windowType) == false && _canvasTypeMap.ContainsKey(windowType) == false &&
                    _isModalMap.ContainsKey(windowType) == false)
                {
                    _canvasTypeMap.Add(windowType, canvasType);
                    _assetTypeMap.Add(windowType, assetType);
                    _isModalMap.Add(windowType, isModal);
                }
            }

            public UICanvasType GetCanvasType(UIWindowType type)
            {
                if (_assetTypeMap.ContainsKey(type) == true)
                {
                    return _canvasTypeMap[type];
                }
                
                return UICanvasType.None;
            }

            public UIWindowAssetType GetAssetType(UIWindowType type)
            {
                if (_assetTypeMap.ContainsKey(type) == true)
                {
                    return _assetTypeMap[type];
                }
                
                return UIWindowAssetType.None;
            }

            public bool GetModalType(UIWindowType type)
            {
                if (_isModalMap.ContainsKey(type) == true)
                {
                    return _isModalMap[type];
                }

                return false;
            }
        }

        public class UIWindow : MonoBehaviour, IUIWindows
        {
            public UIWindowType WindowType { get; private set; }
            public bool IsModal { get; private set; }

            public void SetWindowType(UIWindowType type)
            {
                WindowType = type;
            }

            public void SetIsModal(bool modal)
            {
                IsModal = modal;
            }

            public virtual void Init(UIData data = null)
            {

            }

            public virtual void Release()
            {

            }

            public virtual void OnTop(UIData data = null)
            {
                
            }

            public bool IsModalessContainer()
            {
                return false;
            }
        }

        public class ModalessUIWindowContainer : IUIWindows
        {
            public List<UIWindow> ModalessWindowsList { get; private set; }

            public ModalessUIWindowContainer()
            {
                ModalessWindowsList = new List<UIWindow>();
            }

            public void Add(UIWindow window)
            {
                ModalessWindowsList.Add(window);
            }

            public UIWindow Remove(UIWindowType type)
            {
                foreach (UIWindow window in ModalessWindowsList)
                {
                    if (window.WindowType == type)
                    {
                        UIWindow result = window;
                        ModalessWindowsList.Remove(window);
                        return result;
                    }
                }

                return null;
            }

            public bool IsModalessContainer()
            {
                return true;
            }
        }

        public interface IUIWindowContainer
        {
            void Add(UIWindow window);
            IUIWindows AtLast();
            UIWindow RemoveAtLast(UIWindowType type);
        }

        public class UIWindowContainerWithStack : IUIWindowContainer
        {
            public int Count
            {
                get { return _windowsList.Count; }
            }
            
            private Stack<IUIWindows> _windowsList;

            public UIWindowContainerWithStack()
            {
                _windowsList = new Stack<IUIWindows>();
            }

            public void Add(UIWindow window)
            {
                if (window.IsModal == true)
                {
                    _windowsList.Push(window);
                }
                else
                {
                    if (_windowsList.Count == 0)
                    {
                        ModalessUIWindowContainer container = new ModalessUIWindowContainer();
                        container.Add(window);
                        _windowsList.Push(container);
                    }
                    else
                    {
                        IUIWindows atLast = _windowsList.Peek();
                        if (atLast.IsModalessContainer() == true)
                        {
                            ModalessUIWindowContainer container = atLast as ModalessUIWindowContainer;
                            container.Add(window);
                        }
                        else
                        {
                            ModalessUIWindowContainer container = new ModalessUIWindowContainer();
                            container.Add(window);
                            _windowsList.Push(container);
                        }
                    }
                }
            }

            public IUIWindows AtLast()
            {
                if (_windowsList.Count != 0)
                {
                    return _windowsList.Peek();
                }

                return null;
            }

            public UIWindow RemoveAtLast(UIWindowType type)
            {
                if (_windowsList.Count != 0)
                {
                    IUIWindows windowsAtLast = _windowsList.Peek();
                    if (windowsAtLast.IsModalessContainer() == true)
                    {
                        ModalessUIWindowContainer container = windowsAtLast as ModalessUIWindowContainer;
                        return container.Remove(type);
                    }
                    else
                    {
                        UIWindow window = windowsAtLast as UIWindow;
                        if (window.WindowType == type)
                        {
                            UIWindow result =  _windowsList.Pop() as UIWindow;
                            return result;
                        }
                    }
                }

                return null;
            }
        }

        public interface IUIDataInfoContainer
        {
            void Add(UIWindowType type, UIData data);
            bool Contains(UIWindowType type);
            bool Refresh(UIWindowType type, UIData data);
            UIData GetUIData(UIWindowType type);
            void Remove(UIWindowType type);
        }

        public class UIDataInfoContainer : IUIDataInfoContainer
        {
            private Dictionary<UIWindowType, UIData> _uiDataMap;
            private Dictionary<UIWindowType, bool> _uiDataModifiedMap;

            public UIDataInfoContainer()
            {
                _uiDataMap = new Dictionary<UIWindowType, UIData>();
                _uiDataModifiedMap = new Dictionary<UIWindowType, bool>();
            }

            public void Add(UIWindowType type, UIData data)
            {
                if (_uiDataMap.ContainsKey(type) == false)
                {
                    _uiDataMap.Add(type, data);
                    _uiDataModifiedMap.Add(type, false);
                }
            }

            public bool Contains(UIWindowType type)
            {
                if (_uiDataMap.ContainsKey(type) == true)
                {
                    return true;
                }

                return false;
            }

            public bool Refresh(UIWindowType type, UIData data)
            {
                if (_uiDataMap.ContainsKey(type) == true)
                {
                    _uiDataMap[type] = data;
                    _uiDataModifiedMap[type] = true;
                    return true;
                }

                return false;
            }

            public UIData GetUIData(UIWindowType type)
            {
                if (_uiDataModifiedMap.ContainsKey(type) == true)
                {
                    if (_uiDataModifiedMap[type] == true)
                    {
                        _uiDataModifiedMap[type] = false;
                        return _uiDataMap[type];
                    }
                }
                
                return null;
            }

            public void Remove(UIWindowType type)
            {
                _uiDataMap.Remove(type);
                _uiDataModifiedMap.Remove(type);
            }
        }
    }
}
