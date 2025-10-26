using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBase 
{
    //基本数据
    public bool isWall;
    public bool isPlayer;
    public ObjectConfig obj;
    public Vector2Int mapPos;

    //动态保存占格组
    public List<Vector2Int> gridLock;

    //效果列表初始化
    public SingleEffect[] effectList;

    public OriginalData oData;


    public virtual void Init() { }

    //public void triggerBegin()
    //{
    //    MapManager.instance.ActionAdd(effectList);
    //}

    public void AdjustAll()
    {
        ObjectHandler[] handlers = GameObject.FindObjectsOfType<ObjectHandler>();

        foreach (var handler in handlers)
        {
            if (handler.objb == this)
            {
                handler.SizeAdjust();
                handler.PositionAdjust();
                handler.AdjustAlpha();
                break;
            }
        }
    }

    public void AdjustPosition()
    {
        ObjectHandler[] handlers = GameObject.FindObjectsOfType<ObjectHandler>();

        foreach (var handler in handlers)
        {
            if (handler.objb == this)
            {
                handler.PositionAdjust();
                break;
            }
        }
    }

    public void PlayerMove()
    {
        ObjectHandler[] handlers = GameObject.FindObjectsOfType<ObjectHandler>();

        foreach (var handler in handlers)
        {
            if (handler.objb == this)
            {
                handler.PlayerMoveAnim();
                break;
            }
        }
    }


}


public class OriginalData
{
    public Vector2Int size;
    public Vector2Int mapPos;
    public bool isHollow;
}



