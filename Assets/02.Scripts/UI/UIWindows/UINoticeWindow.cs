using System;
using System.Collections;
using System.Collections.Generic;
using KlayLand.UIInfo;
using UnityEngine;

namespace KlayLand
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

        
        public override void Init()
        {
            throw new NotImplementedException();
        }
        
        public override void Init(UIData data)
        {

        }

        public override void Release()
        {

        }

        public override void OnTop()
        {

        }
    }
}
