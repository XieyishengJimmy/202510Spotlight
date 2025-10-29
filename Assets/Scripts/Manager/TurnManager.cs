using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public bool readyToMove = false;

    public int turnCount;
    public List<EffectQueue> actionList;


    private void Awake()
    {
        instance = this;
        actionList = new List<EffectQueue>();
        readyToMove = true;
    }



    //�����б�������Ч���б�
    public void ActionAdd(List<EffectGroup> effectList)
    {
        EffectQueue newQueue = new EffectQueue();
        newQueue.startTurn = turnCount;
        newQueue.effectQueue = effectList;

        actionList.Add(newQueue);
    }

    public void TurnAction()
    {
        List<EffectGroup> newEffects = new List<EffectGroup>();

        //�������У�ɾ����ʱ�Ĳ��֣�����ʱ�ĵ���
        List<EffectQueue> toRemove = new List<EffectQueue>();
        foreach (var queue in actionList)
        {
            if (queue.startTurn + queue.effectQueue.Count < turnCount + 1)
                toRemove.Add(queue);
            else
            {
                EffectGroup eGroup = queue.effectQueue[turnCount - queue.startTurn];
                newEffects.Add(eGroup);
            }
        }

        foreach (var q in toRemove)
            actionList.Remove(q);

        //�����¾�Ч��
        MapManager.instance.ActionHandler(newEffects);


        int delta =  MapManager.instance.SimGravity(MapManager.instance.player);
        if(delta>1)
            MapManager.instance.isPlayerDead = true;

        if (MapManager.instance.isPlayerDead)
        {
            MapManager.instance.PlayerDie(DeadType.DieOfPushY); 
            return;
        }
        
        turnCount++;
        readyToMove = true;
    }

    public void EffectClear()
    {
        actionList.Clear();
    }

}


public class EffectQueue
{
    public int startTurn;
    public List<EffectGroup> effectQueue;
}

public class EffectGroup
{
    public TriggerGroup group;
    public EffectType effect;
}

public class SingleEffect
{
    public ObjectBase objb;
    public int effect;
}
