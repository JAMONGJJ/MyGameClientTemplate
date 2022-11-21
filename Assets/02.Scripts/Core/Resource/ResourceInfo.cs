using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using UnityEngine;

namespace ClientTemplate
{
    namespace ResourceInfo
    {
        public enum PrefabAssetType
        {
            None,
            TestCube,
            ThreeDCameraObject,
            
            
        }

        public interface IAssetAddressContainer
        {
            void SetAddressMaps(AssetAddressMaps maps);
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

            public void SetAddressMaps(AssetAddressMaps maps)
            {
                foreach (var address in maps.UIWindowAddressList)
                {
                    if (WindowAssetAddressMap.ContainsKey(address.assetType) == false)
                    {
                        WindowAssetAddressMap.Add(address.assetType, address.address);
                    }
                }
                
                foreach (var address in maps.SceneAddressList)
                {
                    if (SceneAssetAddressMap.ContainsKey(address.assetType) == false)
                    {
                        SceneAssetAddressMap.Add(address.assetType, address.address);
                    }
                }
                
                foreach (var address in maps.PrefabAddressList)
                {
                    if (PrefabAssetAddressMap.ContainsKey(address.assetType) == false)
                    {
                        PrefabAssetAddressMap.Add(address.assetType, address.address);
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
