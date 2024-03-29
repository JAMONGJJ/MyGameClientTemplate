using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.ResourceInfo;
using ClientTemplate.SceneRegion.SceneInfo;
using ClientTemplate.UIRegion.UIInfo;
using UnityEngine;

namespace ClientTemplate
{
    [Serializable]
    public class UIWindowAddress
    {
        public UIWindowAssetType assetType;
        public string address;
    }

    [Serializable]
    public class PrefabAddress
    {
        public PrefabAssetType assetType;
        public string address;
    }

    [Serializable]
    public class SceneAddress
    {
        public SceneAssetType assetType;
        public string address;
    }

    [Serializable]
    public class ImageAddress
    {
        public ImageAssetType assetType;
        public string address;
    }
    
    [CreateAssetMenu(fileName = "AssetAddressMaps", menuName = "ScriptableObject/AssetAddressMaps")]
    public class AssetAddressMaps : ScriptableObject
    {
        public List<UIWindowAddress> UIWindowAddressList;
        public List<PrefabAddress> PrefabAddressList;
        public List<SceneAddress> SceneAddressList;
        public List<ImageAddress> ImageAddressList;
    }
}