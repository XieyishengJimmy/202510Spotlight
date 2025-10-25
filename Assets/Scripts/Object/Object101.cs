using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object101 : ObjectBase
{
    public override void Init()
    {
        obj = new ObjectConfig();
        ObjectConfig tempObj = ConfigManager.Instance.GetConfig<ObjectConfig>(101);

        obj.name = tempObj.name;
        obj.desc = tempObj.desc;
        obj.width = tempObj.width;
        obj.height = tempObj.height;
        obj.hollow = tempObj.hollow;


        isWall = false;
        isPlayer = false;
        gridLock = new List<Vector2Int>();

        oData = new OriginalData();
        oData.size = new Vector2Int(obj.width, obj.height);
        oData.isHollow = obj.hollow;

    }

}
