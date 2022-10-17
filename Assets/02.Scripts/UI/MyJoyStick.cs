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
        private float _joyStickWidth;
        private float _joyStickHeight;
        private float _joyStickRadius;
        private Vector3 _initialInputPosition;
        private Vector3 _currentHandlePosition;
        private PointerEventData _currentEventData;

        private void Start()
        {
            _joyStickWidth = GetComponent<RectTransform>().sizeDelta.x;
            _joyStickHeight = GetComponent<RectTransform>().sizeDelta.y;
            _joyStickRadius = _joyStickHeight * 0.5f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GamePlayManager.Instance.MyPlayerController.SetAnimatorTrigger("Run");
            _currentEventData = eventData;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            myHandle.anchoredPosition = Vector3.zero;
            _currentEventData = null;
            GamePlayManager.Instance.MyPlayerController.SetAnimatorTrigger("Idle");
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _currentEventData = eventData;
        }

        private void FixedUpdate()
        {
            if (_currentEventData != null)
            {
                Vector3 handleDirectionVector = new Vector3(_currentEventData.position.x - myRect.anchoredPosition.x,
                    _currentEventData.position.y - myRect.anchoredPosition.y, 0);
                double radius = Vector3.Magnitude(handleDirectionVector);
                if (radius > Math.Sqrt(Math.Pow(_joyStickRadius, 2)))
                {
                    handleDirectionVector = Vector3.Normalize(handleDirectionVector);
                    handleDirectionVector *= _joyStickRadius;
                }
                _currentHandlePosition = new Vector3(handleDirectionVector.x, handleDirectionVector.y, 0.0f);
                myHandle.anchoredPosition = _currentHandlePosition;
                GamePlayManager.Instance.MyPlayerController.SetPlayerTransform(Vector3.Normalize(_currentHandlePosition), radius / _joyStickRadius);
            }
        }
    }
}
