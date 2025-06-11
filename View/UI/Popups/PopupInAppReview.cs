using System.Collections;
using Frameworks.UIPopup;
using Google.Play.Review;
using Manager;
using UnityEngine;

namespace UI.Popups
{
    public class PopupInAppReview : UIPopup
    {
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;


        protected override void OnShown()
        {
            _reviewManager = new ReviewManager();
        }

        public void OnClickYes()
        {
            StartCoroutine(RequestReviews());
        }

        public void OnClickNo()
        {
            Hide();
        }
        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator RequestReviews()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();
            
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            GlobalSave.Instance.SetValuePlayerReviewed();
            Hide();
        }
        
    }
}
