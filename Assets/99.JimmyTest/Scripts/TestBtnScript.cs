using System.Collections;
using System.Collections.Generic;
using ClientTemplate.UIInfo;
using UnityEngine;

namespace ClientTemplate
{
    public class TestBtnScript : MonoBehaviour
    {
        public void OnClick_Btn()
        {
            UIManager.Instance.OpenWindow<TestModalessUIWindow>(UIWindowType.TestModalessUIWindow);
        }
        
        public void OnClick_Btn2()
        {
            UIManager.Instance.OpenWindow<UINoticeWindow>(UIWindowType.NoticeWindow);
            TestModalessUIWindow.WindowData data = new TestModalessUIWindow.WindowData();
            data.testText = "Modified!";
            UIManager.Instance.RefreshUIData(UIWindowType.TestModalessUIWindow, data);
        }

        public void OnClick_Btn3()
        {
        }
    }
}
