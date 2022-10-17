using System.Collections;
using System.Collections.Generic;
using ClientTemplate.ResourceInfo;
using System.Xml.Serialization;
using ClientTemplate.UIInfo;
using UnityEngine;

namespace ClientTemplate
{
    namespace SceneInfo
    {
        public enum SceneType
        {
            None,
            EntryScene,
            TestScene,
            
        }

        public enum SceneAssetType
        {
            None,
            EntryScene,
            TestScene,
            
        }

        [XmlRoot("Scene")]
        public class SceneAssetAddress
        {
            [XmlElement("Type")]
            public SceneAssetType type;
            
            [XmlElement("Address")]
            public string address;
        }

        [XmlRoot("AssetAddressMap")]
        public class SceneAssetAddressContainer
        {
            [XmlArray("Assets"), XmlArrayItem("Scene")]
            public List<SceneAssetAddress> AddressList;
        }

        public class Scene
        {
            public string sceneName { get; private set; }
            public SceneAssetType assetType { get; private set; }

            public Scene(string name, SceneAssetType assetType)
            {
                sceneName = name;
                this.assetType = assetType;
            }
        }

        public interface ISceneContainer
        {
            void Add(SceneType sceneType, string name, SceneAssetType assetType);
            Scene GetValue(SceneType sceneType);
        }

        public class SceneContainer : ISceneContainer
        {
            private Dictionary<SceneType, Scene> SceneMap { get; set; }

            public SceneContainer()
            {
                SceneMap = new Dictionary<SceneType, Scene>();
            }

            public void Add(SceneType sceneType, string name, SceneAssetType assetType)
            {
                if (SceneMap.ContainsKey(sceneType) == false)
                {
                    Scene newScene = new Scene(name, assetType);
                    SceneMap.Add(sceneType, newScene);
                }
            }

            public Scene GetValue(SceneType sceneType)
            {
                if (SceneMap.ContainsKey(sceneType) == true)
                {
                    return SceneMap[sceneType];
                }

                return null;
            }
        }
    }
}
