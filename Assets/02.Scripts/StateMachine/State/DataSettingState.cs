using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientTemplate.StateInfo;
using ClientTemplate.UIInfo;
using ClientTemplate.UtilityFunctions;

namespace ClientTemplate
{
    public class DataSettingState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public DataSettingState()
        {
            name = "Data Setting State";
            type = StateType.DataSetting;
        }

        public void OnBegin()
        {
            DataSetAllFinishCallback();
        }

        public bool OnEnd(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.PreLobby:
                case StateType.Lobby:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        private void DataSetAllFinishCallback()
        {
            GameEntryManager.Instance.GameEntryWindow.SetActivePlayButton(true);
        }
    }
}
