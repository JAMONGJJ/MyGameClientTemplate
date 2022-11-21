using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.ResourceInfo;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
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

    [CreateAssetMenu(fileName = "AssetAddressMaps", menuName = "ScriptableObject/AssetAddressMaps")]
    public class AssetAddressMaps : ScriptableObject
    {
        public List<UIWindowAddress> UIWindowAddressList;
        public List<PrefabAddress> PrefabAddressList;
        public List<SceneAddress> SceneAddressList;
    }
    
    [CreateAssetMenu(fileName = "DataTables", menuName = "ScriptableObject/DataTables")]
    public class DataTables : ScriptableObject
    {
        
    }
}
