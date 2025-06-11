using Frameworks.UIPopup;
using Manager;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI.Popups
{
	public class PopupReviveAds : UIPopup
	{
		[FoldoutGroup("All Button Type")]
		[SerializeField]
		private GameObject buttonOk;
		[FoldoutGroup("All Button Type")]
		[SerializeField]
		private GameObject buttonCancel;
		[FoldoutGroup("All Button Type")]
		[SerializeField]
		private GameObject buttonYes;
		[FoldoutGroup("All Button Type")]
		[SerializeField]
		private TextMeshProUGUI textTime;
		[FoldoutGroup("All Button Type")]
		[SerializeField]
		private GameObject buttonNo;
		[FoldoutGroup("All Button Type")]
		[SerializeField]
		private GameObject buttonRetry;
		

		public TextMeshProUGUI textTitle;
		public TextMeshProUGUI textMessage;

		private int _watchAdsTime = 0;
		private int _specifiedNumberTimes = 2;

		public enum MessageBoxType
		{
			OK,
			OK_Cancel,
			Yes_No,
			Retry
		}

		public enum MessageBoxAction
		{
			Accept,
			Deny
		} 

		public delegate bool OnMessageBoxAction(MessageBoxAction action);

		public class MessageBoxParam
		{
			public MessageBoxType MessageBoxType;
		
			public OnMessageBoxAction OnMessageBoxActionCallback;
		
			public string MessageTitle { get; set; }

			public string MessageBody { get; set; }

			public bool ExcuteAccept ()
			{
				if (OnMessageBoxActionCallback != null) {
					return OnMessageBoxActionCallback (MessageBoxAction.Accept);
				}
				return true;
			}
    
			public bool ExcuteDeny ()
			{
				if (OnMessageBoxActionCallback != null) {
					return OnMessageBoxActionCallback (MessageBoxAction.Deny);
				}
				return true;
			}
		
		}

		public MessageBoxParam MessageBoxParameter {
			get {
				return Parameter as MessageBoxParam;
			}
			set {
				Parameter = value;
			}
		}

		public void OnYesClick()
		{
			if (MessageBoxParameter != null) {
				if (PlayerDataManager.Instance.Property.RemoveAds)
				{
					if (MessageBoxParameter.ExcuteAccept ()) {
						this.Hide();
					}
					Hide();
				}
				else
				{
					if (IronSourceManager.Instance.IsRewardedAdsAvailable(RewardedAdsPlacement.Game_Over_Revive))
					{
						IronSourceManager.Instance.ShowRewardedVideo(RewardedAdsPlacement.Game_Over_Revive, (success) =>
						{
							if (success )
							{
								_watchAdsTime++;
								if (_watchAdsTime == _specifiedNumberTimes)
								{
									if (MessageBoxParameter != null)
									{
										if (MessageBoxParameter.ExcuteAccept())
										{
											this.Hide();
										}
									}
									Hide();
								}
							}
							else
							{
								Debug.LogError("Ads not available, please try later!");
								OnNoClick();
							}
						});
					}
					SetupData();
				}
				
			}
		}

		public void OnNoClick()
		{
			if (MessageBoxParameter.ExcuteDeny ()) {
				this.Hide();
			}
		}

		public void OnRetryClick()
		{
			if (MessageBoxParameter != null) {
				if (MessageBoxParameter.ExcuteAccept ()) {
					this.Hide();
				}
			}
		}

		// ReSharper disable Unity.PerformanceAnalysis
		protected override void OnShowing()
		{
			base.OnShowing();
			_watchAdsTime = 0;
			SetupData();

		}

		void SetupData()
		{
			if (MessageBoxParameter == null)
			{
				Debug.LogError("Message box has no param??");
				return;
			}

			textTime.text = $"Watch {_specifiedNumberTimes - _watchAdsTime} Ads";
			bool hasOK = MessageBoxParameter.MessageBoxType == MessageBoxType.OK ||
			             MessageBoxParameter.MessageBoxType == MessageBoxType.OK_Cancel;

			bool hasCancel = MessageBoxParameter.MessageBoxType == MessageBoxType.OK_Cancel;
			bool hasYes = MessageBoxParameter.MessageBoxType == MessageBoxType.Yes_No;
			bool hasNo = MessageBoxParameter.MessageBoxType == MessageBoxType.Yes_No;
			bool hasRetry = MessageBoxParameter.MessageBoxType == MessageBoxType.Retry;
		
			if(buttonOk)
				buttonOk.SetActive(hasOK);
			if(buttonCancel)
				buttonCancel.SetActive(hasCancel);
			if(buttonYes)
				buttonYes.SetActive(hasYes);
			if(buttonYes)
				buttonYes.SetActive(hasNo);
			if(buttonRetry)
				buttonRetry.SetActive(hasRetry);

			textTitle.text = MessageBoxParameter.MessageTitle;
			textMessage.text = MessageBoxParameter.MessageBody;
		}
	}
}
