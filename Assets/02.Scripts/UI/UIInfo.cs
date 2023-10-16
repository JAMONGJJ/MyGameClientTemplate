using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace UIInfo
    {
        public enum LayoutGroupType
        {
            None,
            Horizontal,
            Vertical,
            Grid,
            
        }

        public enum RectTransformAlignment
        {
            UpperLeft,
            UpperCenter,
            UpperRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            LowerLeft,
            LowerCenter,
            LowerRight,
            Custom,
            
        }
        
        public enum UIWindowType
        {
            None = 0,
            TestModalessUIWindow = 1,
            TestModalessUIWindow2 = 2,
            MainHud = 3,
            NoticeWindow = 4,
            CharacterCustomizingWindow = 5,
            LobbyWindow = 6,
            RoomCustomizingWindow = 7,
            FourCutsWindow = 8,
            CharacterCustomizingPresetOptionWindow = 9,
            EditLobbyWindow = 10,
            MyStatusWindow = 11,
            IndividualRoomWindow = 12,
            AvatarCheckWindow = 13,
            RythmCatWindow = 14
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
            None = 0,
            UISystem = 1,
            TestModalessUIWindow = 2,
            TestModalessUIWindow2 = 3,
            MainHud = 4,
            NoticeWindow = 5,
            CharacterCustomizingWindow = 6,
            CharacterCustomizingContent = 7,
            LobbyWindow = 8,
            CharacterCustomizingCategoryButton = 9,
            RoomCustomizingWindow = 11,
            RoomCustomizingContent = 12,
            RoomCustomizingCategoryButton = 13,
            RoomCustomizingCategoryScrollRect = 14,
            CharacterCustomizingPresetContent = 15,
            FourCutsWindow = 16,
            CharacterCustomizingPresetOptionWindow = 17,
            EditLobbyWindow = 18,
            MyStatusWindow = 19,
            MyStatusContent = 20,
            MyStatusCategoryButton = 21,
            MyStatusCategoryScrollRect = 22,
            AssignFriendButton = 23,
            CharacterCustomizingSubCategoryButton = 25,
            IndividualRoomWindow = 26,
            AvatarCheckWindow = 27,
            RythmCatWindow = 28
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

            public UIWindow RemoveAll()
            {
                foreach (UIWindow window in ModalessWindowsList)
                {
                    UIWindow result = window;
                    ModalessWindowsList.Remove(window);
                    return result;
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
            public int Count { get; }
            void Add(UIWindow window);
            T GetWindow<T>() where T : UIWindow;
            IUIWindows AtLast();
            UIWindow RemoveAtLast(UIWindowType type);
            UIWindow RemoveAtLast();
        }

        public class UIWindowContainerWithStack : IUIWindowContainer
        {
            public int Count
            {
                get { return WindowsList.Count; }
            }
            
            private Stack<IUIWindows> WindowsList;

            public UIWindowContainerWithStack()
            {
                WindowsList = new Stack<IUIWindows>();
            }

            public void Add(UIWindow window)
            {
                if (window.IsModal == true)
                {
                    WindowsList.Push(window);
                }
                else
                {
                    if (WindowsList.Count == 0)
                    {
                        ModalessUIWindowContainer container = new ModalessUIWindowContainer();
                        container.Add(window);
                        WindowsList.Push(container);
                    }
                    else
                    {
                        IUIWindows atLast = WindowsList.Peek();
                        if (atLast.IsModalessContainer() == true)
                        {
                            ModalessUIWindowContainer container = atLast as ModalessUIWindowContainer;
                            container.Add(window);
                        }
                        else
                        {
                            ModalessUIWindowContainer container = new ModalessUIWindowContainer();
                            container.Add(window);
                            WindowsList.Push(container);
                        }
                    }
                }
            }

            public T GetWindow<T>() where T : UIWindow
            {
                foreach (IUIWindows window in WindowsList)
                {
                    T convertedWindow = window as T;
                    if (convertedWindow == null)
                    {
                        continue;
                    }

                    return convertedWindow;
                }

                return null;
            }

            public IUIWindows AtLast()
            {
                if (WindowsList.Count != 0)
                {
                    return WindowsList.Peek();
                }

                return null;
            }

            public UIWindow RemoveAtLast(UIWindowType type)
            {
                if (WindowsList.Count != 0)
                {
                    IUIWindows windowsAtLast = WindowsList.Peek();
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
                            UIWindow result =  WindowsList.Pop() as UIWindow;
                            return result;
                        }
                    }
                }

                return null;
            }

            public UIWindow RemoveAtLast()
            {
                if (WindowsList.Count != 0)
                {
                    IUIWindows windowsAtLast = WindowsList.Peek();
                    if (windowsAtLast.IsModalessContainer() == true)
                    {
                        ModalessUIWindowContainer container = windowsAtLast as ModalessUIWindowContainer;
                        return container.RemoveAll();
                    }
                    else
                    {
                        UIWindow window = windowsAtLast as UIWindow;
                        UIWindow result =  WindowsList.Pop() as UIWindow;
                        return result;
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
            UIData GetModifiedUIData(UIWindowType type);
            void Remove(UIWindowType type);
        }

        public class UIDataInfoContainer : IUIDataInfoContainer
        {
            private Dictionary<UIWindowType, UIData> UIDataMap;
            private Dictionary<UIWindowType, bool> UIDataModifiedMap;

            public UIDataInfoContainer()
            {
                UIDataMap = new Dictionary<UIWindowType, UIData>();
                UIDataModifiedMap = new Dictionary<UIWindowType, bool>();
            }

            public void Add(UIWindowType type, UIData data)
            {
                if (UIDataMap.ContainsKey(type) == false)
                {
                    UIDataMap.Add(type, data);
                    UIDataModifiedMap.Add(type, false);
                }
            }

            public bool Contains(UIWindowType type)
            {
                if (UIDataMap.ContainsKey(type) == true)
                {
                    return true;
                }

                return false;
            }

            public bool Refresh(UIWindowType type, UIData data)
            {
                if (UIDataMap.ContainsKey(type) == true)
                {
                    UIDataMap[type] = data;
                    UIDataModifiedMap[type] = true;
                    return true;
                }

                return false;
            }

            public UIData GetUIData(UIWindowType type)
            {
                UIDataMap.TryGetValue(type, out UIData result);
                return result;
            }

            public UIData GetModifiedUIData(UIWindowType type)
            {
                if (UIDataModifiedMap.ContainsKey(type) == true)
                {
                    if (UIDataModifiedMap[type] == true)
                    {
                        UIDataModifiedMap[type] = false;
                        return UIDataMap[type];
                    }
                }
                
                return null;
            }

            public void Remove(UIWindowType type)
            {
                UIDataMap.Remove(type);
                UIDataModifiedMap.Remove(type);
            }
        }
    }
}
