using System.Collections;
using System.Collections.Generic;
using ClientTemplate.DataManufactureInfo;
using ClientTemplate.SceneRegion;
using ClientTemplate.StateInfo;
using ClientTemplate.UIRegion.UIInfo;
using ClientTemplate.UtilityFunctions;
using UnityEngine;

namespace ClientTemplate
{
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
            Core.System.SetSettingsManager(new SettingsManager());
            Core.System.Settings.Init();
            
            Core.System.SetVersionManager(new VersionControlManager());
            Core.System.Version.Init();
            
            Core.System.Settings.SetResolutionType(ResolutionType.Fixed);
            
#if !UNITY_EDITOR && !TEST_BUILD
            Debug.unityLogger.logEnabled = false;
            Core.System.Settings.SetFrameRate(30);
#endif
            
            GameEntryManager.Instance.Init();
            GameEntryManager.Instance.GameEntry();
        }

        public bool OnEnd(StateType nextStateType)
        {
            InitMonoManagers();
            InitCoreManagers();
            InitContentsManagers();
            Core.System.Settings.SetLanguageType(SystemLanguage.English);
            
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
        
        private void InitMonoManagers()
        {
            SceneManager.Instance.Init();
            GameObjectManager.Instance.Init();
        }
        
        private void InitCoreManagers()
        {
            Core.System.SetDataManufactureManager(new DataManufactureManager());
            Core.System.DataManufacture.SetDelegateContainer(new ParseDelegateContainer());
            Core.System.DataManufacture.Init();
            
            Core.System.SetSerializeManager(new SerializeManager());
            Core.System.Serialize.Init();
            
            Core.System.SetResourceManager(new ResourceManager());
            Core.System.Resource.Init();
        }
        
        private void InitContentsManagers()
        {
            
        }
    }
}
