using System;
using System.Collections.Generic;

namespace Core.Manager.Configs
{
    public class ConfigBoosterPackRecord
    {
        public int id;
        public string name;
        public int sizePackName;
        public int sizePack;
        public int usabilityPack;
        public string imageList;
        public int cost;
        public int typePack;
        public float ratioPack;
        public string description;
    }

    public class ConfigBoosterPack : ConfigDataTable<ConfigBoosterPackRecord>
    {

        private bool isRebuildByID = false;
        protected override void RebuildIndex()
        {
            RebuildIndexByField<int>("id");
        }
        public ConfigBoosterPackRecord GetPackByID(int id)
        {
            if (!isRebuildByID)
            {
                RebuildIndexByField<int>("id");
                isRebuildByID = true;
            }

            return GetRecordByIndex<int>("id", id);
        }

        public int RandomListImage(int id)
        {
            ConfigBoosterPackRecord pack = GetPackByID(id);
            var list = ConvertStringToList(pack.imageList);
            if (list == null || list.Count == 0)
                throw new InvalidOperationException("The list is empty or null");
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }
        private List<int> ConvertStringToList(string input)
        {
            string[] parts = input.Split(';');
            List<int> numbers = new List<int>();
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int number))
                {
                    numbers.Add(number);
                }
            }
            return numbers;
        }
    }
}