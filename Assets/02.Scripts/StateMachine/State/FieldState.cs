using System.Collections;
using System.Collections.Generic;
using KlayLand.SceneInfo;
using KlayLand.StateInfo;
using UnityEngine;

namespace KlayLand
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
            Core.System.Scene.LoadScene(SceneType.StayUp, AfterFieldSceneLoad);
        }

        private void AfterFieldSceneLoad()
        {
            UIManager.Instance.CreateMainHud();
            GamePlayManager.Instance.SetCharacterInitPosition();
            GamePlayManager.Instance.SetMyPlayer();
            GamePlayManager.Instance.SetMyCamera();
        }

        public void OnEnd()
        {
            
        }
    }
}
