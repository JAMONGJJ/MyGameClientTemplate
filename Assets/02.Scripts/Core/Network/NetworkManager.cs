using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    public interface INetworkManager : IManager
    {
        void Connect();
    }

    public class NetworkManager : INetworkManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Network Manager");
            
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Network Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Network Manager");
            Release();
            Init();
        }

        public void Connect()
        {
            // TODO : 서버 연결
        }
    }
}
