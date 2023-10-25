using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    public interface ISettingsManager : IManager
    {
        SystemLanguage LanguageType { get; }
        void SetFrameRate(int fps);
        void SetLanguageType(SystemLanguage type);
    }

    public class SettingsManager : ISettingsManager
    {
        public SystemLanguage LanguageType { get; private set; }

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

        public void SetLanguageType(SystemLanguage type)
        {
            LanguageType = type;
        }
    }
}
