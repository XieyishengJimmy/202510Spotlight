using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBase 
{
    //基本数据
    public bool isWall;
    public bool isPlayer = false;
    public ObjectConfig obj;
    public Vector2Int mapPos;

    //动态保存占格组
    public List<Vector2Int> gridLock;

    //效果列表初始化
    public SingleEffect[] effectList;

    public virtual void Init() { }

    public void triggerBegin()
    {
        MapManager.instance.ActionAdd(effectList);
    }

    public void Gravity()
    {

    }

    public void AdjustAll()
    {
        ObjectHandler[] handlers = GameObject.FindObjectsOfType<ObjectHandler>();

        foreach (var handler in handlers)
        {
            if (handler.objb == this)
            {
                handler.SizeAdjust();
                handler.ColorAdjust();
                handler.PositionAdjust();
                break;
            }
        }
    }

}



