using System.Collections;
using System.Collections.Generic;
using KlayLand.UIInfo;
using UnityEngine;

namespace KlayLand
{
    public interface ICustomizingManager : IManager
    {
        void OpenCustomizeWindow();
    }
    
    public class CustomizingManager : ICustomizingManager
    {
        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Customizing Manager");
            
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Customizing Manager");

        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Customizing Manager");
            Release();
            Init();
        }

        public void OpenCustomizeWindow()
        {
            
        }
    }

    public class CustomizingManagerForTest : ICustomizingManager
    {
        public void Init()
        {
            Debug.Log("Customize Manager for test init!");
            
        }

        public void Release()
        {
            Debug.Log("Customize Manager for test release!");

        }

        public void ReSet()
        {
            Debug.Log("Customize Manager for test reset!");
            Release();
            Init();
        }

        public void OpenCustomizeWindow()
        {
            
        }
    }
}
