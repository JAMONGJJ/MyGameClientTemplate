using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientTemplate
{
    namespace UIRegion
    {
        using UIRegion.UIInfo;

        public class TestModalessUIWindow2 : UIWindow
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
                ExitButton.onClick.AddListener(() =>
                {
                    UIManager.Instance.CloseWindow(UIWindowType.TestModalessUIWindow2);
                });
            }
        }
    }
}
