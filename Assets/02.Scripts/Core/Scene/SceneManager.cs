using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using KlayLand.ResourceInfo;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KlayLand
{
    using SceneInfo;

    public delegate void AfterSceneLoadCallback();
    
    public interface ISceneManager : IManager
    {
        Scene CurrentScene { get; set; }
        AfterSceneLoadCallback SceneLoadCallback { get; set; }
        void SetSceneContainer(ISceneContainer container);
        void LoadScene(SceneType type, AfterSceneLoadCallback callback);
    }

    public class SceneManager : ISceneManager
    {
        public Scene CurrentScene { get; set; }
        public AfterSceneLoadCallback SceneLoadCallback { get; set; }
        private ISceneContainer SceneContainer { get; set; }

        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Scene Manager");
            CurrentScene = new Scene("EntryScene", SceneAssetType.EntryScene);
            SceneLoadCallback = null;
            SceneContainer.Add(SceneType.Entry, "EntryScene", SceneAssetType.EntryScene);
            SceneContainer.Add(SceneType.Test, "TestScene", SceneAssetType.TestScene);
            SceneContainer.Add(SceneType.StayUp, "StayUpScene", SceneAssetType.StayUpScene);
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
            Scene scene = SceneContainer.GetValue(type);
            if (scene != null)
            {
                _LoadScene(scene);
                SceneLoadCallback = callback;
            }
        }

        private async void _LoadScene(Scene newScene)
        {
            LogManager.Log(LogManager.LogType.SCENE_LOADING_START, newScene.sceneName);
            string sceneAddress = Core.System.Resource.GetAddressByType(newScene.assetType);
            var asyncLoad = Core.System.Resource.LoadScene(sceneAddress);
            while (asyncLoad.IsDone == false)
            {
                await (Task.Delay(10));
            }
            Addressables.Release(asyncLoad);
            CurrentScene = newScene;
            if (SceneLoadCallback != null)
            {
                SceneLoadCallback.Invoke();
                SceneLoadCallback = null;
            }

            LogManager.Log(LogManager.LogType.SCENE_LOADING_FINISH, newScene.sceneName);
        }

        private void OpenLoadingScreen()
        {

        }

        private void CloseLoadingScreen()
        {

        }
    }
}
