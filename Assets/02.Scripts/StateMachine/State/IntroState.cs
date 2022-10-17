using System.Collections;
using System.Collections.Generic;
using ClientTemplate.DataManufactureInfo;
using Unity.VisualScripting;
using UnityEngine;

namespace ClientTemplate
{
    using UIInfo;
    using StateInfo;
    using SceneInfo;
    
    public class IntroState : IState
    {
        public string name { get; set; }
        public StateType id { get; set; }

        public IntroState()
        {
            name = "Intro State";
            id = StateType.Intro;
        }

        public bool CanTransitState(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.InitialDataLoad:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        public void OnBegin()
        {
            SetCoreSystem();
            SetUIManager();
            SetGamePlayManager();
            SetContentManager();
            
            StateMachine.NextState(new InitialDataLoadState());
        }

        public void OnEnd()
        {
            Core.System.Settings.SetFrameRate(60);
        }
        
        #region CoreSystem
        private void SetCoreSystem()
        {
            SetSceneManager();
            SetDataManufactureManager();
            SetSerializeManager();
            SetResourceManager();
            SetSettingsManager();
        }

        private void SetSceneManager()
        {
            // Core.System.SetSceneManager(new SceneManagerForTest());
            Core.System.SetSceneManager(new SceneManager());
            Core.System.Scene.SetSceneContainer(new SceneContainer());
            Core.System.Scene.Init();
        }

        private void SetDataManufactureManager()
        {
            Core.System.SetDataManufactureManager(new DataManufactureManager());
            Core.System.DataManufacture.SetDelegateContainer(new ParseDelegateContainer());
            Core.System.DataManufacture.Init();
        }

        private void SetSerializeManager()
        {
            Core.System.SetSerializeManager(new SerializeManager());
            Core.System.Serialize.Init();
        }

        private void SetResourceManager()
        {
            Core.System.SetResourceManager(new ResourceManager());
            Core.System.Resource.Init();
        }

        private void SetSettingsManager()
        {
            Core.System.SetSettingsManager(new SettingsManager());
            Core.System.Settings.Init();
        }
        #endregion
        
        #region UIManager
        private void SetUIManager()
        {
            UIManager.Instance.SetUIWindowContainer(new UIWindowContainerWithStack());
            UIManager.Instance.SetUIWindowAssetTypeContainer(new UIWindowAssetTypeContainer());
            UIManager.Instance.SetUIDataInfoContainer(new UIDataInfoContainer());
            UIManager.Instance.Init();
        }
        #endregion

        #region GamePlayManager
        private void SetGamePlayManager()
        {
            GamePlayManager.Instance.Init();
        }
        #endregion
        
        #region ContentManager
        private void SetContentManager()
        {
            
        }
        #endregion
    }
}
