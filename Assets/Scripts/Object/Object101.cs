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
        obj.width = oData.size.x;
        obj.height = oData.size.y;
        obj.hollow = tempObj.hollow;


        isWall = false;
        isPlayer = false;
        gridLock = new List<Vector2Int>();

        oData.isHollow = obj.hollow;

    }

}
