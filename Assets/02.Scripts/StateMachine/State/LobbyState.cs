using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StateInfo;
using ClientTemplate.SceneRegion;
using ClientTemplate.SceneRegion.SceneInfo;
using ClientTemplate.UIRegion;
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
            SceneManager.Instance.LoadScene(SceneType.Lobby, AfterFieldSceneLoad);
        }

        private void AfterFieldSceneLoad()
        {
            UIManager.Instance.PushToMainCameraStack();
            Contents.Manager.Lobby.Init();
        }

        public bool OnEnd(StateType nextStateType)
        {
            Contents.Manager.Lobby.Release();
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
