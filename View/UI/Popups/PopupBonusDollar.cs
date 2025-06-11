using System;
using Frameworks.Base;
using Frameworks.UIPopup;
using Manager;
using UI.Popups.ClassParameter;
using UnityEngine;

namespace UI.Popups
{
    public class MessageFreeDollar
    {
        public Action<bool> Callback = default;
    }
    public class PopupBonusDollar : UIPopup
    {
        private MessageFreeDollar _messageBoss;
        private int _dollarCanGet = 5;
        protected override void OnShown()
        {
            _messageBoss = (MessageFreeDollar)Parameter;
            SetInfo();
        }

        private void SetInfo()
        {
            
        }

        public void OnClickWatchAds()
        {
            if (PlayerDataManager.Instance.Property.RemoveAds)
            {
                AddDollarAfterAds();
                this.Hide();
                return;
            }
            if (IronSourceManager.Instance.IsRewardedAdsAvailable(RewardedAdsPlacement.Dollar_Free))
            {
                IronSourceManager.Instance.ShowRewardedVideo(RewardedAdsPlacement.Dollar_Free, (success) =>
                {
                    if (success)
                    {
                        AddDollarAfterAds();
                    }
                    _messageBoss.Callback?.Invoke(success);
                    this.Hide();
                });
            }
        }

        public void OnClickHideObject()
        {
            _messageBoss.Callback?.Invoke(false);
            this.Hide();
        }

        private bool AddDollarAfterAds()
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
            {
                TextNotification = "Congratulations on your receipt 5 Dollar",
                IconNotificationEnum = IconNotificationEnum.Warning,
            });
            return PlayerDataManager.Instance.Property.AddDollar(_dollarCanGet);
        }
    }
}
