using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KlayLand
{
    public interface IHousingManager : IManager
    {

    }

    public class HousingManager : IHousingManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Housing Manager");
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Housing Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Housing Manager");
            Release();
            Init();
        }
    }
}
