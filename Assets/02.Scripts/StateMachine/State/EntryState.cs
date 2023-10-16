using System.Collections;
using System.Collections.Generic;
using ClientTemplate.DataManufactureInfo;
using ClientTemplate.UtilityFunctions;
using Unity.VisualScripting;
using UnityEngine;

namespace ClientTemplate
{
    using UIInfo;
    using StateInfo;
    using SceneInfo;
    
    public class EntryState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public EntryState()
        {
            name = "Entry State";
            type = StateType.Entry;
        }

        public void OnBegin()
        {
            SetCoreSystem();
            SetContentsManager();
            SetUtilityFunctions();
            Core.System.Settings.SetFrameRate(60);
            GameEntryManager.Instance.GameEntry();
        }

        public bool OnEnd(StateType nextStateType)
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
            Core.System.Resource.Init();
        }

        private void SetSettingsManager()
        {
            Core.System.SetSettingsManager(new SettingsManager());
            Core.System.Settings.Init();
        }
        #endregion
        
        #region Contents Manager
        private void SetContentsManager()
        {
            
        }
        #endregion

        #region UtilityFunctions
        private void SetUtilityFunctions()
        {
            SetAsyncOperationHandler();
            SetExceptionHandler();
        }

        private void SetAsyncOperationHandler()
        {
            Utility.Functions.SetAsyncOperationHandler(new AsyncOperationHandler());
            Utility.Functions.Async.Init();
        }

        private void SetExceptionHandler()
        {
            Utility.Functions.SetExceptionHandler(new ExceptionHandler());
            Utility.Functions.Exception.Init();
        }
        #endregion
    }
}
