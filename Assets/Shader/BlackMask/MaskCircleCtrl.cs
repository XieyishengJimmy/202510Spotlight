using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskCircleCtrl : MonoBehaviour
{
    public RectTransform Target;

    [Range(0, 40)]
    public float Size = 1;


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Target != null)
        {
            Target.localScale = Vector3.one * Size;
        }
    }
#endif

    public void ShowIn()
    {

    }

    public void ShowOut()
    {

    }

    void OnDestroy()
    {
        if (Target != null)
        { }

    }
}
