using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UIInfo;
using ClientTemplate.UtilityFunctions;
using UnityEngine;

namespace ClientTemplate
{
    public class GameEntryManager : MonoManager<GameEntryManager>
    {
        [HideInInspector]
        public UIGameEntryWindow GameEntryWindow;
        
        public override void Init()
        {
        }

        public void GameEntry()
        {
            GameEntryWindow = GameObject.FindWithTag("GameEntryWindow").GetComponent<UIGameEntryWindow>();
        }

        private void CheckAvailableAppVersion()
        {
            VersionsDataTable versionInfo = Info.Table.GetVersionInfo();
            if(versionInfo.version != Application.version)
            {
                GameEntryWindow.SetActiveGoToStore(true);
            }
            else
            {
                StateMachine.NextState(new InitialDataLoadState());
            }
        }
    }
}
