using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KlayLand
{
    public static class Core
    {
        public static CoreSystem System { get; } = new CoreSystem();
    }
    
    /// <summary>
    /// KlayLand가 아니어도 있을 법한 공통적인 기능들을 모아놓은 클래스
    /// </summary>
    public class CoreSystem
    {
        public ISceneManager Scene { get; private set; } // 씬 이동과 관련된 기능을 수행
        public IDataManufactureManager DataManufacture { get; private set; }    // 데이터 가공 기능을 수행(서버에서 받아온 데이터, 서버로 보낼 데이터, Model <-> View 전환 등)
        public ISerializeManager Serialize { get; private set; }    // 데이터 저장과 로드 기능을 수행(로컬)
        public IResourceManager Resource { get; private set; }  // 게임 리소스를 로드하고 해제하는 기능
        public ISettingsManager Settings { get; private set; }  // 게임 설정값(프레임레이트, 언어 등) 관리
        public INetworkManager Network { get; private set; }    // 서버 통신 관리
        public IAuthenticationManager Authentication { get; private set; }    // 사용자 인증 관리

        public void SetSceneManager(ISceneManager manager)
        {
            Scene = manager;
        }

        public void SetDataManufactureManager(IDataManufactureManager manager)
        {
            DataManufacture = manager;
        }

        public void SetSerializeManager(ISerializeManager manager)
        {
            Serialize = manager;
        }

        public void SetResourceManager(IResourceManager manager)
        {
            Resource = manager;
        }

        public void SetSettingsManager(ISettingsManager manager)
        {
            Settings = manager;
        }

        public void SetNetworkManager(INetworkManager manager)
        {
            Network = manager;
        }

        public void SetAuthenticationManager(IAuthenticationManager manager)
        {
            Authentication = manager;
        }
    }
}
