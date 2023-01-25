using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using ClientTemplate.ResourceInfo;
using ClientTemplate.UtilityFunctions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ClientTemplate
{
    public interface IResourceManager : IManager
    {
        float DownloadProgress { get; }
        event EventHandler<float> OnDownloadProgressChange;
        void LoadVersionDataTable();
        void SetAssetBundleLoadFinishCallback(System.Action assetBundleLoadFinishCallback);
        long GetAssetBundleSize();
        void LoadAddressablesAssets();
        string GetAddressByType(UIWindowAssetType type);
        string GetAddressByType(SceneAssetType type);
        string GetAddressByType(PrefabAssetType type);
        AsyncOperationHandle<GameObject> LoadGameObject(string key);
        AsyncOperationHandle<SceneInstance> LoadScene(string key);
        AsyncOperationHandle<object> LoadObject(string key);
        GameObject LoadAssets(UIWindowAssetType type);
        bool LoadAssets(SceneAssetType type);
        GameObject LoadAssets(PrefabAssetType type);
    }

    public class ResourceManager : IResourceManager
    {
        private float downloadProgress;
        public float DownloadProgress
        {
            get { return downloadProgress; }
            private set
            {
                downloadProgress = value;
                if (OnDownloadProgressChange != null)
                {
                    OnDownloadProgressChange(this, downloadProgress);
                }
            }
        }
        
        public event EventHandler<float> OnDownloadProgressChange;
        
        private IList<string> LabelNames;
        private IAssetAddressContainer AssetAddressContainer;
        private System.Action AssetLoadFinishFinishCallback;
        
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Resource Manager");
            LabelNames = new List<string>() { "Preload", "Dependencies" };
            AssetAddressContainer = new AssetAddressContainer();
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Resource Manager");
            LabelNames = null;
            AssetAddressContainer = null;
            AssetLoadFinishFinishCallback = null;
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Resource Manager");
            Release();
            Init();
        }

        public void LoadVersionDataTable()
        {
            Utility.Functions.Async.SetIsProcessing(true);
            Utility.Functions.Exception.Process(() =>
            {
                var handle = Addressables.LoadAssetAsync<ScriptableObject>("VersionDataTable");
                handle.Completed += _ =>
                {
                    VersionsDataTable version = handle.Result as VersionsDataTable;
                    Data.Table.SetVersion(version);
        
                    Addressables.Release(handle);
                    Utility.Functions.Async.SetIsProcessing(false);
                    LogManager.Log(LogManager.LogType.DEFAULT, "VersionDataTable download completed!");
                };
            });
        }

        public void SetAssetBundleLoadFinishCallback(System.Action assetBundleLoadFinishCallback)
        {
            AssetLoadFinishFinishCallback = assetBundleLoadFinishCallback;
        }

        public long GetAssetBundleSize()
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Checking assets to download!");
            var handle = Addressables.GetDownloadSizeAsync(LabelNames);
            handle.WaitForCompletion();
            
            long downloadSize = handle.Result;
            Addressables.Release(handle);
            return downloadSize;
        }
        
        public void LoadAddressablesAssets()
        {
            LoadAssetAddressMaps();
            LoadDataTables();
            DownloadAssetBundles();
            
            LogManager.Log(LogManager.LogType.DEFAULT, "All data download completed!");
            if (AssetLoadFinishFinishCallback != null)
            {
                AssetLoadFinishFinishCallback.Invoke();
                AssetLoadFinishFinishCallback = null;
            }
        }

        public string GetAddressByType(UIWindowAssetType type)
        {
            return AssetAddressContainer.GetAddress(type);
        }
        
        public string GetAddressByType(SceneAssetType type)
        {
            return AssetAddressContainer.GetAddress(type);
        }

        public string GetAddressByType(PrefabAssetType type)
        {
            return AssetAddressContainer.GetAddress(type);
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

        public GameObject LoadAssets(UIWindowAssetType type)
        {
            string address = AssetAddressContainer.GetAddress(type);
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception("The asset address that matches entered type is null!");
            }
            
            return LoadAssets<GameObject>(address);
        }

        public bool LoadAssets(SceneAssetType type)
        {
            string address = AssetAddressContainer.GetAddress(type);
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception("The asset address that matches entered type is null!");
            }

            LoadAssets(address);
            return true;
        }

        public GameObject LoadAssets(PrefabAssetType type)
        {
            string address = AssetAddressContainer.GetAddress(type);
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception("The asset address that matches entered type is null!");
            }
            
            return LoadAssets<GameObject>(address);
        }
        
        #region Load Addressables Assets

        private void DownloadAssetBundles()
        {
            var handle = Addressables.DownloadDependenciesAsync(LabelNames, Addressables.MergeMode.Union);
            // AssetBundlePercentage(handle);
            handle.WaitForCompletion();
            LogManager.Log(LogManager.LogType.DEFAULT, "Asset Bundles download completed!");
        }

        private async void AssetBundlePercentage(AsyncOperationHandle handle)
        {
            while (handle.PercentComplete < 1.0f)
            {
                DownloadProgress = handle.PercentComplete;
                await Task.Delay(10);
            }
        }

        private void LoadAssetAddressMaps()
        {
            var handle = Addressables.LoadAssetAsync<ScriptableObject>("AssetAddressMaps");
            handle.WaitForCompletion();

            AssetAddressMaps addressMaps = handle.Result as AssetAddressMaps;
            AssetAddressContainer.SetAddressMaps(addressMaps);

            LogManager.Log(LogManager.LogType.DEFAULT, "AssetAddressMaps download completed!");
            Addressables.Release(handle);
        }

        private void LoadDataTables()
        {
            var handle = Addressables.LoadAssetAsync<ScriptableObject>("DataTables");
            handle.WaitForCompletion();

            DataTables dataTables = handle.Result as DataTables;
            Data.Table.SetDataTables(dataTables);

            LogManager.Log(LogManager.LogType.DEFAULT, "DataTables download completed!");
            Addressables.Release(handle);
        }

        private T LoadAssets<T>(string key)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.WaitForCompletion();
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("Asset loading process has failed!");
            }
            
            T result = handle.Result;
            Addressables.Release(handle);
            return result;
        }

        private void LoadAssets(string key)
        {
            var handle = Addressables.LoadSceneAsync(key);
            handle.WaitForCompletion();
            Addressables.Release(handle);
        }
        
        #endregion
    }
}
