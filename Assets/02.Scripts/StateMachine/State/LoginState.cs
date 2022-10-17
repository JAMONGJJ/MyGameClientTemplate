using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ClientTemplate
{
    using UIInfo;
    using StateInfo;

    public class LoginState : IState
    {
        public string name { get; set; }
        public StateType id { get; set; }

        public LoginState()
        {
            name = "Login State";
            id = StateType.Login;
        }

        public bool CanTransitState(StateType nextStateType)
        {
            switch (nextStateType)
            {
                case StateType.Field:
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
            Login();
            
        }

        public void OnEnd()
        {
            
        }

        private void Login()
        {
            Core.System.SetAuthenticationManager(new AuthenticationManager());
            Core.System.Authentication.Init();
            Core.System.Authentication.Authenticate();
            
            GameEntryManager.Instance.DestroySelf();
            StateMachine.NextState(new FieldState());
        }
    }
}
