using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class TargetingArrowUI : MonoBehaviour
{
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private Image arrowNodePrefab;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private int nodeCount = 10;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private float nodeSpacing = 10f;
    [FoldoutGroup("Setting Node ")]
    [SerializeField] private float arrowHeight = 50f;

    [FoldoutGroup("Setting Canvas")]
    [SerializeField] private Canvas canvas; 
    [FoldoutGroup("Setting Canvas")]
    [SerializeField] private Vector2 arrowOrigin;

    public float minNodeScale = 0.5f; 
    public float maxNodeScale = 1.5f;
    public Image arrowTipPrefab;

    private Camera uiCamera;
    private RectTransform canvasRectTransform;
    private List<Image> arrowNodes = new List<Image>();
    private Image arrowTip;

    void Start()
    {
        uiCamera = canvas.worldCamera;
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        for (int i = 0; i < nodeCount - 1; i++)
        {
            Image node = Instantiate(arrowNodePrefab, transform);
            arrowNodes.Add(node);
        }

        arrowTip = Instantiate(arrowTipPrefab, transform);
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, mousePos, uiCamera, out Vector2 mousePosInCanvas);

        Vector2 startPoint = arrowOrigin;
        Vector2 endPoint = mousePosInCanvas;

        float distance = Vector2.Distance(startPoint, endPoint);
        float scaleFactor = Mathf.Lerp(minNodeScale, maxNodeScale, distance / 1000f); // Điều chỉnh 1000f tùy ý

        Vector2[] points = new Vector2[3];
        points[0] = startPoint;
        points[2] = endPoint;
        points[1] = (points[0] + points[2]) / 2 + Vector2.up * arrowHeight;

        for (int i = 0; i < nodeCount - 1; i++)
        {
            float t = (float)i / (nodeCount - 1);
            arrowNodes[i].rectTransform.anchoredPosition = CalculateBezierPoint(t, points);
            arrowNodes[i].rectTransform.localScale = Vector3.one * scaleFactor;

            if (i < nodeCount - 2)
            {
                Vector2 direction = arrowNodes[i + 1].rectTransform.anchoredPosition - arrowNodes[i].rectTransform.anchoredPosition;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                arrowNodes[i].rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        arrowTip.rectTransform.anchoredPosition = endPoint;
        Vector2 arrowTipDirection = endPoint - points[1]; // Hướng từ điểm điều khiển đến điểm cuối
        float arrowTipAngle = Mathf.Atan2(arrowTipDirection.y, arrowTipDirection.x) * Mathf.Rad2Deg;
        arrowTip.rectTransform.rotation = Quaternion.Euler(0f, 0f, arrowTipAngle);
        Vector2 arrowBodyDirection = endPoint - startPoint;
        float arrowBodyAngle = Mathf.Atan2(arrowBodyDirection.y, arrowBodyDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, arrowBodyAngle);
    }

    Vector2 CalculateBezierPoint(float t, Vector2[] points)
    {
        return (1 - t) * (1 - t) * points[0] + 2 * (1 - t) * t * points[1] + t * t * points[2];
    }
}
