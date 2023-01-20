using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StateInfo;
using ClientTemplate.UIInfo;
using UnityEngine;

namespace ClientTemplate
{
    public class InitialDataLoadState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public InitialDataLoadState()
        {
            name = "Initial Data Load State";
            type = StateType.InitialDataLoad;
        }

        public void OnBegin()
        {
            Core.System.Resource.CheckDownloadAssets(LoadFinishCallback);
        }

        private void LoadFinishCallback()
        {
            InitUIManager();
            StateMachine.NextState(new LoginState());
        }

        private void InitUIManager()
        {
            UIManager.Instance.SetUIWindowContainer(new UIWindowContainerWithStack());
            UIManager.Instance.SetUIWindowAssetTypeContainer(new UIWindowAssetTypeContainer());
            UIManager.Instance.SetUIDataInfoContainer(new UIDataInfoContainer());
            UIManager.Instance.Init();
            
            UIManager.Instance.LoadUISystem();
        }

        public bool OnEnd(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.Login:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }
    }
}
