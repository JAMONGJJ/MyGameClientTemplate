using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientTemplate.StateInfo;

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
            StateMachine.NextState(new ServerConnectState());
        }

        public void OnEnd()
        {

        }
    }
}
