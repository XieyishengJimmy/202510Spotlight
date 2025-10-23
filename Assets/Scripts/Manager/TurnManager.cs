using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public bool readyToMove = false;

    public int turnCount;
    public List<EffectQueue> actionList;

    public List<SingleEffect> lastTurnEffect;


    private void Awake()
    {
        instance = this;
        lastTurnEffect = new List<SingleEffect>();
        actionList = new List<EffectQueue>();
        readyToMove = true;
    }



    //处理列表中新增效果列表
    public void ActionAdd(SingleEffect[] effectList)
    {
        EffectQueue newQueue = new EffectQueue();
        newQueue.startTurn = turnCount;
        newQueue.effectQueue = effectList;

        actionList.Add(newQueue);
    }



    public void TurnAction()
    {
        readyToMove = false;

        List<SingleEffect> oldEffects = new List<SingleEffect>(lastTurnEffect);
        lastTurnEffect.Clear();
        List<SingleEffect> newEffects = new List<SingleEffect>();

        //遍历队列，删除超时的部分，不超时的调用
        List<EffectQueue> toRemove = new List<EffectQueue>();
        foreach (var queue in actionList)
        {
            if (queue.startTurn + queue.effectQueue.Length < turnCount + 1)
                toRemove.Add(queue);
            else
            {
                SingleEffect sEffect = queue.effectQueue[turnCount - queue.startTurn];
                newEffects.Add(sEffect);
            }
        }

        foreach (var q in toRemove)
            actionList.Remove(q);

        //触发新旧效果
        MapManager.instance.ActionHandler(oldEffects, newEffects);
        lastTurnEffect = newEffects;


        int delta =  MapManager.instance.SimGravity(MapManager.instance.player);

        turnCount++;
        readyToMove = true;
    }

}

public class EffectQueue
{
    public int startTurn;
    public SingleEffect[] effectQueue;
}

public class SingleEffect
{
    public ObjectBase objb;
    public int effect;
}
