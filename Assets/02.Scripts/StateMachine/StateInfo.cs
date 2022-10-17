using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ClientTemplate
{
    namespace StateInfo
    {
        public enum StateType
        {
            None,
            Intro,
            ServerConnect,
            InitialDataLoad,
            Login,
            Field,
        
        }

        public interface IState
        {
            string name { get; set; }
            StateType id { get; set; }
            bool CanTransitState(StateType nextStateType);
            void OnBegin();
            void OnEnd();
        }
    }
}
