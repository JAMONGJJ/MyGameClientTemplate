using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    using UIInfo;
    using StateInfo;

    public class LoginState : IState
    {
        public string name { get; set; }
        public StateType type { get; set; }

        public LoginState()
        {
            name = "Login State";
            type = StateType.Login;
        }

        public void OnBegin()
        {
            Login();
            
        }

        public bool OnEnd(StateType nextStateType)
        {
            switch (nextStateType)
            {
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

        private void Login()
        {
            #if UNITY_EDITOR
            Core.System.SetAuthenticationManager(new AuthenticationManager());
            #elif UNITY_ANDROID
            Core.System.SetAuthenticationManager(new AndroidAuthenticationManager());
            #elif UNITY_IOS
            Core.System.SetAuthenticationManager(new IOSAuthenticationManager());
            #endif
            Core.System.Authentication.Init();
            Core.System.Authentication.Authenticate();
            
            GameEntryManager.Instance.DestroySelf();
            StateMachine.NextState(new LobbyState());
        }
    }
}
