using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StateInfo;
using ClientTemplate.DataManufactureInfo;
using ClientTemplate.PlayerInfo;
using UnityEngine;

namespace ClientTemplate
{
    public class ServerConnectState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public ServerConnectState()
        {
            name = "Server Connect State";
            type = StateType.ServerConnect;
        }

        public void OnBegin()
        {
            SetNetworkManager();
            StateMachine.NextState(new DataSettingState());
        }

        public bool OnEnd(StateType nextStateType)
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

        private void SetNetworkManager()
        {
            Core.System.SetNetworkManager(new NetworkManager());
            Core.System.Network.Init();
            Core.System.Network.Connect();
        }
    }
}
