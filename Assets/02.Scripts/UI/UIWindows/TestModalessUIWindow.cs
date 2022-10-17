using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace ClientTemplate
{
    public class TestModalessUIWindow : UIWindow
    {
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
