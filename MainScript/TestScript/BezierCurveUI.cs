using UnityEngine;
using UnityEngine.UI;

public class BezierCurveUI : MonoBehaviour
{
    public RectTransform startPoint;         // RectTransform của điểm bắt đầu
    public RectTransform arrowImage;        // RectTransform của hình ảnh mũi tên
    public Camera uiCamera;                 // Camera của UI

    public LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer.positionCount = 2; // Chỉ cần 2 điểm: bắt đầu và kết thúc
        uiCamera = GetComponentInParent<Canvas>().worldCamera;
    }

    void Update()
    {
        if (startPoint != null && arrowImage != null && uiCamera != null)
        {
            // Lấy tọa độ thế giới của điểm bắt đầu và mũi tên
            Vector3 startPos = startPoint.position;
            Vector3 endPos = arrowImage.position;

            // Đặt vị trí cho Line Renderer
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}