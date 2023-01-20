using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTemplate.ResourceInfo;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ClientTemplate
{
    using SceneInfo;

    public delegate void AfterSceneLoadCallback();
    
    public interface ISceneManager : IManager
    {
        Scene CurrentScene { get; set; }
        AfterSceneLoadCallback SceneLoadCallback { get; set; }
        void SetSceneContainer(ISceneContainer container);
        void LoadScene(SceneType type, AfterSceneLoadCallback callback);
        void LoadScene(SceneType type);
    }

    public class SceneManager : ISceneManager
    {
        public Scene CurrentScene { get; set; }
        public AfterSceneLoadCallback SceneLoadCallback { get; set; }
        private ISceneContainer SceneContainer { get; set; }

        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Scene Manager");
            CurrentScene = new Scene("EntryScene", SceneAssetType.Entry);
            SceneLoadCallback = null;
            SceneContainer.Add(SceneType.Entry, "EntryScene", SceneAssetType.Entry);
            SceneContainer.Add(SceneType.PreLobby, "PreLobbyScene", SceneAssetType.PreLobby);
            SceneContainer.Add(SceneType.Lobby, "LobbyScene", SceneAssetType.Lobby);
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Scene Manager");
            CurrentScene = null;
            SceneLoadCallback = null;
            SceneContainer = null;
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Scene Manager");
            Release();
            Init();

        }

        public void SetSceneContainer(ISceneContainer container)
        {
            SceneContainer = container;
        }

        public void LoadScene(SceneType type, AfterSceneLoadCallback callback)
        {
            Scene scene = SceneContainer.GetScene(type);
            if (scene != null)
            {
                _LoadScene(scene);
                SceneLoadCallback = callback;
            }
        }

        public void LoadScene(SceneType type)
        {
            Scene scene = SceneContainer.GetScene(type);
            if (scene != null)
            {
                _LoadScene(scene);
                SceneLoadCallback = null;
            }
        }

        private void _LoadScene(Scene newScene)
        {
            LogManager.Log(LogManager.LogType.SCENE_LOADING_START, newScene.sceneName);
            bool result = Core.System.Resource.LoadAssets(newScene.assetType);
            if (result == false)
            {
                LogManager.Log(LogManager.LogType.SCENE_LOADING_FAIL, newScene.sceneName);
                return;
            }

            CurrentScene = newScene;
            if (SceneLoadCallback != null)
            {
                SceneLoadCallback.Invoke();
                SceneLoadCallback = null;
            }

            LogManager.Log(LogManager.LogType.SCENE_LOADING_FINISH, newScene.sceneName);
        }
    }
}
