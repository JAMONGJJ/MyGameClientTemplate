using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ClientTemplate.StringInfo;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using ClientTemplate.UtilityFunctions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Task = System.Threading.Tasks.Task;

namespace ClientTemplate
{
    using ResourceInfo;

    public delegate void AssetLoadFinishCallback();
    
    public interface IResourceManager : IManager
    {
        void LoadVersionDataTable();
        void CheckDownloadAssets(AssetLoadFinishCallback assetDownloadFinishFinishCallback);
        string GetAddressByType(UIWindowAssetType type);
        string GetAddressByType(SceneAssetType type);
        string GetAddressByType(PrefabAssetType type);
        AsyncOperationHandle<GameObject> LoadGameObject(string key);
        AsyncOperationHandle<SceneInstance> LoadScene(string key);
        AsyncOperationHandle<object> LoadObject(string key);
    }

    public class ResourceManager : IResourceManager
    {
        private IList<string> LabelNames;
        private IAssetAddressContainer AssetAddressContainer;
        private AssetLoadFinishCallback AssetLoadFinishFinishCallback;
        private bool firstDownload;
        
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

        public void CheckDownloadAssets(AssetLoadFinishCallback assetDownloadFinishFinishCallback)
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Checking assets to download!");
            AssetLoadFinishFinishCallback = assetDownloadFinishFinishCallback;
            var handle = Addressables.GetDownloadSizeAsync(LabelNames);
            handle.Completed += _ =>
            {
                long downloadSize = handle.Result;
                if (downloadSize > 0)
                {
                    firstDownload = true;
                    GameEntryManager.Instance.GameEntryWindow.SetActiveAssetDownload(true);
                    GameEntryManager.Instance.GameEntryWindow.SetAssetDownloadText(downloadSize);
                    GameEntryManager.Instance.GameEntryWindow.AcceptAssetDownloadButton.onClick.AddListener(() =>
                        {
                            GameEntryManager.Instance.GameEntryWindow.SetActiveDownloadSlider(true);
                            GameEntryManager.Instance.GameEntryWindow.SetActiveAssetDownload(false);
                            LoadAddressablesAssets();
                        });
                }
                else
                {
                    firstDownload = false;
                    LoadAddressablesAssets();
                }

                Addressables.Release(handle);
            };
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
            LoadAssetAddressMaps();
            LoadDataTables();
            DownloadAssetBundles();
            
            CheckAllLoadIsCompleted();
        }

        private async void DownloadAssetBundles()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(LabelNames, Addressables.MergeMode.Union);
            while (downloadHandle.IsDone == false)
            {
                await Task.Delay(10);
                if (firstDownload == true)
                {
                    GameEntryManager.Instance.GameEntryWindow.SetLoadingSliderValue(downloadHandle.PercentComplete);
                }
            }
            LogManager.Log(LogManager.LogType.DEFAULT, "Asset Bundles download completed!");
            _loadStateList[index] = true;
        }

        private async void LoadAssetAddressMaps()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            var handle = Addressables.LoadAssetAsync<ScriptableObject>("AssetAddressMaps");
            while (handle.IsDone == false)
            {
                await Task.Delay(10);
            }

            AssetAddressMaps addressMaps = handle.Result as AssetAddressMaps;
            AssetAddressContainer.SetAddressMaps(addressMaps);

            LogManager.Log(LogManager.LogType.DEFAULT, "UIWindowAddressMap download completed!");
            Addressables.Release(handle);
            _loadStateList[index] = true;
        }

        private async void LoadDataTables()
        {
            _loadStateList.Add(false);
            int index = _loadStateList.Count - 1;
            var handle = Addressables.LoadAssetAsync<ScriptableObject>("DataTables");
            while (handle.IsDone == false)
            {
                await Task.Delay(10);
            }

            DataTables dataTables = handle.Result as DataTables;
            Data.Table.SetDataTables(dataTables);

            LogManager.Log(LogManager.LogType.DEFAULT, "UIWindowAddressMap download completed!");
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
            if (AssetLoadFinishFinishCallback != null)
            {
                AssetLoadFinishFinishCallback.Invoke();
                AssetLoadFinishFinishCallback = null;
            }
        }
        #endregion
    }
}
