// using System;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace InGame
// {
//     public class LanguageVo : MonoBehaviour
//     {
//         [SerializeField] private TextMeshProUGUI languageName;
//         [SerializeField] private Image imgBtnLanguage;
//         
//         [SerializeField] private Sprite btnSpriteActive;
//         [SerializeField] private Sprite btnSpriteDeActive;
//
//         public LanguageSettingsEnum Token => _tokenLanguage;
//         private LanguageSettingsEnum _tokenLanguage;
//         private Action<LanguageSettingsEnum> _callback;
//         
//         public void OnInit(Action<LanguageSettingsEnum> callback)
//         {
//             _callback = callback;
//         }
//         public void SetDataBtn(LanguageSettingsEnum tokenLanguage, bool isSelected)
//         {
//             _tokenLanguage = tokenLanguage;
//             languageName.text = tokenLanguage.ToString();
//             imgBtnLanguage.sprite = isSelected ? btnSpriteActive : btnSpriteDeActive;
//         }
//
//         public void OnClick()
//         {
//             _callback?.Invoke(_tokenLanguage);
//         }
//     }
// }
