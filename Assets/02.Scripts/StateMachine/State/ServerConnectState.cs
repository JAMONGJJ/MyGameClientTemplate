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
        public StateType id { get; set; }

        public ServerConnectState()
        {
            name = "Server Connect State";
            id = StateType.ServerConnect;
        }

        public bool CanTransitState(StateType nextStateType)
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

        public void OnBegin()
        {
            SetNetworkManager();
            StateMachine.NextState(new LoginState());
        }

        public void OnEnd()
        {
            
        }

        private void SetNetworkManager()
        {
            Core.System.SetNetworkManager(new NetworkManager());
            Core.System.Network.Init();
        }
    }
}
