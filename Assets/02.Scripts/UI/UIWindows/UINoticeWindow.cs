using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UIInfo;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ClientTemplate
{
    public class UINoticeWindow : UIWindow
    {
        public class WindowData : UIData
        {
            public string message;
            public bool enableCancelButton;
            public System.Action okButtonCallback;

            public WindowData()
            {
                message = string.Empty;
                enableCancelButton = false;
                okButtonCallback = null;
            }
        }

        private WindowData _myWindowData;
        public Button ExitButton;
        
        public override void Init(UIData data = null)
        {
            SetButtons();
        }

        public override void Release()
        {

        }

        public override void OnTop(UIData data = null)
        {
            
        }

        private void SetOnTopSubject()
        {
            
        }

        private void SetButtons()
        {
            ExitButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    UIManager.Instance.CloseWindow(UIWindowType.NoticeWindow);
                });
        }
    }
}
