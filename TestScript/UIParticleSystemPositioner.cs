using UnityEngine;

public class UIParticleSystemPositioner : MonoBehaviour
{
    public RectTransform uiElement;
    public ParticleSystem particleSystemPrefab;
    public Camera uiCamera;
    public bool useWorldSpaceCanvas = false; // Chọn chế độ hiển thị Canvas

    private ParticleSystem instantiatedParticleSystem;

    private void Start()
    {
        if (!useWorldSpaceCanvas)
        {
            // Nếu không sử dụng World Space Canvas, khởi tạo Particle System từ prefab
            instantiatedParticleSystem = Instantiate(particleSystemPrefab, transform);
        }
        else
        {
            // Nếu sử dụng World Space Canvas, đặt Particle System làm con của UI element
            instantiatedParticleSystem = Instantiate(particleSystemPrefab, uiElement);
            instantiatedParticleSystem.transform.localPosition = Vector3.zero;
            instantiatedParticleSystem.transform.localScale = Vector3.one;
        }
    }

    private void Update()
    {
        if (!useWorldSpaceCanvas)
        {
            // Lấy vị trí của UI element trên màn hình
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, uiElement.position);

            // Chuyển đổi vị trí màn hình thành tọa độ thế giới 3D
            Vector3 worldPos;
            Ray ray = uiCamera.ScreenPointToRay(screenPos);
            Plane plane = new Plane(Vector3.forward, Vector3.zero); // Mặt phẳng XY
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                worldPos = ray.GetPoint(distance);
            }
            else
            {
                worldPos = uiElement.position; 
            }

            // Đặt vị trí của Particle System đã khởi tạo
            instantiatedParticleSystem.transform.position = worldPos;
        }
        // Nếu sử dụng World Space Canvas, không cần cập nhật vị trí vì Particle System đã là con của UI element
    }
}