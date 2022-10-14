using System.Collections;
using System.Collections.Generic;
using KlayLand.DataManufactureInfo;
using KlayLand.PlayerInfo;
using KlayLand.StateInfo;
using UnityEngine;

namespace KlayLand
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
            Core.System.SetNetworkManager(new NetworkManager());
            Core.System.Network.Init();
            StateMachine.NextState(new LoginState());
        }

        public void OnEnd()
        {
            
        }
    }
}
