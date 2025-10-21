using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayer : ObjectBase
{
    public override void Init()
    {
        obj = ConfigManager.Instance.GetConfig<ObjectConfig>(999);
        isWall = false;
        isPlayer = true;
        gridLock = new List<Vector2Int>();
    }

}
