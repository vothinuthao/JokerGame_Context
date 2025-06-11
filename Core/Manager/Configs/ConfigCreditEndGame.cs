namespace Core.Manager.Configs
{
    public class ConfigCreditEndGameRecord
    {
        public int id;
        public int milestone;
        public int interest;
    }

    public class ConfigCreditEndGame : ConfigDataTable<ConfigCreditEndGameRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("id");
        }
        public ConfigCreditEndGameRecord GetValueByID(int id)
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