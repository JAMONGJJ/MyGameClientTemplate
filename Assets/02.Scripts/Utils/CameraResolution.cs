using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClientTemplate
{
    public class CameraResolution : MonoBehaviour
    {
        [SerializeField] private float _widthRatio = 9.0f;
        [SerializeField] private float _heightRatio = 16.0f;
        [SerializeField] private Camera MyCamera;
            
        private void Start()
        {
            if (MyCamera == null)
            {
                MyCamera = Camera.main;
            }
            
            if (MyCamera == null)
            {
                return;
            }

            Rect rect = MyCamera.rect;
            float scaleHeight = ((float)Screen.width / Screen.height) / (_widthRatio / _heightRatio);
            float scaleWidth = 1f / scaleHeight;
            if (scaleHeight < 1)
            {
                rect.height = scaleHeight;
                rect.y = (1f - scaleHeight) / 2f;
            }
            else
            {
                rect.width = scaleWidth;
                rect.x = (1f - scaleWidth) / 2f;
            }

            MyCamera.rect = rect;
        }
    }
}
