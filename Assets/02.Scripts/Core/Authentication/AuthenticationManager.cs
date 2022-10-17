using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace ClientTemplate
{
    public interface IAuthenticationManager : IManager
    {
        void Authenticate();
    }

    public class AndroidAuthenticationManager : IAuthenticationManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Android Authentication Manager");
            
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Android Authentication Manager");
            
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Android Authentication Manager");
            Release();
            Init();
        }

        public void Authenticate()
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }

        private void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success) {
                // Continue with Play Games Services
            } else {
                // Disable your integration with Play Games Services or show a login button
                // to ask users to sign-in. Clicking it should call
                // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            }
        }
    }

    public class IOSAuthenticationManager : IAuthenticationManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "IOS Authentication Manager");
            
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "IOS Authentication Manager");
            
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "IOS Authentication Manager");
            Release();
            Init();
        }

        public void Authenticate()
        {
            
        }

    }
}
