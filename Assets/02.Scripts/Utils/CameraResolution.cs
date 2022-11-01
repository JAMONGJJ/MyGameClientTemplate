using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    public class CameraResolution : MonoBehaviour
    {
        [SerializeField] private float _widthRatio = 9.0f;
        [SerializeField] private float _heightRatio = 16.0f;
            
        private void Start()
        {
            Camera mainCamera = Camera.main;
            Rect rect = mainCamera.rect;
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

            mainCamera.rect = rect;
        }

        private void OnPreCull() => GL.Clear(true, true, Color.black);
    }
}