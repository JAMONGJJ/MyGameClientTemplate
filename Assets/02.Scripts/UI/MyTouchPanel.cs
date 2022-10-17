using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClientTemplate
{
    public class MyTouchPanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private List<PointerEventData> _eventDatas = new List<PointerEventData>();
        private PointerEventData _firstEventData;
        private PointerEventData _secondEventData;
        private float _lastDistance = 0.0f;

        public void OnPointerDown(PointerEventData eventData)
        {
            _eventDatas.Add(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _eventDatas.Remove(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
            {
                RotateCamera(eventData);
            }
            else if (eventData.clickCount == 2)
            {
                Zoom();
            }
        }

        private void RotateCamera(PointerEventData eventData)
        {
            GamePlayManager.Instance.MyCameraController.RotateMainCamera(eventData.delta);
        }

        private void Zoom()
        {
            _firstEventData = _eventDatas[0];
            _secondEventData = _eventDatas[1];
            float distanceBetweenInputData = Math.Abs(_firstEventData.position.x - _secondEventData.position.x) +
                                             Math.Abs(_firstEventData.position.y - _secondEventData.position.y);
            if (distanceBetweenInputData > _lastDistance)
            {
                GamePlayManager.Instance.MyCameraController.ZoomIn();
            }
            else
            {
                GamePlayManager.Instance.MyCameraController.ZoomOut();
            }

            _lastDistance = distanceBetweenInputData;
        }
    }
}
