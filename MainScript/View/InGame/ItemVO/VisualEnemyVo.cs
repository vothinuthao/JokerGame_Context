using System;
using Lean.Touch;
using Manager;
using Runtime.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGame
{
   
    public class VisualEnemyVo : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        
        [SerializeField] private RectTransform startPoint;
        [SerializeField] private RectTransform endPoint;

        private RectTransform _enemy;
        private Action<int> _callback;
        private int _idZone;
        private LeanFinger _currentFinger;
        private RectTransform _enemySelfZone;
        private Camera _camera;
        public float arrowHeadLength = 0.5f; // Chiều dài đầu mũi tên
        public float arrowHeadAngle = 20f;
        public void Start()
        {
            _camera = Camera.main;
            // highLight.SetActive(false);
            lineRenderer.positionCount = 5;
        }
        private void OnEnable()
        {
            LeanTouch.OnFingerDown += HandleFingerDown;
            LeanTouch.OnFingerUp += HandleFingerUp;
            LeanTouch.OnFingerUpdate += HandleFingerUpdate;
        }
        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= HandleFingerDown;
            LeanTouch.OnFingerUp -= HandleFingerUp;
            LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        }

        public void SetData(int id,GameObject enemy, Action<int> callBackID)
        {
            lineRenderer.enabled = false;
            _callback = callBackID;
            _enemySelfZone = gameObject.AsRectTransform();
            _idZone = id;
            endPoint = enemy.gameObject.AsRectTransform();
            
        }
        
        private void HandleFingerUpdate(LeanFinger finger)
        {
            if (TouchManager.IsInstanceValid() && TouchManager.Instance.CurrentFinger != null)
            {
                if (_camera)
                {
                    float distanceFromCamera = -_camera.transform.position.z;
                    Vector3 uiPosition = _camera.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, distanceFromCamera));
                    bool checkSidePlay = RectTransformUtility.RectangleContainsScreenPoint(this.transform as RectTransform, uiPosition);
                    if (checkSidePlay)
                    {

                        if (PlayerDataManager.Instance.Property.IsCompletedTutorial ||
                            PlayerDataManager.Instance.Property.CurrentIdTutorial >= 10)
                        {
                            lineRenderer.enabled = true;
                            CreatorLineLink();
                        }
                        for (var i = 0; i < EnemyController.Instance.ListEnemyScript.Count; i++)
                        {
                            var sc = EnemyController.Instance.ListEnemyScript[i];
                            if (i == _idZone)
                                sc.OnSelectData(_idZone);
                            else
                                sc.DeSelectData();
                        }
                       
                    }
                    else
                    {
                        lineRenderer.enabled = false;
                    }
                }
            }
        }
        private void HandleFingerDown(LeanFinger finger)
        {

        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void HandleFingerUp(LeanFinger finger)
        {
            float distanceFromCamera = -_camera.transform.position.z;
            Vector3 uiPosition = _camera.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, distanceFromCamera));
            bool checkSidePlay = RectTransformUtility.RectangleContainsScreenPoint(_enemySelfZone, uiPosition);
            if (checkSidePlay)
            {
                if (TouchManager.IsInstanceValid() && TouchManager.Instance.IsPlayerTouch && TouchManager.Instance.CanPlay)
                {
                    if (PlayerDataManager.Instance.Property.IsCompletedTutorial || PlayerDataManager.Instance.Property.CurrentIdTutorial >= 10)
                    {
                        _callback?.Invoke(_idZone);
                        lineRenderer.enabled = false;
                    }
                }
                TouchManager.Instance.SetPlayerTouch(false);
            }
        }
        void CreatorLineLink()
        {
            if (startPoint)
            {
                if (!lineRenderer || !startPoint || !endPoint) return;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPoint.position);
                lineRenderer.SetPosition(1, endPoint.position);
            }
        }
    }
}
