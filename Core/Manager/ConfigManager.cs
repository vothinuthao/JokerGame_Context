using Core.Manager.Configs;
using Core.Utils;
using Manager.Configs;

namespace Core.Manager
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        public const string assetClientConfigNames = "config-bundle-names";
        public const string assetUnetConfigNames = "config-unet-bundle-names";

        public const string assetClientConfigBundle = "configs-bundle";
        public const string assetUnetConfigBundle = "configs-unet-bundle";
        public static bool configLoaded = false;

        //public static Dictionary<Language, ConfigString> dictConfigString = new Dictionary<Language, ConfigString> ();

        #region CONFIG_CLIENT_ONLY

        private static string CONF_CLIENT_PATH = "Configs/ClientOnly/";

        //public static ConfigLocalLanguage configLocalLang;

        //public static ConfigSkinMaterial configSkinMaterial;

        #endregion

        #region CONFIG_SHARE

        private static string CONF_SHARE_PATH = "Configs/";

        public static ConfigValuePoints configValuePoints;
        public static ConfigValueHands configValueHands;
        public static ConfigPlayRound configPlayRound;
        public static ConfigDefaultData configDefaultData;
        public static ConfigBoosterPack configBoosterPack;
        public static ConfigPlanetCard configPlanetCard;
        public static ConfigCreditEndGame configCreditEndGame;
        public static ConfigBossCutSceneDescription configBossCutSceneDescription;
        public static ConfigSpellCard configSpell;
        public static ConfigIAPPack configIAPPack;
        #endregion

        #region CONFIG_UNET_ONLY

        private static string CONF_UNET_PATH = "Configs/UNETOnly/";

        #endregion

        #region GAMEPLAY_CONFIG

        //public SpecialSkillProperties[] specialSkillProps = null;
        //public StatProperties[] statProps = null;

        #endregion

        //======================================================

        private static bool isLoadedConfigLocal = false;
        public static bool IsLoadedConfigLocal
        {
            set { isLoadedConfigLocal = value; }
            get { return isLoadedConfigLocal; }
        }


        public void LoadAllConfigLocal()
        {
            if (isLoadedConfigLocal)
                return;

            //Fixed bug read config on VI, TH ... devices
            System.Globalization.CultureInfo customCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            customCulture.DateTimeFormat.LongDatePattern = "MM/dd/yyyy HH:mm:ss";
            customCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            //=====================LOAD_CONFIG_CLIENT_ONLY====================
            configDefaultData = new ConfigDefaultData();
            configDefaultData.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigDefaultData");
            // load config default if player is new guys
            
            configValuePoints = new ConfigValuePoints();
            configValuePoints.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigValuePoints");
            configValueHands = new ConfigValueHands();
            configValueHands.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigValueHands");
            configPlayRound = new ConfigPlayRound();
            configPlayRound.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigPlayRound");
            
            //shop
            configBoosterPack = new ConfigBoosterPack();
            configBoosterPack.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigBoosterPack");
            configPlanetCard = new ConfigPlanetCard();
            configPlanetCard.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigPlanetCard");
            configSpell = new ConfigSpellCard();
            configSpell.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigSpellCard");
            //API
            configIAPPack = new ConfigIAPPack();
            configIAPPack.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigIAPPack");
            
            //Game Play
            configBossCutSceneDescription = new ConfigBossCutSceneDescription();
            configBossCutSceneDescription.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigBossCutSceneDescription");
            

            // result end game
            configCreditEndGame = new ConfigCreditEndGame();
            configCreditEndGame.LoadFromAssetPath(CONF_SHARE_PATH + "ConfigCreditEndGame");
            //========================LOAD_CONFIG_SHARE========================
            
        }


    }
}
