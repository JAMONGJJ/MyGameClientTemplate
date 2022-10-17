using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ClientTemplate
{
    public class MyPlayerController : MonoBehaviour
    {
        public Animator playerAnimator;
        private float _maxSpeed = 50.0f;
        private float _elapsedTime = 0.0f;
        private Camera _mainCamera;

        private void Start()
        {
            gameObject.tag = "MyPlayer";
            _mainCamera = Camera.main;
        }

        public void SetPlayerTransform(Vector3 characterTransform, double radius)
        {
            Vector3 cameraDirection = _mainCamera.transform.forward;
            cameraDirection.y = 0;
            float cameraAngle = (float)(Math.Atan2(cameraDirection.z, cameraDirection.x) * Mathf.Rad2Deg);
            
            Vector3 direction = new Vector3(characterTransform.x, 0, characterTransform.y);
            direction = Quaternion.AngleAxis(cameraAngle - 90, Vector3.down) * direction;
            float speed = math.lerp(0, _maxSpeed, (float)(Math.Min(radius, 1)));
            transform.Translate(speed * Time.deltaTime * direction, Space.World);
            float angle = (float)(Math.Atan2(characterTransform.y, characterTransform.x) * Mathf.Rad2Deg);
            transform.localRotation = Quaternion.AngleAxis((cameraAngle - 90) + (angle - 90), Vector3.down);
        }

        public void SetAnimatorTrigger(string trigger)
        {
            _elapsedTime = 0.0f;
            playerAnimator.SetTrigger(trigger);
        }

        public void SetAnimatorBool(int id, bool state)
        {
            _elapsedTime = 0.0f;
            playerAnimator.SetBool(id, state);
        }
    }
}

