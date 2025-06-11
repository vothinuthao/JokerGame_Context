using System.Collections.Generic;
using Frameworks.Utils;
using Lean.Touch;
using Runtime.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Manager
{
    public class TouchManager : ManualSingletonMono<TouchManager>
    {
        [SerializeField]
        private TargetingArrowUIController targetingScript;
        
        [SerializeField]
        private RectTransform  handPokerZoneRectTransform;
        [SerializeField]
        private RectTransform  playZoneRectTransform;
        [SerializeField]
        private RectTransform  discardZoneRectTransform;

        [SerializeField] private GameObject objDiscardVisual;
        [SerializeField]
        private UnityEvent onCallActionPlay;
        [SerializeField]
        private UnityEvent onCallActionDiscard;
        [SerializeField]
        private UnityEvent<bool> onCallActionUpdate;

        private LeanFinger _currentFinger;
        private Camera _camera;
        private bool _isCanPlay = true;
        private bool _isCanTouch = true;
        private bool _isPlayerTouch = false;
        public bool CanPlay => _isCanPlay;
        public LeanFinger CurrentFinger => _currentFinger;
        public bool IsPlayerTouch => _isPlayerTouch;
        public override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
            objDiscardVisual.SetActive(false);
            _currentFinger = null;
            _isCanTouch = true;
        }
        public void SetCanTouch(bool isTouch)
        {
            _isCanTouch = isTouch;
        }

        public void SetPlayerTouch(bool isTouch)
        {
            _isPlayerTouch = isTouch;
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
        private void HandleFingerDown(LeanFinger finger)
        {
            float distanceFromCamera = -_camera.transform.position.z;
            Vector3 uiPosition = _camera.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, distanceFromCamera));
            bool checkSidePlay = RectTransformUtility.RectangleContainsScreenPoint(handPokerZoneRectTransform, uiPosition);
            if (checkSidePlay)
            {
                _currentFinger = finger;
                // _isPlayerTouch = true;
            }
        }
        private void HandleFingerUp(LeanFinger finger)
        {
            float distanceFromCamera = -_camera.transform.position.z;
            Vector3 uiPosition = _camera.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, distanceFromCamera));
            bool checkSidePlay = RectTransformUtility.RectangleContainsScreenPoint(handPokerZoneRectTransform, uiPosition);
            bool checkSideOnPlayZone = RectTransformUtility.RectangleContainsScreenPoint(playZoneRectTransform, uiPosition);
            bool checkSideDiscard = RectTransformUtility.RectangleContainsScreenPoint(discardZoneRectTransform, uiPosition);
            if (finger == _currentFinger)
            {
                if (!checkSidePlay && !checkSideDiscard && checkSideOnPlayZone && _isCanTouch)
                { 
                    _isPlayerTouch = true;
                   _isCanPlay = true;
                   objDiscardVisual.SetActive(false);
                }
                else if (checkSideDiscard && !checkSidePlay && !checkSideOnPlayZone && _isCanTouch)
                {
                    if (StateMachineController.Instance.CurrentState == GameState.PlayerTurn)
                    {
                        if (PlayerDataManager.Instance.Property.CurrentIdTutorial != 10)
                        {
                            onCallActionDiscard?.Invoke();
                        }
                            
                    }
                   
                }
                targetingScript.OnDisableObject();
                _currentFinger = null;
                objDiscardVisual.SetActive(false);
                EnemyController.Instance.UnSelectEnemyScript();
            }
        }
        private void HandleFingerUpdate(LeanFinger finger)
        {
            float distanceFromCamera = Vector3.Distance(_camera.transform.position, finger.ScreenPosition); 
            Vector3 clampedFingerPosition = finger.ScreenPosition;
            clampedFingerPosition.x = Mathf.Clamp(clampedFingerPosition.x, 0, Screen.width);
            clampedFingerPosition.y = Mathf.Clamp(clampedFingerPosition.y, 0, Screen.height);
            Vector3 uiPosition = _camera.ScreenToWorldPoint(new Vector3(
                clampedFingerPosition.x, 
                clampedFingerPosition.y, 
                distanceFromCamera));
            bool checkSidePlay =
                RectTransformUtility.RectangleContainsScreenPoint(handPokerZoneRectTransform, uiPosition);
            bool checkSideDiscard =
                RectTransformUtility.RectangleContainsScreenPoint(discardZoneRectTransform, uiPosition);
            bool checkSideOnPlayZone =
                RectTransformUtility.RectangleContainsScreenPoint(playZoneRectTransform, uiPosition);
          

            if (checkSideDiscard && _currentFinger != null && PlayCardController.Instance.ListCardOnSelect.Count != 0 && _isCanTouch)
            {
                objDiscardVisual.SetActive(true);
                onCallActionUpdate?.Invoke(false);
            }
            // else
            // {
            //     objDiscardVisual.SetActive(false);
            //    
            // }
            if (!checkSidePlay && !checkSideDiscard && checkSideOnPlayZone && _currentFinger != null && _isCanTouch)
            {
                objDiscardVisual.SetActive(true);
                onCallActionUpdate?.Invoke(PlayCardController.Instance.ListCardOnSelect.Count != 0);
            }
            else
            {
                onCallActionUpdate?.Invoke(false);
                targetingScript.OnDisableObject();
            }

        }
        
        }
}