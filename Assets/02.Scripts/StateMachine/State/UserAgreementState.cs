using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ClientTemplate
{
    using UIInfo;
    using StateInfo;

    public class UserAgreementState : IState
    {
        public string name { get; set; }
        public StateType id { get; set; }

        public UserAgreementState()
        {
            name = "User Agreement State";
            id = StateType.UserAgreement;
        }

        public bool CanTransitState(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.InitialDataLoad:
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

            StateMachine.NextState(new InitialDataLoadState());
        }

        public void OnEnd()
        {
            
        }

        private void ShowUserAgreementWindow()
        {
            
        }
    }
}