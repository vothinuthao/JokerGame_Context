using Core.Manager;

namespace Manager.Configs
{
    public class ConfigPlayRoundsRecord
    {
        public int round;
        public int firstBoss;
        public string nameRound;
        public string enemyID;
        public int difficultLevel;
        public int baseStakeReward;
    }

    public class ConfigPlayRound : ConfigDataTable<ConfigPlayRoundsRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("round");
        }
        public ConfigPlayRoundsRecord GetValueByRound(int round)
        {
            if (!isRebuildByID)
            {
                RebuildIndexByField<int>("round");
                isRebuildByID = true;
            }

            return GetRecordByIndex<int>("round", round);
        }
    }
}