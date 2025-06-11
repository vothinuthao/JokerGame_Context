using System;
using System.Linq;
using Core.Entity;
using Core.Manager;
using TMPro;
using UnityEngine;

namespace InGame
{
    public class HandTypeVo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textNameHandType;
        [SerializeField] private TextMeshProUGUI textNumberOfHandPlayed;
		[SerializeField] private TextMeshProUGUI textLevelHandType;
		[SerializeField] private TextMeshProUGUI textPointChip;
        [SerializeField] private TextMeshProUGUI textPointMult;
        [SerializeField] private GameObject objHighLight;
        private PokerHandValue _pokerHandValue;
        private Action<PokerHandValue> _callback;
        public void OnInit(Action<PokerHandValue> callback)
        {
            _callback = callback;
        }
        public void SetDataHandType(PokerHandValue pokerHandValue, bool isSelect)
        {
            _pokerHandValue = pokerHandValue;
            var pc = PlayCardController.Instance.CalValueHandPoker(_pokerHandValue);
            textNameHandType.text = PlayCardController.Instance.GetNameByPokerValueHand(_pokerHandValue);
            textNumberOfHandPlayed.text =  PlayCardController.Instance.GetCountPlaying(pokerHandValue).ToString();
            textLevelHandType.text = PlayCardController.Instance.GetLevelByPokerValueHand(_pokerHandValue).ToString();
            textPointChip.text =  pc.pokerChip.ToString();
            textPointMult.text =  pc.pokerMult.ToString();
            objHighLight.SetActive(isSelect);
        }
        
        public void OnclickSelectHandType()
        {
            objHighLight.SetActive(true);
            _callback?.Invoke(_pokerHandValue);
        }
    }
}
