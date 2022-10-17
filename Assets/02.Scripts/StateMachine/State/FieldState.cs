using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StateInfo;
using ClientTemplate.SceneInfo;
using UnityEngine;

namespace ClientTemplate
{
    public class FieldState : IState
    {
        public string name { get; set; }
        public StateType id { get; set; }

        public FieldState()
        {
            name = "Field State";
            id = StateType.Field;
        }

        public bool CanTransitState(StateType nextStateType)
        {
            switch (nextStateType)
            {
                default:
                {
                    return true;
                }
            }
        }

        public void OnBegin()
        {
            // Core.System.Scene.LoadScene(SceneType.Test, AfterFieldSceneLoad);
        }

        private void AfterFieldSceneLoad()
        {
            // GamePlayManager.Instance.SetCharacterInitPosition();
            // GamePlayManager.Instance.SetMyPlayer();
            // GamePlayManager.Instance.SetMyCamera();
            // UIManager.Instance.CreateMainHud();
            // UIManager.Instance.SetOverlayCamera();
        }

        public void OnEnd()
        {
            
        }
    }
}
