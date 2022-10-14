using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KlayLand
{
    public interface IDialogManager : IManager
    {

    }

    public class DialogManager : IDialogManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Dialog Manager");
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Dialog Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Dialog Manager");
            Release();
            Init();
        }
    }
}
