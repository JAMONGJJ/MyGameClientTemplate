using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace ClientTemplate
{
    public class TestModalessUIWindow : UIWindow
    {
        public class WindowData : UIData
        {
            public string testText;
        }

        public Button ExitButton;
        public TMP_Text TestText;
        
        public override void Init(UIData data = null)
        {
            SetButtons();
            WindowData windowData = data as WindowData;
            if (windowData != null)
            {
                TestText.text = windowData.testText;
                
            }
        }

        public override void Release()
        {

        }

        public override void OnTop(UIData data = null)
        {
            WindowData windowData = data as WindowData;
            if (windowData != null)
            {
                TestText.text = windowData.testText;
                
            }
        }

        private void SetButtons()
        {
            ExitButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    UIManager.Instance.CloseWindow(UIWindowType.TestModalessUIWindow);
                });
        }
    }
}
