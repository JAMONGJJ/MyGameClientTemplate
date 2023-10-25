using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClientTemplate
{
    public class MyJoyStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public RectTransform myRect;
        public RectTransform myHandle;
        private float joyStickWidth;
        private float joyStickHeight;
        private float joyStickRadius;
        private Vector3 initialInputPosition;
        private Vector3 currentHandlePosition;
        private PointerEventData CurrentEventData;

        private void Start()
        {
            joyStickWidth = myRect.sizeDelta.x;
            joyStickHeight = myRect.sizeDelta.y;
            joyStickRadius = joyStickHeight * 0.5f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
            CurrentEventData = eventData;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            myHandle.anchoredPosition = Vector3.zero;
            CurrentEventData = null;
            
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            CurrentEventData = eventData;
        }

        private void FixedUpdate()
        {
            if (CurrentEventData != null)
            {
                Vector3 handleDirectionVector = new Vector3(CurrentEventData.position.x - myRect.anchoredPosition.x,
                    CurrentEventData.position.y - myRect.anchoredPosition.y, 0);
                double radius = Vector3.Magnitude(handleDirectionVector);
                if (radius > Math.Sqrt(Math.Pow(joyStickRadius, 2)))
                {
                    handleDirectionVector = Vector3.Normalize(handleDirectionVector);
                    handleDirectionVector *= joyStickRadius;
                }
                currentHandlePosition = new Vector3(handleDirectionVector.x, handleDirectionVector.y, 0.0f);
                myHandle.anchoredPosition = currentHandlePosition;
                GamePlayManager.Instance.MyPlayerController.SetPlayerTransform(Vector3.Normalize(currentHandlePosition), radius / joyStickRadius);
            }
        }
    }
}
