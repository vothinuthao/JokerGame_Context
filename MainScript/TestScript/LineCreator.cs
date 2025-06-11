using UnityEngine;
using UnityEngine.UI;

public class LineCreator : MonoBehaviour
{
    public RectTransform startPoint; // Điểm bắt đầu của đường line
    public RectTransform endPoint;   // Điểm kết thúc của đường line
    public Image lineImage;          // Image sẽ được dùng để hiển thị đường line

    void Start()
    {
        // Tính toán vị trí, xoay và scale của đường line
        UpdateLine();
    }

    void UpdateLine()
    {
        // Kiểm tra xem các RectTransform có được gán chưa
        if (startPoint == null || endPoint == null || lineImage == null)
        {
            Debug.LogError("Các RectTransform hoặc Image chưa được gán!");
            return;
        }

        // Tính toán vector hướng từ điểm bắt đầu đến điểm kết thúc
        Vector2 direction = endPoint.anchoredPosition - startPoint.anchoredPosition;

        // Tính toán độ dài của đường line
        float distance = direction.magnitude;

        // Đặt vị trí của lineImage ở giữa hai điểm
        lineImage.rectTransform.anchoredPosition = startPoint.anchoredPosition + direction / 2f;

        // Xoay lineImage để nó hướng theo vector direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineImage.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);

        // Điều chỉnh kích thước của lineImage để nó có độ dài bằng distance
        lineImage.rectTransform.sizeDelta = new Vector2(distance, lineImage.rectTransform.sizeDelta.y);
    }
}