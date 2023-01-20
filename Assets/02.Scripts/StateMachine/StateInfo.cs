using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace StateInfo
    {
        public enum StateType
        {
            None,
            Entry,
            InitialDataLoad,
            DataSetting,
            ServerConnect,
            Login,
            Lobby,
        
        }

        public interface IState
        {
            string name { get; set; }
            StateType type { get; set; }
            void OnBegin();
            bool OnEnd(StateType nextStateType);
        }
    }
}
