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
            UIManager.Instance.OpenWindow<UINoticeWindow>(UIWindowType.NoticeWindow);
        }
        public void OnClick_Btn2()
        {
            UIManager.Instance.OpenWindow<TestModalessUIWindow>(UIWindowType.TestModalessUIWindow);
        }
    }
}
