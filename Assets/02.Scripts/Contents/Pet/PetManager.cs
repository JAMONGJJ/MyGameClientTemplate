using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KlayLand.PetInfo;

namespace KlayLand
{
    public interface IPetManager : IManager
    {

    }

    public class PetManager : IPetManager
    {
        
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Pet Manager");
            
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Pet Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Pet Manager");
            Release();
            Init();
        }
    }
}
