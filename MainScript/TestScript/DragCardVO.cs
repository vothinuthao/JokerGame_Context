using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestScript
{
    public class DragCardVo : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Canvas canvas;
        public GraphicRaycaster graphicRaycaster;
        public Image imageComponent;
        public RectTransform parentCard;

        [Header("States")]
        public bool isDragging;
        [HideInInspector] public bool wasDragged;
        
        private Vector3 offset;

        [Header("Movement")]
        [SerializeField] private float moveSpeedLimit = 50;
        
        [Header("Rotation Parameters")]
        [SerializeField] private float rotationAmount = 20;
        [SerializeField] private float rotationSpeed = 20;
        [SerializeField] private float autoTiltAmount = 30;
        [SerializeField] private float manualTiltAmount = 20;
        [SerializeField] private float tiltSpeed = 20;
        private Vector3 rotationDelta;
        Vector3 movementDelta;
        private Camera _camera;
        [HideInInspector] public UnityEvent<GameObject> BeginDragEvent;
        [HideInInspector] public UnityEvent<GameObject> EndDragEvent;

        void Start()
        {
            _camera = Camera.main;
        }
        private void Update()
        {
            Vector2 screenBounds = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _camera.transform.position.z));
            Vector3 clampedPosition = new Vector3(transform.position.x ,transform.position.y , 0);
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
            // objCardToDrag.transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
            if (isDragging)
            {
                Vector2 targetPosition = _camera.ScreenToWorldPoint(Input.mousePosition) - offset;
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
                transform.Translate(velocity * Time.deltaTime);
                FollowRotation();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDragEvent.Invoke(this.gameObject);
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)transform.position;
            isDragging = true;
            graphicRaycaster.enabled = false;
            imageComponent.raycastTarget = false;
            
            wasDragged = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDragEvent.Invoke(this.gameObject);
            isDragging = false;
            graphicRaycaster.enabled = true;
            imageComponent.raycastTarget = true;
            
            StartCoroutine(FrameWait());
            
            IEnumerator FrameWait()
            {
                yield return new WaitForEndOfFrame();
                wasDragged = false;
            }
        }
        
        
        private void FollowRotation()
        {
            Vector3 movement = (transform.position - parentCard.position);
            movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
            Vector3 movementRotation = (isDragging ? movementDelta : movement) * rotationAmount;
            rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
        }
    }
}
