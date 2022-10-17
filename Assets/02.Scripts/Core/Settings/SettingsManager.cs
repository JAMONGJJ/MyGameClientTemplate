using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    public interface ISettingsManager : IManager
    {
        void SetFrameRate(int fps);
    }

    public class SettingsManager : ISettingsManager
    {
        public void Init()
        {
            
        }

        public void Release()
        {
            
        }

        public void ReSet()
        {
            Release();
            Init();
        }

        public void SetFrameRate(int fps)
        {
            Application.targetFrameRate = fps;
        }
    }
}
