using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ClientTemplate.StringInfo;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Task = System.Threading.Tasks.Task;

namespace ClientTemplate
{
    using ResourceInfo;

    public delegate void AssetSizeCheckCallback(long amount, System.Action callback);
    public delegate void AssetLoadFinishCallback();
    
    public interface IResourceManager : IManager
    {
        void CheckAssetToDownload(AssetSizeCheckCallback afterSizeCheckCallback, AssetLoadFinishCallback assetDownloadFinishFinishCallback);
        string GetAddressByType(UIWindowAssetType type);
        string GetAddressByType(SceneAssetType type);
        string GetAddressByType(PrefabAssetType type);
        AsyncOperationHandle<GameObject> LoadGameObject(string key);
        AsyncOperationHandle<SceneInstance> LoadScene(string key);
        AsyncOperationHandle<object> LoadObject(string key);
    }

    public class ResourceManager : IResourceManager
    {
        private IList<string> _labelNames;
        private IAssetAddressContainer _assetAddressContainer;
        private AssetSizeCheckCallback _sizeCheckCallback;
        private AssetLoadFinishCallback _assetLoadFinishFinishCallback;
        
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Resource Manager");
            _labelNames = new List<string>() { "Preload", "Dependencies" };
            _assetAddressContainer = new AssetAddressContainer();
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Resource Manager");
            _labelNames = null;
            _assetAddressContainer = null;
            _sizeCheckCallback = null;
            _assetLoadFinishFinishCallback = null;
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Resource Manager");
            Release();
            Init();
        }

        public async void CheckAssetToDownload(AssetSizeCheckCallback afterSizeCheckCallback, AssetLoadFinishCallback assetDownloadFinishFinishCallback)
        {
            _sizeCheckCallback = afterSizeCheckCallback;
            _assetLoadFinishFinishCallback = assetDownloadFinishFinishCallback;
            LogManager.Log(LogManager.LogType.DEFAULT, "Checking assets to download!");
            var handle = Addressables.GetDownloadSizeAsync(_labelNames);
            while (handle.IsDone == false)
            {
                await Task.Delay(10);
            }
            long downloadSize = handle.Result;
            if (downloadSize > 0)
            {
                LogManager.Log(LogManager.LogType.DEFAULT, $"Need to download {downloadSize}bytes of assets!");
                if (_sizeCheckCallback != null)
                {
                    _sizeCheckCallback.Invoke(downloadSize, LoadAddressablesAssets);
                    _sizeCheckCallback = null;
                }
            }
            else
            {
                LogManager.Log(LogManager.LogType.DEFAULT, "No need to download assets!");
                LoadAddressablesAssets();
            }

            Addressables.Release(handle);
        }

        public string GetAddressByType(UIWindowAssetType type)
        {
            return _assetAddressContainer.GetAddress(type);
        }
        
        public string GetAddressByType(SceneAssetType type)
        {
            return _assetAddressContainer.GetAddress(type);
        }

        public string GetAddressByType(PrefabAssetType type)
        {
            return _assetAddressContainer.GetAddress(type);
        }

        public AsyncOperationHandle<GameObject> LoadGameObject(string key)
        {
            return Addressables.LoadAssetAsync<GameObject>(key);
        }

        public AsyncOperationHandle<SceneInstance> LoadScene(string key)
        {
            return Addressables.LoadSceneAsync(key);
        }

        public AsyncOperationHandle<object> LoadObject(string key)
        {
            return Addressables.LoadAssetAsync<object>(key);
        }
        
        #region Load Addressables Assets
        private List<bool> _loadStateList;

        private bool CheckLoadStateList()
        {
            foreach (bool state in _loadStateList)
            {
                if (state == false)
                {
                    return false;
                }
            }

            return true;
        }
        
        private void LoadAddressablesAssets()
        {
            _loadStateList = new List<bool>();
            LoadAddressMaps();
            LoadDataTables();
            DownloadAssetBundles();
            
            CheckAllLoadIsCompleted();
        }

        private void LoadAddressMaps()
        {
            LoadUIWindowAddressMap();
            LoadSceneAddressMap();
            LoadPrefabAddressMap();
        }

        private void LoadDataTables()
        {
            LoadStringsDataTable();
        }

        private async void DownloadAssetBundles()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(_labelNames, Addressables.MergeMode.Union);
            while (downloadHandle.IsDone == false)
            {
                GameEntryManager.Instance.DataLoadWindow.SetSliderValue(downloadHandle.PercentComplete);
                await Task.Delay(10);
            }
            LogManager.Log(LogManager.LogType.DEFAULT, "Asset Bundles download completed!");
            _loadStateList[index] = true;
        }

        private async void LoadUIWindowAddressMap()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            var handle = Addressables.LoadAssetAsync<TextAsset>("UIWindowAddressMap");
            while (handle.IsDone == false)
            {
                await Task.Delay(10);
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(handle.Result.text);
            XmlSerializer serializer = new XmlSerializer(typeof(UIWindowAssetAddressContainer));
            using (StringReader reader = new StringReader(handle.Result.text))
            {
                try
                {
                    UIWindowAssetAddressContainer uiWindowAssetAddressContainer =
                        serializer.Deserialize(reader) as UIWindowAssetAddressContainer;
                    _assetAddressContainer.Add(uiWindowAssetAddressContainer);
                }
                catch (Exception e)
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                }
            }

            LogManager.Log(LogManager.LogType.DEFAULT, "UIWindowAddressMap download completed!");
            Addressables.Release(handle);
            _loadStateList[index] = true;
        }

        private async void LoadSceneAddressMap()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            var handle = Addressables.LoadAssetAsync<TextAsset>("SceneAddressMap");
            while (handle.IsDone == false)
            {
                await Task.Delay(10);
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(handle.Result.text);
            XmlSerializer serializer = new XmlSerializer(typeof(SceneAssetAddressContainer));
            using (StringReader reader = new StringReader(handle.Result.text))
            {
                try
                {
                    SceneAssetAddressContainer sceneAssetAddressContainer =
                        serializer.Deserialize(reader) as SceneAssetAddressContainer;
                    _assetAddressContainer.Add(sceneAssetAddressContainer);
                }
                catch (Exception e)
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                }
            }

            LogManager.Log(LogManager.LogType.DEFAULT, "SceneAddressMap download completed!");
            Addressables.Release(handle);
            _loadStateList[index] = true;
        }

        private async void LoadPrefabAddressMap()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            var handle = Addressables.LoadAssetAsync<TextAsset>("PrefabAddressMap");
            while (handle.IsDone == false)
            {
                await Task.Delay(10);
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(handle.Result.text);
            XmlSerializer serializer = new XmlSerializer(typeof(PrefabAssetAddressContainer));
            using (StringReader reader = new StringReader(handle.Result.text))
            {
                try
                {
                    PrefabAssetAddressContainer prefabAssetAddressContainer =
                        serializer.Deserialize(reader) as PrefabAssetAddressContainer;
                    _assetAddressContainer.Add(prefabAssetAddressContainer);
                }
                catch (Exception e)
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                }
            }

            LogManager.Log(LogManager.LogType.DEFAULT, "PrefabAddressMap download completed!");
            Addressables.Release(handle);
            _loadStateList[index] = true;
        }

        private async void LoadStringsDataTable()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            var handle = Addressables.LoadAssetAsync<TextAsset>("CommonStringsDataTable");
            while (handle.IsDone == false)
            {
                await Task.Delay(10);
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(handle.Result.text);
            XmlSerializer stringsSerializer = new XmlSerializer(typeof(CommonStrings));
            using (StringReader reader = new StringReader(handle.Result.text))
            {
                try
                {
                    CommonStrings commonStrings = stringsSerializer.Deserialize(reader) as CommonStrings;
                    CommonStringsInfoContainer commonStringsInfoContainer = new CommonStringsInfoContainer(commonStrings);
                    Data.Table.SetStringsInfoContainer(commonStringsInfoContainer);
                }
                catch (Exception e)
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                }
            }

            LogManager.Log(LogManager.LogType.DEFAULT, "CommonStringsDataTable download completed!");
            Addressables.Release(handle);
            _loadStateList[index] = true;
        }

        private async void CheckAllLoadIsCompleted()
        {
            while (CheckLoadStateList() == false)
            {
                await Task.Delay(10);
            }

            LogManager.Log(LogManager.LogType.DEFAULT, "All data download completed!");
            if (_assetLoadFinishFinishCallback != null)
            {
                _assetLoadFinishFinishCallback.Invoke();
                _assetLoadFinishFinishCallback = null;
            }
        }
        #endregion
    }
}
