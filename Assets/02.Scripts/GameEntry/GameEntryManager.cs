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

        public override void Release()
        {
            
        }

        public void GameEntry()
        {
            GameEntryWindow = GameObject.FindWithTag("GameEntryWindow").GetComponent<UIGameEntryWindow>();
            GameEntryWindow.Init();
            CheckAvailableAppVersion();
        }

        private void CheckAvailableAppVersion()
        {
            VersionType versionType = Core.System.Version.GetVersionType();
            switch (versionType)
            {
                case VersionType.Play:
                {
                    StateMachine.NextState(new InitialDataLoadState());
                }
                    break;
                case VersionType.Update:
                {
                    GameEntryWindow.SetActiveGoToStore(true);
                }
                    break;
                case VersionType.Inspect:
                {

                }
                    break;
                default:
                {

                }
                    break;
            }
        }
    }
}
