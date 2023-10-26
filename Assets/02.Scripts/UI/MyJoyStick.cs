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

        private void Start()
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {

            
        }
        
        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}
