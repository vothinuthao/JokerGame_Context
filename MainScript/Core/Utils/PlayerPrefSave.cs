using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefSave
{
    public static int CountPlayGame
    {
        set { PlayerPrefs.SetInt("CountPlayGame", value); }
        get { return PlayerPrefs.GetInt("CountPlayGame", 0); }
    }
    public static double Gold
    {
        set { PlayerPrefs.SetString("Gold", value.ToString()); }
        get { return double.Parse(PlayerPrefs.GetString("Gold", "400")); }
    }
    public static double Gem
    {
        set { PlayerPrefs.SetString("Gem", value.ToString()); }
        get { return double.Parse(PlayerPrefs.GetString("Gem", "10")); }
    }
    public static string TimeExit
    {
        set { PlayerPrefs.SetString("TimeExit", value.ToString()); }
        get { return PlayerPrefs.GetString("TimeExit", ""); }
    }
    public static int LevelCurrent
    {
        set { PlayerPrefs.SetInt("LevelCurrent", value); }
        get { return PlayerPrefs.GetInt("LevelCurrent", 1); }
    }
    public static float ExpCurrent
    {
        set { PlayerPrefs.SetFloat("ExpCurrent", value); }
        get { return PlayerPrefs.GetFloat("ExpCurrent", 0); }
    }
    public static int RemoveAds
    {
        set { PlayerPrefs.SetInt("RemoveAds", value); }
        get { return PlayerPrefs.GetInt("RemoveAds", 0); }
    }
    public static int TotalTimePlayGame
    {
        set { PlayerPrefs.SetInt("TotalTimePlayGame", value); }
        get { return PlayerPrefs.GetInt("TotalTimePlayGame", 0); }
    }
    public static int TutorialStep
    {
        set { PlayerPrefs.SetInt("TutorialStep", value); }
        get { return PlayerPrefs.GetInt("TutorialStep", 0); }
    }
    public static int IdLanguage
    {
        set { PlayerPrefs.SetInt("IdLanguage", value); }
        get { return PlayerPrefs.GetInt("IdLanguage", 0); }
    }
    #region handle save unlock item
    public static void UnlockItem(string nameItem)
    {
        PlayerPrefs.SetInt("UnlockItem" + nameItem, 1);
    }
    public static bool CheckUnlockItem(string nameItem)
    {
        return PlayerPrefs.HasKey("UnlockItem" + nameItem);
    }
    public static void SavePrice(string nameItem, float priceChange)
    {
        PlayerPrefs.SetFloat("price" + nameItem, priceChange);
    }
    public static float GetPrice(string nameItem, float priceOrigin = 0)
    {
        if (!PlayerPrefs.HasKey("price" + nameItem))
        {
            PlayerPrefs.SetFloat("price" + nameItem, priceOrigin);
        }
        return PlayerPrefs.GetFloat("price" + nameItem);
    }
    #endregion
    #region handle Data product
    public static void ChangeTotalProduct(int idProduct, int count)
    {
        PlayerPrefs.SetInt("TotalProduct" + idProduct, GetTotalProduct(idProduct) + count);
    }

    public static int GetTotalProduct(int idProduct)
    {
        return PlayerPrefs.GetInt("TotalProduct" + idProduct, 0);
    }
    #endregion
    #region Handle Oder
    public static int TotalOderCurrent
    {
        set { PlayerPrefs.SetInt("TotalOderCurrent", value); }
        get { return PlayerPrefs.GetInt("TotalOderCurrent", 0); }
    }
    public static int FirstOder
    {
        set { PlayerPrefs.SetInt("FirstOder", value); }
        get { return PlayerPrefs.GetInt("FirstOder", 0); }
    }
    public static int FirstOderCustomer
    {
        set { PlayerPrefs.SetInt("FirstOderCustomer", value); }
        get { return PlayerPrefs.GetInt("FirstOderCustomer", 0); }
    }
    #endregion
    #region setting
    public static float Music
    {
        set { PlayerPrefs.SetFloat("Music", value); }
        get { return PlayerPrefs.GetFloat("Music", .3f); }
    }
    public static float Sound
    {
        set { PlayerPrefs.SetFloat("Sound", value); }
        get { return PlayerPrefs.GetFloat("Sound", 1); }
    }
    public static float Vibration
    {
        set { PlayerPrefs.SetFloat("Vibration", value); }
        get { return PlayerPrefs.GetFloat("Vibration", 1); }
    }
    #endregion
    #region handle mini game deco
    public static bool CheckOwnDeco(int id, int number)
    {
        return PlayerPrefs.HasKey("OwnDeco" + id + "number" + number);
    }
    public static string TimeTotalIdleDeco
    {
        set { PlayerPrefs.SetString("TimeTotalIdleDeco", value); }
        get { return PlayerPrefs.GetString("TimeTotalIdleDeco"); }
    }
    public static void SetTimeIdleDeco(int id, int number)
    {
        PlayerPrefs.SetString("TimeIdleDeco" + id + "number" + number, System.DateTime.Now.ToString());
    }
    public static string GetTimeSaveIdleDeco(int id, int number)
    {
        return PlayerPrefs.GetString("TimeIdleDeco" + id + "number" + number);
    }
    public static void SetGemIdleDeco(int id, int number, int count)
    {
        PlayerPrefs.SetInt("GemIdleDeco" + id + "number" + number, count);
    }
    public static int GetGemIdleDeco(int id, int number)
    {
        return PlayerPrefs.GetInt("GemIdleDeco" + id + "number" + number, 0);
    }
    public static void UnlockDeco(int id, int number)
    {
        PlayerPrefs.SetInt("OwnDeco" + id + "number" + number, 1);
    }
    public static void SetDecoCurrent(int id, int number)
    {
        PlayerPrefs.SetInt("DecoCurrent" + id, number);
    }
    public static int GetDecoCurrent(int id)
    {
        return PlayerPrefs.GetInt("DecoCurrent" + id, -1);
    }
    public static void SaveAdsDeco(int id, int number)
    {
        PlayerPrefs.SetInt("AdsDeco" + id + "number" + number, GetAdsDeco(id, number) + 1);
    }
    public static int GetAdsDeco(int id, int number)
    {
        return PlayerPrefs.GetInt("AdsDeco" + id + "number" + number, 0);
    }
    #endregion
    public static string TimeSpinWheel
    {
        set { PlayerPrefs.SetString("TimeSpinWheel", value.ToString()); }
        get { return PlayerPrefs.GetString("TimeSpinWheel", ""); }
    }
    #region Handle mission
    public static int IdMissionCurrent
    {
        set { PlayerPrefs.SetInt("IdMissionCurrent", value); }
        get { return PlayerPrefs.GetInt("IdMissionCurrent", 0); }
    }
    public static int GetCountMission(int idMission)
    {
        return PlayerPrefs.GetInt("CountMission" + idMission, 0);
    }
    public static void SetCountMission(int idMission, int count)
    {
        PlayerPrefs.SetInt("CountMission" + idMission, GetCountMission(idMission) + count);
    }

    #endregion
    #region Handle tutorial
    public static int IdTutorialCurrent
    {
        set { PlayerPrefs.SetInt("IdTutorialCurrent", value); }
        get { return PlayerPrefs.GetInt("IdTutorialCurrent", 0); }
    }
    public static int GetDoneTutorial(int idTutorial)
    {
        return PlayerPrefs.GetInt("CountTutorial" + idTutorial, 0);
    }
    public static void SetDoneTutorial(int idTurotial, int count)
    {
        PlayerPrefs.SetInt("CountTutorial" + idTurotial, GetDoneTutorial(idTurotial) + count);
    }
    #endregion
}
