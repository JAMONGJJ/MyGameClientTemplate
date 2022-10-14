using System;
using System.Collections;
using System.Collections.Generic;
using KlayLand.ResourceInfo;
using UnityEngine;
using UniRx;

namespace KlayLand
{
    namespace UIInfo
    {
        public interface IDataContainer
        {
        }

        public enum UIWindowType
        {
            None,
            MainHud,
            CustomizeWindow,
            MyPet,
            NoticeWindow,
            MyProfile,
            

        }

        public enum UICanvasType
        {
            None,
            Normal,
            Alert,
            Loading,
            
        }

        public interface IUIWindowAssetTypeContainer : IDataContainer
        {
            void Add(UIWindowType windowType, UICanvasType canvasType, UIWindowAssetType assetType);
            UICanvasType GetCanvasType(UIWindowType type);
            UIWindowAssetType GetAssetType(UIWindowType type);
        }

        public class UIWindowAssetTypeContainer : IUIWindowAssetTypeContainer
        {
            private Dictionary<UIWindowType, UICanvasType> CanvasTypeMap { get; set; }
            private Dictionary<UIWindowType, UIWindowAssetType> AssetTypeMap { get; set; }

            public UIWindowAssetTypeContainer()
            {
                AssetTypeMap = new Dictionary<UIWindowType, UIWindowAssetType>();
                CanvasTypeMap = new Dictionary<UIWindowType, UICanvasType>();
            }

            public void Add(UIWindowType windowType, UICanvasType canvasType, UIWindowAssetType assetType)
            {
                if (AssetTypeMap.ContainsKey(windowType) == false && CanvasTypeMap.ContainsKey(windowType) == false)
                {
                    CanvasTypeMap.Add(windowType, canvasType);
                    AssetTypeMap.Add(windowType, assetType);
                }
            }

            public UICanvasType GetCanvasType(UIWindowType type)
            {
                if (AssetTypeMap.ContainsKey(type) == true)
                {
                    return CanvasTypeMap[type];
                }
                
                return UICanvasType.None;
            }

            public UIWindowAssetType GetAssetType(UIWindowType type)
            {
                if (AssetTypeMap.ContainsKey(type) == true)
                {
                    return AssetTypeMap[type];
                }
                
                return UIWindowAssetType.None;
            }
        }

        public class UIData
        {

        }

        public class UIWindow : MonoBehaviour
        {
            public UIWindowType WindowType { get; protected set; }

            public void SetWindowType(UIWindowType type)
            {
                WindowType = type;
            }

            public virtual void Init()
            {

            }

            public virtual void Init(UIData data)
            {

            }

            public virtual void Release()
            {

            }

            public virtual void OnTop()
            {

            }
        }

        public interface IUIWindowContainer : IDataContainer
        {
            void Add(UIWindow window);
            UIWindow At(int index);
            UIWindow AtLast();
            UIWindow RemoveAtLast();
            UIWindow Remove<T>();
            UIWindow Find<T>();
        }

        public class UIWindowContainer : IUIWindowContainer
        {
            public int Count
            {
                get { return WindowList.Count; }
            }

            private List<UIWindow> WindowList { get; set; }

            public UIWindowContainer()
            {
                WindowList = new List<UIWindow>();
            }

            public void Add(UIWindow window)
            {
                WindowList.Add(window);
            }

            public UIWindow At(int index)
            {
                if (index >= 0 && Count > index)
                {
                    return WindowList[index];
                }

                return null;
            }

            public UIWindow AtLast()
            {
                if (Count > 0)
                {
                    return WindowList[Count - 1];
                }

                return null;
            }

            public UIWindow RemoveAtLast()
            {
                if (Count > 0)
                {
                    UIWindow window = WindowList[Count - 1];
                    WindowList.Remove(window);
                    return window;
                }

                return null;
            }

            public UIWindow Remove<T>()
            {
                foreach (var window in WindowList)
                {
                    if (window.GetComponent<T>() != null)
                    {
                        WindowList.Remove(window);
                        return window;
                    }
                }

                return null;
            }

            public UIWindow Find<T>()
            {
                foreach (var window in WindowList)
                {
                    if (window.GetComponent<T>() != null)
                    {
                        return window;
                    }
                }

                return null;
            }
        }
    }
}
