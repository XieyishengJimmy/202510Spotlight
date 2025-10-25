using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColumnData
{
    public int columnIndex;
    public TriggerGroup group; 
    public EffectType effectType; 
}

[System.Serializable]
public class ListData
{
    public int listIndex;
    public List<ColumnData> columns = new List<ColumnData>();
}

[System.Serializable]
public class SaveData
{
    public List<ListData> lists = new List<ListData>();
}
