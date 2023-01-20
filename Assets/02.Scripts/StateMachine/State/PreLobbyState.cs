using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    using UIInfo;
    using StateInfo;

    public class PreLobbyState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public PreLobbyState()
        {
            name = "Pre Lobby State";
            type = StateType.PreLobby;
        }

        public void OnBegin()
        {
        }

        public bool OnEnd(StateType nextStateType)
        {
            switch (nextStateType)
            {
                default:
                {
                    return true;
                }
            }
        }
    }
}
