using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StateInfo;
using UnityEngine;
using UniRx;

namespace ClientTemplate
{
    public class InitialDataLoadState : IState
    {
        public string name { get; set; }
        public StateType id { get; set; }

        public InitialDataLoadState()
        {
            name = "Initial Data Load State";
            id = StateType.InitialDataLoad;
        }

        public bool CanTransitState(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.ServerConnect:
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
            GameEntryManager.Instance.CheckForAssetsToDownload();
        }

        public void OnEnd()
        {
            
        }
    }
}
