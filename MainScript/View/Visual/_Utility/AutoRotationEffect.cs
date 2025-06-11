using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Visual._Utility
{
    public class AutoRotationEffect : MonoBehaviour
    {
        public float rotationDuration = 2f; // Thời gian cho mỗi lần xoay (giây)
        public Vector3 maxRotationAngles = new Vector3(15f, 15f, 15f);
        public Ease ease;

        void Start()
        {
            RotateRandomly();
        }

        void RotateRandomly()
        {
            Vector3 newRotation = new Vector3(
                Random.Range(-maxRotationAngles.x, maxRotationAngles.x),
                Random.Range(-maxRotationAngles.y, maxRotationAngles.y),
                Random.Range(-maxRotationAngles.z, maxRotationAngles.z)
            );

            transform.DORotate(newRotation, rotationDuration)
                .SetEase(ease)
                .OnComplete(RotateRandomly);
        }
    }
}