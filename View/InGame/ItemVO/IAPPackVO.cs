using System;
using Manager.Configs;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace InGame
{
    public class IAPPackVO : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textNamePack;
        [SerializeField] private TextMeshProUGUI pricePack;
        [SerializeField] private MMF_Player mmfFeedback;
        private ConfigIAPPackRecord _data;

        private Action<ConfigIAPPackRecord> _callbackSelect;
        private Action<ConfigIAPPackRecord> _callback;

        public void OnInit(Action<ConfigIAPPackRecord> callbackSelect, Action<ConfigIAPPackRecord> callback)
        {
            _callbackSelect = callbackSelect;
            _callback = callback;
        }
        public void SetData(ConfigIAPPackRecord data)
        {
            _data = data;
            textNamePack.text = $"{_data.name}";
            pricePack.text = $"${_data.price}";
            mmfFeedback.PlayFeedbacks();
        }

        public void OnClick()
        {
            _callbackSelect?.Invoke(_data);
        }
        public void OnClickBuyPack()
        {
            _callback?.Invoke(_data);
        }
    }
}
