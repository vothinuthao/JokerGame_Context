using System.Collections;
using Manager;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Visual.Cards
{
    public class CardVisual : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        private Canvas canvas;
        private Image imageComponent;
        [SerializeField] private bool instantiateVisual = true;
        private Vector3 offset;
        private RectTransform _rectTransform;
        [Header("Movement")]
        [SerializeField] private float moveSpeedLimit = 50;
        private Vector3 rotationDelta;
        [Header("Follow Parameters")]
        [SerializeField] private float followSpeed = 30;
        
        [Header("Rotation Parameters")]
        private float rotationAmount = 20;
        private float rotationSpeed = 20;
        private float autoTiltAmount = 30;
        private float manualTiltAmount = 20;
        private float tiltSpeed = 20;
        [Header("Selection")]
        public bool selected;
        public float selectionOffset = 50;
        private float pointerDownTime;
        private float pointerUpTime;
        [Header("States")]
        public bool isDragging;
        [HideInInspector] public bool wasDragged;
        [Header("Events")]
        [HideInInspector] public UnityEvent<CardVisual, bool> PointerUpEvent;
        [HideInInspector] public UnityEvent<CardVisual> PointerDownEvent;
        [HideInInspector] public UnityEvent<CardVisual> BeginDragEvent;
        [HideInInspector] public UnityEvent<CardVisual> EndDragEvent;
        [HideInInspector] public UnityEvent<CardVisual, bool> SelectEvent;
        
        [SerializeField] private MMF_Player mmPlayerEffectSelect;
        [SerializeField] private MMF_Player mmPlayerEffectUnSelect;
        [SerializeField]
        private GameObject objHighLight;
        
        private Camera _camera;
        private float curveYOffset;
        Vector3 movementDelta;
        void Start()
        {
            _camera = Camera.main;
            canvas = GetComponentInParent<Canvas>();
            imageComponent = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
        }
        void Update()
        {
            if (isDragging)
            {
                Vector2 targetPosition = _camera.ScreenToWorldPoint(Input.mousePosition) - (offset * 1.1f);
                Vector2 direction = (targetPosition - (Vector2)_rectTransform.position).normalized;
                Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(_rectTransform.position, targetPosition) / Time.deltaTime);
                _rectTransform.Translate(new Vector2(velocity.x,0) * Time.deltaTime);
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(StateMachineController.Instance.CurrentState != GameState.PlayerTurn) return;
            
            BeginDragEvent.Invoke(this);
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)_rectTransform.position;
            isDragging = true;
            canvas.GetComponent<GraphicRaycaster>().enabled = false;
            imageComponent.raycastTarget = false;
            wasDragged = true;
            OnPlayEffectSelectCard();
        }
        public void OnDrag(PointerEventData eventData)
        {
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public void OnEndDrag(PointerEventData eventData)
        {
            EndDragEvent.Invoke(this);
            isDragging = false;
            canvas.GetComponent<GraphicRaycaster>().enabled = true;
            imageComponent.raycastTarget = true;

            StartCoroutine(FrameWait());
            
            IEnumerator FrameWait()
            {
                yield return new WaitForEndOfFrame();
                wasDragged = false;
                OnPlayEffectUnSelectCard();
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            PointerDownEvent.Invoke(this);
            pointerDownTime = Time.time;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            pointerUpTime = Time.time;

            PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

            if (pointerUpTime - pointerDownTime > .2f)
                return;

            if (wasDragged)
                return;

            selected = !selected;
            SelectEvent.Invoke(this, selected);
        }
        public int ParentIndex()
        {
            return _rectTransform.parent.CompareTag("Slot") ? _rectTransform.parent.GetSiblingIndex() : 0;
        }
        private void OnPlayEffectSelectCard()
        {
            if(!mmPlayerEffectSelect) return;
            mmPlayerEffectSelect.PlayFeedbacks();
            objHighLight.SetActive(true);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private void OnPlayEffectUnSelectCard()
        {
            if(!mmPlayerEffectUnSelect) return;
            mmPlayerEffectUnSelect.PlayFeedbacks();
            objHighLight.SetActive(false);
        }
    }
}
