using System;
using System.Collections.Generic;

namespace Core.Manager.Configs
{
    public class ConfigBossCutSceneDescriptionRecord
    {
        public int id;
        public int idEnemy;
        public string defeatedSpeech;
        public string winSpeech;
        public string encounterSpeech;
    }

    public class ConfigBossCutSceneDescription : ConfigDataTable<ConfigBossCutSceneDescriptionRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("idEnemy");
        }
        public ConfigBossCutSceneDescriptionRecord GetBossCutScene(int id)
        {
            if (!isRebuildByID)
            {
                RebuildIndexByField<int>("idEnemy");
                isRebuildByID = true;
            }

            return GetRecordByIndex<int>("idEnemy", id);
        }
    }
}