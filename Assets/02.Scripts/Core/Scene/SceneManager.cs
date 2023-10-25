using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ClientTemplate
{
    namespace SceneRegion
    {
        using SceneRegion.SceneInfo;
        public class SceneManager : MonoManager<SceneManager>
        {
            public Scene CurrentScene { get; private set; }
            
            private SceneContainer SceneContainer;
            private UnityAction SceneLoadCallback;
            
            public override void Init()
            {
                LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Scene Manager");
                CurrentScene = new Scene("EntryScene", SceneAssetType.Entry);
                SceneLoadCallback = null;
                SceneContainer = new SceneContainer();
                SceneContainer.Add(SceneType.Entry, "EntryScene", SceneAssetType.Entry);
                SceneContainer.Add(SceneType.Lobby, "LobbyScene", SceneAssetType.Lobby);
                SceneContainer.Add(SceneType.EmptyScene, "EmptyScene", SceneAssetType.EmptyScene);
            }

            public override void Release()
            {
                LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Scene Manager");
                CurrentScene = null;
                SceneLoadCallback = null;
                SceneContainer = null;
            }

            public void LoadScene(SceneType type, UnityAction callback)
            {
                Scene scene = SceneContainer.GetScene(type);
                if (scene != null)
                {
                    SceneLoadCallback = callback;
                    StartCoroutine(_LoadScene(scene));
                }
            }

            private IEnumerator _LoadScene(Scene newScene)
            {
                LogManager.Log(LogManager.LogType.SCENE_LOADING_START, newScene.sceneName);
                SceneInstance emptyScene = Core.System.Resource.LoadAssets(SceneAssetType.EmptyScene);
                yield return emptyScene.ActivateAsync();

                yield return Resources.UnloadUnusedAssets();
                
                SceneInstance sceneInstance = Core.System.Resource.LoadAssets(newScene.assetType);
                yield return sceneInstance.ActivateAsync();

                CurrentScene = newScene;
                LogManager.Log(LogManager.LogType.SCENE_LOADING_FINISH, newScene.sceneName);
                if (SceneLoadCallback != null)
                {
                    SceneLoadCallback.Invoke();
                    SceneLoadCallback = null;
                }
            }
        }
    }
}
