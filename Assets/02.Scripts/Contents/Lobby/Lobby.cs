using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UIRegion;
using ClientTemplate.UIRegion.UIInfo;
using UnityEngine;

namespace ClientTemplate
{
    namespace LobbyRegion
    {
        public class Lobby
        {
            public void Init()
            {
                InitLobbyWindow();
            }

            public void Release()
            {
                ReleaseLobbyWindow();
            }

#region LobbyWindow

            private void InitLobbyWindow()
            {
                UIManager.Instance.OpenWindow<MainHud>(UIWindowType.MainHud);
            }

            private void ReleaseLobbyWindow()
            {
                UIManager.Instance.CloseWindow(UIWindowType.MainHud);
            }

#endregion
        }
    }
}
