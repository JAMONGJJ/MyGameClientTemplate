using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    public enum VersionType
    {
        None,
        Play,
        Update,
        Inspect,
        
    }

    public interface IVersionControlManager : IManager
    {
        VersionType GetVersionType();
    }

    public class VersionControlManager : IVersionControlManager
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

        public VersionType GetVersionType()
        {
            VersionType result = VersionType.Play;
            string version = Application.version;
            // TODO : version 문자열에 따라서 Play, Update, Block 구분하는 코드 작성 필요
            return result;
        }
    }
}
