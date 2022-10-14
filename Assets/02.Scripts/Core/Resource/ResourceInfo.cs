using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace KlayLand
{
    namespace ResourceInfo
    {
        public enum UIWindowAssetType
        {
            None,
            MainHud,
            CustomizeWindow,
            MyPet,
            NoticeWindow,
            MyProfile,
            
        }

        public enum SceneAssetType
        {
            None,
            EntryScene,
            TestScene,
            StayUpScene,
            
        }

        public enum PrefabAssetType
        {
            None,
            CameraObject,
            TestCharacterRig,
            
            
        }

        [XmlRoot("Asset")]
        public class UIWindowAssetAddress
        {
            [XmlElement("Type")]
            public UIWindowAssetType type;
            
            [XmlElement("Address")]
            public string address;
        }

        [XmlRoot("Assets")]
        public class UIWindowAssetAddressContainer
        {
            [XmlArray("Group"), XmlArrayItem("Asset")]
            public List<UIWindowAssetAddress> AddressList;
        }

        [XmlRoot("Asset")]
        public class SceneAssetAddress
        {
            [XmlElement("Type")]
            public SceneAssetType type;
            
            [XmlElement("Address")]
            public string address;
        }

        [XmlRoot("Assets")]
        public class SceneAssetAddressContainer
        {
            [XmlArray("Group"), XmlArrayItem("Asset")]
            public List<SceneAssetAddress> AddressList;
        }

        [XmlRoot("Asset")]
        public class PrefabAssetAddress
        {
            [XmlElement("Type")]
            public PrefabAssetType type;
            
            [XmlElement("Address")]
            public string address;
        }

        [XmlRoot("Assets")]
        public class PrefabAssetAddressContainer
        {
            [XmlArray("Group"), XmlArrayItem("Asset")]
            public List<PrefabAssetAddress> AddressList;
        }

        public interface IAssetAddressContainer
        {
            void Add(UIWindowAssetType type, string address);
            void AddRange(UIWindowAssetAddressContainer container);
            void Add(SceneAssetType type, string address);
            void AddRange(SceneAssetAddressContainer container);
            void Add(PrefabAssetType type, string address);
            void AddRange(PrefabAssetAddressContainer container);
            string GetAddress(UIWindowAssetType type);
            string GetAddress(SceneAssetType type);
            string GetAddress(PrefabAssetType type);
        }

        public class AssetAddressContainer : IAssetAddressContainer
        {
            private Dictionary<UIWindowAssetType, string> WindowAssetAddressMap { get; set; }
            private Dictionary<SceneAssetType, string> SceneAssetAddressMap { get; set; }
            private Dictionary<PrefabAssetType, string> PrefabAssetAddressMap { get; set; }

            public AssetAddressContainer()
            {
                WindowAssetAddressMap = new Dictionary<UIWindowAssetType, string>();
                SceneAssetAddressMap = new Dictionary<SceneAssetType, string>();
                PrefabAssetAddressMap = new Dictionary<PrefabAssetType, string>();
            }

            public void Add(UIWindowAssetType type, string address)
            {
                if (WindowAssetAddressMap.ContainsKey(type) == false)
                {
                    WindowAssetAddressMap.Add(type, address);
                }
            }

            public void AddRange(UIWindowAssetAddressContainer container)
            {
                foreach (UIWindowAssetAddress add in container.AddressList)
                {
                    if (WindowAssetAddressMap.ContainsKey(add.type) == false)
                    {
                        WindowAssetAddressMap.Add(add.type, add.address);
                    }
                }
            }

            public void Add(SceneAssetType type, string address)
            {
                if (SceneAssetAddressMap.ContainsKey(type) == false)
                {
                    SceneAssetAddressMap.Add(type, address);
                }
            }

            public void AddRange(SceneAssetAddressContainer container)
            {
                foreach (SceneAssetAddress add in container.AddressList)
                {
                    if (SceneAssetAddressMap.ContainsKey(add.type) == false)
                    {
                        SceneAssetAddressMap.Add(add.type, add.address);
                    }
                }
            }

            public void Add(PrefabAssetType type, string address)
            {
                if (PrefabAssetAddressMap.ContainsKey(type) == false)
                {
                    PrefabAssetAddressMap.Add(type, address);
                }
            }

            public void AddRange(PrefabAssetAddressContainer container)
            {
                foreach (PrefabAssetAddress add in container.AddressList)
                {
                    if (PrefabAssetAddressMap.ContainsKey(add.type) == false)
                    {
                        PrefabAssetAddressMap.Add(add.type, add.address);
                    }
                }
            }

            public string GetAddress(UIWindowAssetType type)
            {
                if (WindowAssetAddressMap.ContainsKey(type) == true)
                {
                    return WindowAssetAddressMap[type];
                }

                return string.Empty;
            }

            public string GetAddress(SceneAssetType type)
            {
                if (SceneAssetAddressMap.ContainsKey(type) == true)
                {
                    return SceneAssetAddressMap[type];
                }

                return string.Empty;
            }

            public string GetAddress(PrefabAssetType type)
            {
                if (PrefabAssetAddressMap.ContainsKey(type) == true)
                {
                    return PrefabAssetAddressMap[type];
                }

                return string.Empty;
            }
        }
    }
}
