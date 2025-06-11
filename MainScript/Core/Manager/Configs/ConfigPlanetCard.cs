using System;
using System.Collections.Generic;

namespace Core.Manager.Configs
{
    public class ConfigPlanetCardRecord
    {
        public int id;
        public string name;
        public int pokerHand;
        public int ratioPack;
        public int indexSprite;
        public string description;
    }

    public class ConfigPlanetCard : ConfigDataTable<ConfigPlanetCardRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("id");
        }
        public ConfigPlanetCardRecord GetPackByID(int id)
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