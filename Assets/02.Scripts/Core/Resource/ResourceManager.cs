using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTemplate.ResourceInfo;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using PrefabAssetType = ClientTemplate.ResourceInfo.PrefabAssetType;

namespace ClientTemplate
{
    /// <summary>
    /// Dependency Injection을 위해 만들었던 interface 함수.
    /// </summary>
    public interface IResourceManager : IManager
    {
        void SetAssetBundleLoadFinishCallback(Action assetBundleLoadFinishCallback);
        long GetAssetBundleSize();
        float GetAssetBundleDownloadProgress();
        void LoadAddressablesAssets();
        GameObject LoadAssets(UIWindowAssetType type);
        SceneInstance LoadAssets(SceneAssetType type);
        GameObject LoadAssets(PrefabAssetType type);
        Texture2D LoadAssets(ImageAssetType type);
        void LoadAssets<T>(string address, out T result) where T : class;
    }

    public class ResourceManager : IResourceManager
    {
        private IAssetAddressContainer AssetAddressContainer;
        private Action AssetLoadFinishFinishCallback;
        private AssetLabelReference assetLabelReference;
        private float downloadProgress;
        
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Resource Manager");
            AssetAddressContainer = new AssetAddressContainer();
            assetLabelReference = new AssetLabelReference();
            assetLabelReference.labelString = "Dependencies";
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Resource Manager");
            AssetAddressContainer = null;
            AssetLoadFinishFinishCallback = null;
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Resource Manager");
            Release();
            Init();
        }

        /// <summary>
        /// 애셋 번들 다운로드가 다 끝난 후에 실행될 콜백 함수를 설정하는 함수.
        /// LoadAddressablesAssets() 함수가 호출되기 전에 호출되어야 정상적인 루틴으로 실행 가능.
        /// </summary>
        /// <param name="assetBundleLoadFinishCallback">애셋 번들 다운로드가 끝난 후에 실행될 콜백 함수</param>
        public void SetAssetBundleLoadFinishCallback(Action assetBundleLoadFinishCallback)
        {
            AssetLoadFinishFinishCallback = assetBundleLoadFinishCallback;
        }

        /// <summary>
        /// 다운로드 받아야 할 애셋 번들의 크기.
        /// </summary>
        /// <returns>byte단위의 애셋 번들 크기를 반환함. 새로 다운로드 받을 애셋 번들이 없다면 0 반환. 오류가 발생하면 -1 반환.</returns>
        public long GetAssetBundleSize()
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Checking assets to download!");
            var handle = Addressables.GetDownloadSizeAsync(assetLabelReference);
            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, handle.OperationException);
                return -1;
            }
            
            long downloadSize = handle.Result;
            LogManager.Log(LogManager.LogType.DEFAULT, $"Asset download size : {downloadSize}");
            Addressables.Release(handle);
            return downloadSize;
        }

        /// <summary>
        /// 현재 애셋 번들을 다운받고 있다면, 얼마나 다운 받았는지 반환하는 함수.
        /// </summary>
        /// <returns>애셋 번들을 다운받고 있는 중이라면, 0과 1사이의 값으로 진행도를 반환. 애셋 번들을 다운받고 있지 않다면 0 반환.</returns>
        public float GetAssetBundleDownloadProgress()
        {
            LogManager.Log(LogManager.LogType.DEFAULT, "Checking asset bundle download progress!");
            return downloadProgress;
        }
        
        /// <summary>
        /// 애셋 번들를 다운로드 하는 함수.
        /// AssetAddressMaps, DataTables, Asset Bundle들을 다운로드함.
        /// </summary>
        public void LoadAddressablesAssets()
        {
            try
            {
                LoadAssetAddressMaps();
                LoadDataTables();
                DownloadAssetBundles();
            }
            catch (Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                return;
            }
        }

        /// <summary>
        /// 입력받은 UIWindowAssetType에 맞는 GameObject을 로드한 후에 반환.
        /// </summary>
        /// <param name="type">로드하고 싶은 애셋의 타입</param>
        /// <returns>정상적으로 애셋이 로드됐다면 GameObject로 반환. 오류가 있다면 null 반환.</returns>
        /// <exception cref="Exception">입력받은 타입에 맞는 애셋의 어드레스가 없다면 exception.</exception>
        public GameObject LoadAssets(UIWindowAssetType type)
        {
            string address = AssetAddressContainer.GetAddress(type);
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception($"The asset address({address}) that matches entered type({type}) is null!");
            }
            
            return LoadAssets<GameObject>(address);
        }

        /// <summary>
        /// 입력받은 SceneAssetType에 맞는 SceneInstance을 로드한 후에 반환.
        /// 반환된 SceneInstance는 SceneManager에서 로드함.
        /// </summary>
        /// <param name="type">로드하고 싶은 애셋의 타입</param>
        /// <returns>정상적으로 애셋이 로드됐다면 SceneInstance 반환. 오류가 있다면 null 반환.</returns>
        /// <exception cref="Exception">입력받은 타입에 맞는 애셋의 어드레스가 없다면 exception.</exception>
        public SceneInstance LoadAssets(SceneAssetType type)
        {
            string address = AssetAddressContainer.GetAddress(type);
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception($"The asset address({address}) that matches entered type({type}) is null!");
            }

            var handle = Addressables.LoadSceneAsync(address);
            handle.WaitForCompletion();
            SceneInstance result = handle.Result;
            return result;
        }

        /// <summary>
        /// 입력받은 PrefabAssetType에 맞는 GameObject을 로드한 후에 반환.
        /// </summary>
        /// <param name="type">로드하고 싶은 애셋의 타입</param>
        /// <returns>정상적으로 애셋이 로드됐다면 GameObject로 반환. 오류가 있다면 null 반환.</returns>
        /// <exception cref="Exception">입력받은 타입에 맞는 애셋의 어드레스가 없다면 exception.</exception>
        public GameObject LoadAssets(ClientTemplate.ResourceInfo.PrefabAssetType type)
        {
            string address = AssetAddressContainer.GetAddress(type);
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception($"The asset address({address}) that matches entered type({type}) is null!");
            }
            
            return LoadAssets<GameObject>(address);
        }

        /// <summary>
        /// 입력받은 ImageAssetType에 맞는 Texture2D을 로드한 후에 반환.
        /// </summary>
        /// <param name="type">로드하고 싶은 애셋의 타입</param>
        /// <returns>정상적으로 애셋이 로드됐다면 Texture2D로 반환. 오류가 있다면 null 반환.</returns>
        /// <exception cref="Exception">입력받은 타입에 맞는 애셋의 어드레스가 없다면 exception.</exception>
        public Texture2D LoadAssets(ImageAssetType type)
        {
            if (type == ImageAssetType.None)
            {
                return null;
            }

            string address = AssetAddressContainer.GetAddress(type);
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception($"The asset address({address}) that matches entered type({type}) is null!");
            }
            
            return LoadAssets<Texture2D>(address);
        }
        
        /// <summary>
        /// 입력받은 어드레스에 맞는 애셋을 로드한 후에 result로 반환.
        /// </summary>
        /// <param name="address">로드하고 싶은 애셋의 어드레스</param>
        /// <param name="result">정상적으로 애셋이 로드됐다면 입력 받는 타입 T로 반환. 오류가 있다면 null 반환.</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="Exception">입력받은 타입에 맞는 애셋의 어드레스가 없다면 exception.</exception>
        public void LoadAssets<T>(string address, out T result) where T : class
        {
            if (string.IsNullOrEmpty(address) == true)
            {
                throw new Exception($"The asset address({address}) that matches entered type is null!");
            }

            result = LoadAssets<T>(address);
        }

        /// <summary>
        /// 입력 받은 어드레스에 해당하는 애셋이 애셋 번들에 포함되어 있는지 확인하는 함수.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool AddressableResourceExists<T>(string key) where T : class
        {
            foreach (var locator in Addressables.ResourceLocators)
            {
                if (locator.Locate(key, typeof(T), out IList<IResourceLocation> locs))
                {
                    return true;
                }
            }

            return false;
        }
        
        #region Load Addressables Assets

        private async void DownloadAssetBundles()
        {
            downloadProgress = -1f;
            var handle = Addressables.DownloadDependenciesAsync(assetLabelReference);
            handle.Completed += (opHandle) =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw handle.OperationException;
                }
                
                LogManager.Log(LogManager.LogType.DEFAULT, "Asset Bundles download completed!");
            };

            while (handle.IsDone == false)
            {
                downloadProgress = handle.GetDownloadStatus().Percent;
                LogManager.Log(LogManager.LogType.DEFAULT, $"downloadProgress : {downloadProgress}");
                await Task.Delay(1);
            }
            
            LogManager.Log(LogManager.LogType.DEFAULT, "All data download completed!");
            if (AssetLoadFinishFinishCallback != null)
            {
                AssetLoadFinishFinishCallback.Invoke();
                AssetLoadFinishFinishCallback = null;
            }
        }

        private void LoadAssetAddressMaps()
        {
            var handle = Addressables.LoadAssetAsync<ScriptableObject>("AssetAddressMaps");
            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw handle.OperationException;
            }

            AssetAddressMaps addressMaps = handle.Result as AssetAddressMaps;
            AssetAddressContainer.SetAddressMaps(addressMaps);

            LogManager.Log(LogManager.LogType.DEFAULT, "AssetAddressMaps download completed!");
        }

        private void LoadDataTables()
        {
            var handle = Addressables.LoadAssetAsync<ScriptableObject>("DataTables");
            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw handle.OperationException;
            }

            DataTables dataTables = handle.Result as DataTables;
            Info.Table.SetInfoTable(dataTables);

            LogManager.Log(LogManager.LogType.DEFAULT, "DataTables download completed!");
        }

        private T LoadAssets<T>(string key) where T : class
        {
            if (AddressableResourceExists<T>(key) == false)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"There is no corresponding key in Addressables asset! key : {key}");
                return null;
            }
            
            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.WaitForCompletion();
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("Asset loading process has failed!");
            }
            
            T result = handle.Result;
            return result;
        }
        
        #endregion
    }
}
