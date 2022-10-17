using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    public interface IAuthenticationManager : IManager
    {
        void Authenticate();
    }

    public class AuthenticationManager : IAuthenticationManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Authentication Manager");
            
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Authentication Manager");
            
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Authentication Manager");
            Release();
            Init();
        }

        public void Authenticate()
        {
            
        }
    }
}
