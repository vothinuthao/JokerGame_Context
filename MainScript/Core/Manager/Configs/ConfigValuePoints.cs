namespace Core.Manager.Configs
{
    public class ConfigValuePointsRecord
    {
        public int id;
        public string name;
        public int chip;
        public int mult;
    }

    public class ConfigValuePoints : ConfigDataTable<ConfigValuePointsRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("id");
        }
        public ConfigValuePointsRecord GetValueByID(int id)
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