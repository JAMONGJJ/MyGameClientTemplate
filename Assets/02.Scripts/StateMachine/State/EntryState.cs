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
            InitCoreManagers();
            InitContentsManager();
            Core.System.Settings.SetFrameRate(30);
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
        
        private void InitCoreManagers()
        {
            SceneManager.Instance.Init();
            GameObjectManager.Instance.Init();
            
            Core.System.SetDataManufactureManager(new DataManufactureManager());
            Core.System.DataManufacture.SetDelegateContainer(new ParseDelegateContainer());
            Core.System.DataManufacture.Init();
            
            Core.System.SetSerializeManager(new SerializeManager());
            Core.System.Serialize.Init();
            
            Core.System.SetResourceManager(new ResourceManager());
            Core.System.Resource.Init();
            
            Core.System.SetSettingsManager(new SettingsManager());
            Core.System.Settings.Init();
            
            Core.System.SetVersionManager(new VersionControlManager());
            Core.System.Version.Init();
        }
        
        #endregion
        
        #region Contents Manager
        
        private void InitContentsManager()
        {
            
        }
        
        #endregion
    }
}
