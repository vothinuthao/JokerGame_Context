using Core.Manager;

public class ConfigDefaultDataRecord
{
    public int id;
    public int coin;
    public int gem;
}

public class ConfigDefaultData : ConfigDataTable<ConfigDefaultDataRecord>
{

    private bool isRebuildByID = false;
    protected override void RebuildIndex()
    {
        RebuildIndexByField<int>("id");
    }
    public ConfigDefaultDataRecord GetNameByID(int id)
    {
        if (!isRebuildByID)
        {
            RebuildIndexByField<int>("id");
            isRebuildByID = true;
        }

        return GetRecordByIndex<int>("id", id);
    }
    public ConfigDefaultDataRecord GetDefaultData()
    {
        RebuildIndex();
        return GetRecordByIndex<int>("id", 0);
    }
}