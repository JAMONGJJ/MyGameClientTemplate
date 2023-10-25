using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace LobbyRegion
    {
        public class LobbyManager
        {
            private Lobby Lobby;
            
            public void Init()
            {
                Lobby = new Lobby();
                Lobby.Init();
            }

            public void Release()
            {
                Lobby.Release();
                Lobby = null;
            }
        
        
        }
    }
}
