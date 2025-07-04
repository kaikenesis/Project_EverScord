﻿using UnityEngine;

namespace SciFiArsenal
{
    public class SciFiDragMouseOrbit : MonoBehaviour
    {
        public Transform target;
        public float distance = 5.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;
        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;
        public float distanceMin = .5f;
        public float distanceMax = 15f;
        public float smoothTime = 2f;
        float rotationYAxis = 0.0f;
        float rotationXAxis = 0.0f;
        float velocityX = 0.0f;
        float velocityY = 0.0f;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            rotationYAxis = angles.y;
            rotationXAxis = angles.x;
        }
        void FixedUpdate()
        {
            if (!target)
                return;

            velocityX += xSpeed * distance;
            
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);

            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            distance = Mathf.Clamp(distance, distanceMin, distanceMax);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}