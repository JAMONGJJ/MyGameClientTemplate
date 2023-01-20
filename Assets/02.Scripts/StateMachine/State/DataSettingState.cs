using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientTemplate.StateInfo;
using ClientTemplate.UIInfo;
using ClientTemplate.UtilityFunctions;

namespace ClientTemplate
{
    public class DataSettingState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public DataSettingState()
        {
            name = "Data Setting State";
            type = StateType.DataSetting;
        }

        public void OnBegin()
        {
            // synchronized setting
            InitUIManager();
            
            DataSetAllFinishCallback();
        }

        public bool OnEnd(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.PreLobby:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        private void InitUIManager()
        {
            UIManager.Instance.SetUIWindowContainer(new UIWindowContainerWithStack());
            UIManager.Instance.SetUIWindowAssetTypeContainer(new UIWindowAssetTypeContainer());
            UIManager.Instance.SetUIDataInfoContainer(new UIDataInfoContainer());
            UIManager.Instance.Init();
            
            UIManager.Instance.LoadUISystem();
            UIManager.Instance.CreateMainHud();
            UIManager.Instance.SetInActiveMainHud();
        }

        private void DataSetAllFinishCallback()
        {
            StateMachine.NextState(new PreLobbyState());
        }
    }
}
