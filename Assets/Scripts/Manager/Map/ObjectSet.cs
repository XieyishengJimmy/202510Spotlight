using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSet : MonoBehaviour
{
    public void SetPos()
    {
        Vector3 newPos;
        newPos.x = RoundToNearestMultiple(transform.position.x, 0.5f);
        newPos.y = RoundToNearestMultiple(transform.position.y, 0.5f);
        newPos.z = 0f;
        transform.position = newPos;
    }

    private float RoundToNearestMultiple(float value, float multiple)
    {
        float num;

        if (value > 0)
        {
            num = Mathf.Floor(value / multiple);
        }
        else
        {
            num = Mathf.Ceil(value / multiple);
        }

        if (num % 2 != 0)
        {
            return num * 0.5f;
        }
        else
        {
            if (num != 0)
                return num >= 0 ? (num * 0.5f) + 0.5f : (num * 0.5f) - 0.5f;
            else
                return value >= 0 ? 0.5f : -0.5f;
        }

    }
}
