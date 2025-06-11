using UnityEngine;

namespace TestScript
{
    public class BaseEffect : MonoBehaviour
    {
        public ParticleSystem effectPrefab; // Prefab của Particle System
        public Camera effectCamera; // Camera để render hiệu ứng
        public RectTransform rectPosition;
        public void PlayEffect()
        {
            // Khởi tạo Particle System từ prefab
            ParticleSystem effectInstance = Instantiate(effectPrefab, transform);

            // Chuyển đổi vị trí UI sang tọa độ thế giới của EffectCamera
            Vector3 uiScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rectPosition.position);
            Vector3 effectWorldPos = effectCamera.ScreenToWorldPoint(uiScreenPos);
            effectWorldPos.z = 10; // Đảm bảo hiệu ứng nằm trên cùng một mặt phẳng Z với UI

            // Đặt vị trí của Particle System
            effectInstance.transform.position = effectWorldPos;

            // Tự động hủy Particle System khi hiệu ứng kết thúc (nếu cần)
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }
}