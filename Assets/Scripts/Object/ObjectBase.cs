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

    public bool readyToBePush = false;


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

    public void PlayerDid(DeadType type)
    {
        ObjectHandler[] handlers = GameObject.FindObjectsOfType<ObjectHandler>();
        ObjectHandler playerH = null;

        foreach (var handler in handlers)
        {
            if (handler.objb == this)
            {
                playerH = handler;
                break;
            }
        }

        switch (type)
        {
            case DeadType.DieOfPushX:
                if(playerH != null)
                playerH.anim.Play("PlayerDie");
                break;
            case DeadType.DieOfPushY:
                if (playerH != null)
                    playerH.anim.Play("PlayerDieFall");
                break;
            default:
                break;
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



