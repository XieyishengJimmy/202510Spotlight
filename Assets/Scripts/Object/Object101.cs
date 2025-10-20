using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object101 : ObjectBase
{
    public override void Init()
    {
        obj = ConfigManager.Instance.GetConfig<ObjectConfig>(101);
        isWall = false;
        gridLock = new List<Vector2Int>();
        effectList = new SingleEffect[3];

        for (int i = 0; i < effectList.Length; i++)
        {
            effectList[i] = new SingleEffect();
            effectList[i].objb = this;
            effectList[i].effect = 201;
        }

    }

}
