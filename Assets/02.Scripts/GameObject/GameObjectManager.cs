using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.GameObjectManagerInfo;
using ClientTemplate.ResourceInfo;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ClientTemplate
{
    namespace GameObjectManagerInfo
    {
        public class LoadTextureInfo
        {
            public string url;
            public UnityAction<Texture2D> LoadFinishAction;
        }
    }

    public class GameObjectManager : MonoManager<GameObjectManager>
    {
        private Dictionary<string, Texture2D> TextureMap;
        private Dictionary<string, UnityAction<Texture2D>> LoadFinishActionMap;
        private Queue<LoadTextureInfo> WaitingLoadQueue;
        private Coroutine TextureLoadCoroutine;
        private int TextureMapMaxCount;
        
        public void Init()
        {
            TextureMapMaxCount = 50;
            TextureMap = new Dictionary<string, Texture2D>();
            LoadFinishActionMap = new Dictionary<string, UnityAction<Texture2D>>();
            WaitingLoadQueue = new Queue<LoadTextureInfo>();
        }
        
        public void Release()
        {
            WaitingLoadQueue = null;
            LoadFinishActionMap = null;
            TextureMap = null;
        }

        /// <summary>
        /// 입력받은 타입에 맞는 애셋을 Instantiate하는 함수.
        /// </summary>
        /// <param name="assetType">생성하고자 하는 애셋 타입</param>
        /// <param name="pos">생성한 후의 초기 위치</param>
        /// <param name="rot">생성한 후의 초기 회전값</param>
        /// <param name="parentTransform">부모 오브젝트의 transform 값, 기본값은 null</param>
        /// <param name="worldPositionStays">부모 오브젝트가 있다면 위치값을 유지할 지 결정하는 변수</param>
        /// <returns>생성된 GameObject를 반환함. 애셋 타입이 잘못됐다면 null 반환.</returns>
        /// <exception cref="Exception"></exception>
        public GameObject InstantiateGameObject(UIWindowAssetType assetType, Vector3 pos, Vector3 rot, Transform parentTransform = null, bool worldPositionStays = false)
        {
            try
            {
                if (assetType == UIWindowAssetType.None)
                {
                    throw new Exception("Unexpected UIWindowAssetType!");
                }

                GameObject result;
                if (parentTransform == null)
                {
                    result = Instantiate(Core.System.Resource.LoadAssets(assetType));
                }
                else
                {
                    result = Instantiate(Core.System.Resource.LoadAssets(assetType), parentTransform, worldPositionStays);
                }

                result.transform.localPosition = pos;
                result.transform.localEulerAngles = rot;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 입력받은 타입에 맞는 애셋을 Instantiate하는 함수.
        /// </summary>
        /// <param name="assetType">생성하고자 하는 애셋 타입</param>
        /// <param name="pos">생성한 후의 초기 위치</param>
        /// <param name="rot">생성한 후의 초기 회전값</param>
        /// <param name="parentTransform">부모 오브젝트의 transform 값, 기본값은 null</param>
        /// <returns>생성된 GameObject를 반환함. 애셋 타입이 잘못됐다면 null 반환.</returns>
        /// <exception cref="Exception"></exception>
        public GameObject InstantiateGameObject(PrefabAssetType assetType, Vector3 pos, Vector3 rot, Transform parentTransform = null)
        {
            try
            {
                if (assetType == PrefabAssetType.None)
                {
                    throw new Exception("Unexpected PrefabAssetType!");
                }

                GameObject result;
                if (parentTransform == null)
                {
                    result = Instantiate(Core.System.Resource.LoadAssets(assetType));
                }
                else
                {
                    result = Instantiate(Core.System.Resource.LoadAssets(assetType), parentTransform);
                }

                result.transform.localPosition = pos;
                result.transform.localRotation = Quaternion.Euler(rot);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 입력받은 타입에 맞는 애셋을 Instantiate하는 함수.
        /// </summary>
        /// <param name="obj">생성하고자 하는 애셋의 GameObject</param>
        /// <param name="pos">생성한 후의 초기 위치</param>
        /// <param name="rot">생성한 후의 초기 회전값</param>
        /// <param name="parentTransform">부모 오브젝트의 transform 값, 기본값은 null</param>
        /// <returns>생성된 GameObject를 반환함. 애셋 타입이 잘못됐다면 null 반환.</returns>
        /// <exception cref="Exception"></exception>
        public GameObject InstantiateGameObject(GameObject obj, Vector3 pos, Vector3 rot, Transform parentTransform = null)
        {
            try
            {
                GameObject result;
                if (parentTransform == null)
                {
                    result = Instantiate(obj);
                }
                else
                {
                    result = Instantiate(obj, parentTransform);
                }

                result.transform.localPosition = pos;
                result.transform.localRotation = Quaternion.Euler(rot);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 입력 받은 GameObject를 삭제하는 함수.
        /// </summary>
        /// <param name="obj"></param>
        public void DestroyGameObject(GameObject obj)
        {
            Destroy(obj);
        }

        /// <summary>
        /// Coroutine으로 webrequest를 통해 텍스쳐를 로드하는 함수.
        /// 로드된 텍스쳐들은 사전에 등록된 max size만큼 캐싱되고, 캐싱된 텍스쳐를 로드하면 webrequest를 보내지 않고 바로 필요한 텍스쳐를 반환함.
        /// </summary>
        /// <param name="url">텍스쳐 경로 url</param>
        /// <param name="LoadFinishAction">이미지 로드가 끝나면 실행될 콜백 함수</param>
        public void LoadTexture(string url, UnityAction<Texture2D> LoadFinishAction)
        {
            LoadTextureInfo info = new LoadTextureInfo
            {
                url = url,
                LoadFinishAction = LoadFinishAction
            };
            WaitingLoadQueue.Enqueue(info);
        }

        private IEnumerator _LoadTexture(LoadTextureInfo info)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(info.url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (TextureMap.Count >= TextureMapMaxCount)
                {
                    foreach (var keyValuePair in TextureMap)
                    {
                        TextureMap.Remove(keyValuePair.Key);
                        break;
                    }
                }
                TextureMap.Add(info.url, texture);
                info.LoadFinishAction.Invoke(texture);
            }

            TextureLoadCoroutine = null;
        }

        private void FixedUpdate()
        {
            if (WaitingLoadQueue.Count > 0 && TextureLoadCoroutine == null)
            {
                LoadTextureInfo info = WaitingLoadQueue.Dequeue();
                if (TextureMap.ContainsKey(info.url) == false)
                {
                    TextureLoadCoroutine = StartCoroutine(_LoadTexture(info));
                }
                else
                {
                    Texture2D texture = TextureMap[info.url];
                    info.LoadFinishAction.Invoke(texture);
                }
            }
        }
    }
}
