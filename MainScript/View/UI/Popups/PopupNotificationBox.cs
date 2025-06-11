using System;
using Frameworks.UIPopup;
using TMPro;
using UI.BaseColor;
using UI.Popups.ClassParameter;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class PopupNotificationBox : UIPopup
    {
        [SerializeField] private GameObject objNotificationBox;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private Image imgIcon;

        [SerializeField] private Sprite iconWarning;
        [SerializeField] private Sprite iconError;

        private NotificationParameter _data;

        private void Start()
        {
            objNotificationBox.SetActive(false);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnShown()
        {
            base.OnShown();
            OnShowNotification();
        }

        private void OnShowNotification()
        {
            _data = (NotificationParameter)Parameter;
            switch (_data.IconNotificationEnum)
            {
                case IconNotificationEnum.None:
                    break;
                case IconNotificationEnum.Warning:
                    imgIcon.sprite = iconWarning;
                    break;
                case IconNotificationEnum.Error:
                    imgIcon.sprite = iconError;
                    break;
            }
            txtDescription.text = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(_data.TextNotification);
            objNotificationBox.SetActive(true);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void HidePopup()
        {
            Hide();
        }
    }
}
