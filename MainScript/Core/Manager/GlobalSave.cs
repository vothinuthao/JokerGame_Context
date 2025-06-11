using Core.Utils;
using UnityEngine;

namespace Manager
{
    public class GlobalSave : Singleton<GlobalSave>
    {
        private string _tokenLanguage = "English";
        private int _isInAppReview = 0;

        public string GetTokenLanguage => _tokenLanguage;
        public int IsInAppReview => _isInAppReview;
        public void ChangeTokenLanguage(string token)
        {
            if (_tokenLanguage != null && token == _tokenLanguage) return;
            _tokenLanguage = token;
        }

        public int GetValuePlayerReviewed()
        {
            int getValue = 0;
            if (!PlayerPrefs.HasKey("_isInAppReview")) 
            {
                PlayerPrefs.SetInt("_isInAppReview", 0);
                PlayerPrefs.Save();
            }
            else
            {
                getValue = PlayerPrefs.GetInt("_isInAppReview");
            }
            return getValue;
        }

        public void SetValuePlayerReviewed()
        {
            if (!PlayerPrefs.HasKey("_isInAppReview")) 
            {
                PlayerPrefs.SetInt("_isInAppReview", 1);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.SetInt("_isInAppReview", 1);
            }
        }
        
    }
    
    
    
}
