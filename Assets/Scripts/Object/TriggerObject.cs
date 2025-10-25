using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    public List<EffectGroup> effectList;

    private void Awake()
    {
        effectList = new List<EffectGroup>();
    }


    public void TriggerEffect()
    {
        MapManager.instance.ActionAdd(effectList);
    }

}

public enum TriggerGroup
{
    None,
    GroupA,
    GroupB,
    GroupC
}
