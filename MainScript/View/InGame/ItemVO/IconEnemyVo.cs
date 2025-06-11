using System;
using Core.Utils;
using DG.Tweening;
using Frameworks.Utils;
using Lean.Touch;
using Manager;
using Runtime.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame
{
    public class IconEnemyVo : MonoBehaviour
    {
        [SerializeField] private Image iconEnemyTarget;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private RectTransform startPoint;
        [SerializeField] private RectTransform endPoint;
        
        [FoldoutGroup("Visual Card")]
        [SerializeField] private GameObject objPrefabFake;
        [FoldoutGroup("Visual Card")]
        [SerializeField] private Transform transParentFake;
        private Camera _camera;
        private EnemyCardVo _data;

        private void Start()
        {
            _camera = Camera.main;
            lineRenderer.positionCount = 2;
        }
        private void OnEnable()
        {
            LeanTouch.OnFingerUp += HandleFingerUp;
            LeanTouch.OnFingerUpdate += HandleFingerUpdate;
        }
        private void OnDisable()
        {
            LeanTouch.OnFingerUp -= HandleFingerUp;
            LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        }
        public void SetData(EnemyCardVo data)
        {
            _data = data;
            lineRenderer.enabled = false;
            iconEnemyTarget.sprite = _data.EnemySO.imgEnemy;
            GameObject fakeE = GameObjectUtils.Instance.SpawnGameObject(transParentFake, objPrefabFake);
            fakeE.transform.position = _data.transform.position;
            RectTransform dataRectTransform = fakeE.transform as RectTransform;
            if (dataRectTransform != null)
            {
                Vector2 bottomCenterPosition = new Vector2(
                    dataRectTransform.anchoredPosition.x,
                    dataRectTransform.anchoredPosition.y - dataRectTransform.rect.height / 10f
                );
                endPoint = fakeE.transform as RectTransform;
                if (endPoint != null) endPoint.anchoredPosition = bottomCenterPosition;
            }
        }

        void CreatorLineLink()
        {
            if (startPoint)
            {
                Vector3 startPosition = startPoint.position;
                Vector3 endPosition = new Vector3(endPoint.position.x , endPoint.position.y / 1.2f, endPoint.position.z);
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, endPosition);
            }
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
                        if (this.gameObject.activeSelf)
                        {
                            EnemyController.Instance.IdEnemySelected = _data.GetIndexEnemy;
                            _data.OnSelectData(_data.GetIndexEnemy);
                            lineRenderer.enabled = true;
                            CreatorLineLink();
                        }
                    }
                    else
                    {
                        lineRenderer.enabled = false;
                        _data.DeSelectData();
                    }
                }
            }
        }
        private void HandleFingerUp(LeanFinger finger)
        {
            _camera ??= Camera.main;
            if (_camera)
            {
                float distanceFromCamera = -_camera.transform.position.z;
                Vector3 uiPosition = _camera.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, distanceFromCamera));
                bool checkSidePlay = RectTransformUtility.RectangleContainsScreenPoint(this.transform as RectTransform, uiPosition);
                if (checkSidePlay)
                {
                    EnemyController.Instance.IdEnemySelected = _data.GetIndexEnemy;
                    _data.OnSelectData(_data.GetIndexEnemy);
                    lineRenderer.enabled = false;
                }
            }
        }
    }
}
