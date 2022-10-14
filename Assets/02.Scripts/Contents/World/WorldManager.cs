using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KlayLand
{
    public interface IWorldManager : IManager
    {
        
    }

    public class WorldManager : IWorldManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "World Manager");
            
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "World Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "World Manager");
            Release();
            Init();
        }
    }
}
