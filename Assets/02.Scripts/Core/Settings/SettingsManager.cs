using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UIRegion.UIInfo;
using UnityEngine;

namespace ClientTemplate
{
    public interface ISettingsManager : IManager
    {
        SystemLanguage LanguageType { get; }
        ResolutionType ResolutionType { get; }
        void SetFrameRate(int fps);
        void SetLanguageType(SystemLanguage type);
        void SetResolutionType(ResolutionType type);
    }

    public class SettingsManager : ISettingsManager
    {
        public SystemLanguage LanguageType { get; private set; }
        public ResolutionType ResolutionType { get; private set; }

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

        public void SetResolutionType(ResolutionType type)
        {
            ResolutionType = type;
        }
    }
}
