using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UniRx;

namespace ClientTemplate
{
    namespace UIInfo
    {
        public enum UIWindowType
        {
            None,
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
            MainHud,
            NoticeWindow,
            
        }

        [XmlRoot("UIWindow")]
        public class UIWindowAssetAddress
        {
            [XmlElement("Type")]
            public UIWindowAssetType type;
            
            [XmlElement("Address")]
            public string address;
        }

        [XmlRoot("AssetAddressMap")]
        public class UIWindowAssetAddressContainer
        {
            [XmlArray("Assets"), XmlArrayItem("UIWindow")]
            public List<UIWindowAssetAddress> AddressList;
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
            protected Subject<UIData> WindowOnTopSubject { get; set; }

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

            public UIWindow Find(UIWindowType type)
            {
                foreach (UIWindow window in ModalessWindowsList)
                {
                    if (window.WindowType == type)
                    {
                        return window;
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
    }
}
