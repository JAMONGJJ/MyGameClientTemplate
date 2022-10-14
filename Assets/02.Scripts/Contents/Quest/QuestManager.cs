using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KlayLand
{
    public interface IQuestManager : IManager
    {

    }

    public class QuestManager : IQuestManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Quest Manager");
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Quest Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Quest Manager");
            Release();
            Init();
        }
    }
}
