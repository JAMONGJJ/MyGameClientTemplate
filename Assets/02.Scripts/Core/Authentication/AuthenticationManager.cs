using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
