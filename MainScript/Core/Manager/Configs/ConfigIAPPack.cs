using Core.Manager;

namespace Manager.Configs
{
    public class ConfigIAPPackRecord
    {
        public int id;
        public string name;
        public string reward;
        public float price;
        public int bonus;
        public int idPack;
        public string description;
    }

    public class ConfigIAPPack : ConfigDataTable<ConfigIAPPackRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("id");
        }
        public ConfigIAPPackRecord GetValueByID(int id)
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