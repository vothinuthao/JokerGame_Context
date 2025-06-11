using Core.Entity;

namespace Core.Manager.Configs
{
    public class ConfigValueHandsRecord
    {
        public int id;
        public string name;
        public int chip;
        public int mult;
        public int upPerLevelChip;
        public int upPerLevelMult;
        public string exampleRank;
        public string exampleSuit;
        public int availableCount;
    }

    public class ConfigValueHands : ConfigDataTable<ConfigValueHandsRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("id");
        }
        public ConfigValueHandsRecord GetValueByID(int id)
        {
            if (!isRebuildByID)
            {
                RebuildIndexByField<int>("id");
                isRebuildByID = true;
            }

            return GetRecordByIndex<int>("id", id);
        }
    }
}