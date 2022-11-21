using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StateInfo;
using UnityEngine;
using UniRx;

namespace ClientTemplate
{
    public class InitialDataLoadState : IState
    {
        public string name { get; set; }
        public StateType id { get; set; }

        public InitialDataLoadState()
        {
            name = "Initial Data Load State";
            id = StateType.InitialDataLoad;
        }

        public bool CanTransitState(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.DataSetting:
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
            Core.System.Resource.CheckDownloadAssets(LoadFinishCallback);
        }

        private void LoadFinishCallback()
        {
            StateMachine.NextState(new DataSettingState());
        }

        public void OnEnd()
        {
            
        }
    }
}
