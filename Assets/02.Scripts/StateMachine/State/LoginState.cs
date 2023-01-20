using System.Collections;
using System.Collections.Generic;
using ClientTemplate.SceneInfo;
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
            Core.System.Scene.LoadScene(SceneType.PreLobby);
            UIManager.Instance.SetOverlayCamera();
            Login();
        }

        public bool OnEnd(StateType nextStateType)
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
            StateMachine.NextState(new ServerConnectState());
        }
    }
}
