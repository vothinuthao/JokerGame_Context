public abstract class DefineShopData
{
    public enum TypeBoosterPack
    {
        None = 0,
        Spell = 1,
        Joker = 2,
        Planet = 3,
        Spectral = 4,
        Standard = 5
    }


    public abstract class SpellCardName
    {
        public static string s_heart_morph = "s_heart_morph";
        public static string s_diamond_morph = "s_diamond_morph";
        public static string s_club_morph = "s_club_morph";
        public static string s_spade_morph = "s_spade_morph";
        public static string s_spreading_growth = "s_spreading_growth";
        public static string s_decimation = "s_decimation";
        public static string s_money_multiplication = "s_money_multiplication";
        public static string s_wild_growth = "s_wild_growth";
        public static string s_interchange = "s_interchange";
        public static string s_card_multiplication = "s_card_multiplication";
        public static string s_card_transfusion = "s_card_transfusion";
        public static string s_empower = "s_empower";
    }

    public enum RewardPack
    {
        None = 0,
        BoosterPackJoker = 10001, // 100016
        BoosterPackPlanet = 10002,
        BoosterPackStandard = 10003,
        BoosterPackSpell = 10004,
        ExtraSlotJoker = 10005
    }
    
}