using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBase 
{
    //��������
    public bool isWall;
    public bool isPlayer;
    public ObjectConfig obj;
    public Vector2Int mapPos;

    //��̬����ռ����
    public List<Vector2Int> gridLock;

    //Ч���б��ʼ��
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



