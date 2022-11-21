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
        public StateType id { get; set; }

        public DataSettingState()
        {
            name = "Data Setting State";
            id = StateType.DataSetting;
        }

        public bool CanTransitState(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.ServerConnect:
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
            // synchronized setting
            InitUIManager();
            
            // asynchronized setting
            Utility.Functions.Async.Process(DataSetAllFinishCallback,
                UIManager.Instance.LoadUISystem,
                UIManager.Instance.CreateMainHud,
                UIManager.Instance.SetInActiveMainHud
            );
        }

        public void OnEnd()
        {
            
        }

        private void InitUIManager()
        {
            UIManager.Instance.SetUIWindowContainer(new UIWindowContainerWithStack());
            UIManager.Instance.SetUIWindowAssetTypeContainer(new UIWindowAssetTypeContainer());
            UIManager.Instance.SetUIDataInfoContainer(new UIDataInfoContainer());
            UIManager.Instance.Init();
        }

        private void DataSetAllFinishCallback()
        {
            StateMachine.NextState(new ServerConnectState());
        }
    }
}
