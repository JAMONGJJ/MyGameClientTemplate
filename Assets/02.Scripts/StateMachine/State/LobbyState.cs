using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StateInfo;
using ClientTemplate.SceneInfo;
using UnityEngine;

namespace ClientTemplate
{
    public class LobbyState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public LobbyState()
        {
            name = "Lobby State";
            type = StateType.Lobby;
        }

        public void OnBegin()
        {
            Core.System.Scene.LoadScene(SceneType.Lobby, AfterFieldSceneLoad);
        }

        private void AfterFieldSceneLoad()
        {
            UIManager.Instance.PushToMainCameraStack();
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
