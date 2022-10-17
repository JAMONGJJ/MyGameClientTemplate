using System.Collections;
using System.Collections.Generic;
using KlayLand.UIInfo;
using UnityEngine;

namespace KlayLand
{
    public class GameEntryManager : MonoManager<GameEntryManager>
    {
        [HideInInspector] public UIGameEntryWindow GameEntryWindow;
        [HideInInspector] public UIDataLoadWindow DataLoadWindow;
        
        public override void Init()
        {
            GameEntryWindow = FindObjectOfType<UIGameEntryWindow>();
            DataLoadWindow = FindObjectOfType<UIDataLoadWindow>();

            DataLoadWindow.gameObject.SetActive(false);
        }

        public override void Release()
        {
        }

        public override void ReSet()
        {
        }

        public void EnterGame()
        {
            StateMachine.NextState(new IntroState());
        }

        public void CheckForAssetsToDownload()
        {
            Core.System.Resource.CheckAssetToDownload(SizeCheckCallback, LoadFinishCallback);
        }

        private void SizeCheckCallback(long size, System.Action callback)
        {
            DataLoadWindow.gameObject.SetActive(true);
            DataLoadWindow.SetDescText(size);
            DataLoadWindow.SetActiveDownloadButton(true);
            DataLoadWindow.SetActiveDownloadSlider(false);
            DataLoadWindow.SetDownloadButton(callback);
        }

        private void LoadFinishCallback()
        {
            StateMachine.NextState(new ServerConnectState());
        }
    }
}
