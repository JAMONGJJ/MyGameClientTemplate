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
        public StateType type { get; set; }

        public FieldState()
        {
            name = "Field State";
            type = StateType.Field;
        }

        public void OnBegin()
        {
            Core.System.Scene.LoadScene(SceneType.TestScene, AfterFieldSceneLoad);
        }

        private void AfterFieldSceneLoad()
        {
            UIManager.Instance.SetOverlayCamera();
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
