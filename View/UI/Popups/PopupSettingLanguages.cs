// using System;
// using System.Collections.Generic;
// using Core.Utils;
// using Frameworks.UIPopup;
// // using I2.Loc;
// using InGame;
// using Manager;
// using UnityEngine;
//
//
// public enum LanguageSettingsEnum
// {
//     English = 1,
//     Chinese = 2,
//     Vietnamese = 3
// }
// namespace UI.Popups
// {
//     public class PopupSettingLanguages : UIPopup
//     {
//         [SerializeField] private GameObject objectPrefabLanguage;
//         [SerializeField] private Transform transParentSpawn;
//         
//         [SerializeField] private List<LanguageSettingsEnum> listLanguageActive;
//         
//
//         private LanguageSettingsEnum _token;
//         // ReSharper disable Unity.PerformanceAnalysis
//         private void Start()
//         {
//             objectPrefabLanguage.SetActive(false);
//         }
//         
//         protected override void OnShown()
//         {
//             OnSpawnLanguage();
//         }
//
//         public void OnHide()
//         {
//             Hide();
//         }
//
//         private void OnSpawnLanguage()
//         {
//             GameObjectUtils.Instance.ClearAllChild(transParentSpawn.gameObject);
//             foreach (var language in  listLanguageActive)
//             {
//                 GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transParentSpawn, objectPrefabLanguage);
//                 var script = obj.GetComponent<LanguageVo>();
//                 script.SetDataBtn(language, language == _token);
//                 script.OnInit(OnClickCallback);
//                 obj.SetActive(true);
//             }
//         }
//         private void OnClickCallback(LanguageSettingsEnum token)
//         {
//             if (token == _token) return;
//             _token = token;
//             OnChangeLanguage(_token.ToString());
//             OnSpawnLanguage();
//         }
//
//         // public void OnChangeLanguage(string token)
//         // {
//         //     if(!LocalizationManager.HasLanguage(token)) return;
//         //     LocalizationManager.CurrentLanguage = token;
//         //     GlobalSave.Instance.ChangeTokenLanguage(token);
//         // }
//         
//     }
// }
