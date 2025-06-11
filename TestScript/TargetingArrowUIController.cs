using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Frameworks.Utils;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class TargetingArrowUIController : MonoBehaviour
{
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private Image arrowArrowPrefab;   
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private Image arrowNodePrefab;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private Transform contentSpawn;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private int nodeCount = 5;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private float nodeCurve = 0.5f;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private float multiEndPoint = 0.5f;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private float nodeSpacing = 10f;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private float arrowHeight = 50f;

    [FoldoutGroup("Setting Canvas ")]
    [SerializeField] private Canvas canvas;
    [FormerlySerializedAs("rectTrans")]
    [FoldoutGroup("Setting Canvas ")]
    [SerializeField] private RectTransform startSpawnNode; 
    [FoldoutGroup("Setting Canvas ")]
    [SerializeField] private RectTransform canvasRectTransform;
    
    private Vector2 _arrowOrigin; 
    private Camera _uiCamera;
    private List<Image> _arrowNodes = new List<Image>();
    private bool _isDragCard;
    void Start()
    {
        _uiCamera = canvas.worldCamera;
        _arrowOrigin = startSpawnNode.anchoredPosition;
        arrowArrowPrefab.gameObject.transform.SetActive(false);
        arrowNodePrefab.gameObject.transform.SetActive(false);
        for (int i = 0; i < nodeCount; i++)
        {
            Image node = Instantiate(i == nodeCount - 1 ? arrowArrowPrefab : arrowNodePrefab, contentSpawn);
            _arrowNodes.Add(node);
            
        }
    }
    public void OnListeningAction(bool isDragCard)
    {
        _isDragCard = isDragCard;
        contentSpawn.SetActive(_isDragCard);
    }
    public void OnDisableObject()
    {
        foreach (var node in _arrowNodes)
        {
            node.SetActive(false);
        }
    }
    void Update()
    {
        if (_isDragCard)
        {
            Vector2 mousePos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, mousePos, _uiCamera, out Vector2 mousePosInCanvas);
            Vector2 startPoint = _arrowOrigin;
            Vector2 endPoint = mousePosInCanvas * multiEndPoint;

            Vector2[] points = new Vector2[3];
            points[0] = startPoint;
            points[2] = endPoint;
            points[1] = (points[0] + points[2]) / 2 + Vector2.up * arrowHeight;

            for (int i = 0; i < nodeCount; i++)
            {
                _arrowNodes[i].transform.SetActive(true);
                float t = (float)i / (nodeCount - 1) * nodeCurve;
                _arrowNodes[i].rectTransform.anchoredPosition = CalculateBezierPoint(t, points) * nodeSpacing;

                if (i < nodeCount - 1)
                {
                    Vector2 direction = _arrowNodes[i + 1].rectTransform.anchoredPosition - _arrowNodes[i].rectTransform.anchoredPosition;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    _arrowNodes[i].rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
        }
    }
    Vector2 CalculateBezierPoint(float t, Vector2[] points)
    {
        return (1 - t) * (1 - t) * points[0] + 2 * (1 - t) * t * points[1] + t * t * points[2];
    }
}
