using System;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace InGame
{
    public class TotalDameVo : MonoBehaviour
    {
        [SerializeField]
        private MMF_Player mmfPlayerEffect;
        [SerializeField]
        private TextMeshProUGUI textTotalDame;
        [SerializeField]
        private TextMeshProUGUI textTypeDame;
        [SerializeField]
        private GameObject objPlayerTotalDame;
        
        public void SetDataEffect(RectTransform rect, float dame, bool isBigDame)
        {
            transform.position = isBigDame ? new Vector3(0,0,rect.position.z): rect.position;
            string formattedNumber = $"{dame:0}";
            textTotalDame.text = formattedNumber;
            textTypeDame.text = "DMG: ";
            mmfPlayerEffect.PlayFeedbacks();
         
        }
        public void OnDestroyObject()
        {
            Destroy(this.gameObject);
        }
    }
}
