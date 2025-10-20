using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBase 
{
    //��������
    public bool isWall;
    public bool isPlayer = false;
    public ObjectConfig obj;
    public Vector2Int mapPos;

    //��̬����ռ����
    public List<Vector2Int> gridLock;

    //Ч���б��ʼ��
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



