using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.Mathematics;
using System.Linq;

namespace KlayLand
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject _cameraCollideObject;
        private Camera _mainCamera;
        private GameObject _playerObject;
        private Vector3 _characterHeightPivot;
        private float _distanceBetweenCameraAndCollder;

        private readonly float _minRotate = -30.0f;
        private readonly float _maxRotate = 60.0f;
        private readonly float _dragSpeed = 0.8f;
        private readonly float _minZoomInDistance = 10.0f;
        private readonly float _maxZoomOutDistance = 40.0f;
        private readonly float _zoomInSpeed = 10.0f;
        private readonly float _zoomOutSpeed = 10.0f;

        // Start is called before the first frame update
        private void Start()
        {
            _mainCamera = Camera.main;
            _playerObject = GameObject.FindWithTag("Player");
            _characterHeightPivot = new Vector3(0.0f, 7.0f, 0.0f);
            transform.rotation = Quaternion.Euler(new Vector3(30.0f, 0.0f, 0.0f));
            _distanceBetweenCameraAndCollder =
                Vector3.Magnitude(transform.position - _cameraCollideObject.transform.position);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            UpdatePosition();
            TestRayToColliderObject();
        }

        private void UpdatePosition()
        {
            transform.position = _playerObject.transform.position + _characterHeightPivot;
        }
        
        /// <summary>
        /// transform.eulerAngles가 0 ~ 360 범위의 값만 갖기 때문에 계산이 필요함.
        /// 회전값 범위 => -30 ~ 60
        /// </summary>
        /// <param name="dragAmount"></param>
        public void RotateMainCamera(Vector2 dragAmount)
        {
            Vector3 rot = transform.eulerAngles;
            if (rot.x > _maxRotate * 2)
            {
                rot.x = Math.Max(rot.x, 360 + _minRotate);
                if (rot.x >= 360 + _minRotate)
                {
                    rot.x -= 360;
                }
            }

            rot.x = Math.Min(Math.Max(rot.x - dragAmount.y * _dragSpeed, _minRotate), _maxRotate);
            rot.y += dragAmount.x * _dragSpeed;
            transform.localRotation = Quaternion.Euler(new Vector3(rot.x, rot.y, 0));
        }

        public void ZoomIn()
        {
            Vector3 pos = _cameraCollideObject.transform.localPosition;
            pos.z += _zoomInSpeed * Time.deltaTime;
            pos.z = Math.Min(pos.z, -_minZoomInDistance);
            _cameraCollideObject.transform.localPosition = pos;
        }

        public void NormalZoom()
        {
            Vector3 pos = _cameraCollideObject.transform.localPosition;
            pos.z = -_distanceBetweenCameraAndCollder;
            _cameraCollideObject.transform.localPosition = pos;
        }

        public void ZoomOut()
        {
            Vector3 pos = _cameraCollideObject.transform.localPosition;
            pos.z -= _zoomOutSpeed * Time.deltaTime;
            pos.z = Math.Max(pos.z, -_maxZoomOutDistance);
            _cameraCollideObject.transform.localPosition = pos;
        }

        private void TestRayToColliderObject()
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, _cameraCollideObject.transform.position - transform.position,
                    Math.Abs(_cameraCollideObject.transform.localPosition.z))
                .OrderBy(hit => hit.distance).ToArray();
            if (hits.Length != 0)
            {
                _mainCamera.transform.position = hits[0].point;
            }
        }
    }
}
