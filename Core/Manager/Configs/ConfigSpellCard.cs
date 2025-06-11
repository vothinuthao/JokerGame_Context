using Core.Manager;

namespace Manager.Configs
{
    public class ConfigSpellCardRecord
    {
        public int id;
        public string nameCode;
        public string name;
        public int countAttack;
        public int ratioPack;
        public int indexSprite;
        public string description;
        public bool isActive;
    }

    public class ConfigSpellCard : ConfigDataTable<ConfigSpellCardRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("id");
        }
        public ConfigSpellCardRecord GetPackByID(int id)
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