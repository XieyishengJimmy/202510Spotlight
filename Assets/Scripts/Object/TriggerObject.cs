using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    public List<SingleEffect> effectList;

    private void Awake()
    {
        effectList = new List<SingleEffect>();
    }

    public void TriggerEffect()
    {
        SingleEffect[] effectArray = new SingleEffect[effectList.Count];
        for (int i = 0; i < effectArray.Length; i++)
        {
            effectArray[i] = effectList[i];
        }
        MapManager.instance.ActionAdd(effectArray);
    }

    public void LookLook()
    {
        int index = 0;
        foreach (var item in effectList)
        {
            index++;
            Debug.Log($"ÐòºÅ = {index},OBJ={item.objb},effect={item.effect}");
        }
    }

}
