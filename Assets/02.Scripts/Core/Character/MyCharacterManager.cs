using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    public interface IMyCharacterManager : IManager
    {

    }

    public class MyCharacterManager : IMyCharacterManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "MyCharacter Manager");
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "MyCharacter Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "MyCharacter Manager");
            Release();
            Init();
        }
    }
}
