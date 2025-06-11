using System.Collections;
using System.Globalization;
using TMPro;
using UI.BaseColor;
using UnityEngine;
using UnityEngine.Events;

namespace InGame
{
    public class BonusVO : MonoBehaviour
    {
        //[SerializeField] private TextMeshProUGUI txtAmount;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtBonus;

        [SerializeField] private UnityEvent onPlayEffect;
        public void OnInitData(BonusResult data)
        {
           // txtAmount.color = ColorSchemeManager.Instance.GetMainColorByEnum(data.ColorAmount);
            //txtAmount.text = data.Amount.ToString();
            txtDescription.text = $"{data.Description}";
            txtBonus.text = $"{data.Quantity.ToString(CultureInfo.InvariantCulture)}$";
            onPlayEffect?.Invoke();
        }
        
        
    }
}
