// using System.Collections.Generic;
// using Core.Entity;
// using Core.Utils;
// // using I2.Loc;
// using UnityEngine;
//
// namespace Manager
// {
//     public class I2LocalizationManager : ManualSingletonMono<I2LocalizationManager>
//     {
//         public string GetString(string key, LocalizationCategory category = LocalizationCategory.NONE, params object[] args)
//         {
//             try
//             {
//                 string term = string.Format("{0}{1}", category == LocalizationCategory.NONE ? "" : category.ToString() + "/", key);
//                 var result = LocalizationManager.GetTranslation(term);
//                 if (string.IsNullOrEmpty(result))
//                     return key;
//                 return string.Format(result, args);
//             }
//             catch
//             {
//                 return "";
//             }
//         }
//
//         public static List<string> GetAllString(LocalizationCategory category = LocalizationCategory.NONE)
//         {
//             List<string> termsList = category == LocalizationCategory.NONE ? LocalizationManager.GetTermsList() : LocalizationManager.GetTermsList(category.ToString());
//             List<string> result = new List<string>();
//             foreach (var term in termsList)
//             {
//                 result.Add(LocalizationManager.GetTermTranslation(term));
//             }
//             return result;
//         }
//
//         public static List<string> GetAllTerm(LocalizationCategory category = LocalizationCategory.NONE)
//         {
//             List<string> termsList = category == LocalizationCategory.NONE ? LocalizationManager.GetTermsList() : LocalizationManager.GetTermsList(category.ToString());
//             return termsList;
//         }
//
//         public static string GetRandomString(LocalizationCategory category = LocalizationCategory.NONE)
//         {
//             List<string> result = GetAllString(category);
//             return result.Count > 0 ? result[Random.Range(0, result.Count)] : "";
//         }
//
//         public TermData GetStringData(string key, LocalizationCategory category = LocalizationCategory.NONE)
//         {
//             string term = string.Format("{0}{1}", category == LocalizationCategory.NONE ? "" : category.ToString() + "/", key);
//             var data = LocalizationManager.GetTermData(term);
//             return data;
//         }
//
//         public string GetDescription(string key, LocalizationCategory category = LocalizationCategory.NONE)
//         {
//             string term = string.Format("{0}{1}", category == LocalizationCategory.NONE ? "" : category.ToString() + "/", key);
//             var data = LocalizationManager.GetTermData(term);
//             if (data == null)
//                 return "";
//             return data.Description;
//         }
//
//         public void SetLocalizeTerm(ref Localize localize, string key, LocalizationCategory category = LocalizationCategory.NONE)
//         {
//             string term = string.Format("{0}{1}", category == LocalizationCategory.NONE ? "" : category.ToString() + "/", key);
//             localize.SetTerm(term);
//         }
//
//         public void SetLocalizeStringTerm(ref LocalizedString localizedString, string key, LocalizationCategory category = LocalizationCategory.NONE)
//         {
//             string term = string.Format("{0}{1}", category == LocalizationCategory.NONE ? "" : category.ToString() + "/", key);
//             localizedString = term;
//         }
//
//     }
// }
