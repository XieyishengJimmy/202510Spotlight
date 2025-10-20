using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    //Õº∆¨π‹¿Ì
    public SpriteRenderer sp;
    public ObjectBase objb;

    private void Awake()
    {
        sp = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        objb = new Object101();
    }

    public void PositionAdjust()
    {
        var newPos = MapManager.instance.GridToWorld(objb.mapPos);
        this.transform.position = newPos;
    }

    public void SizeAdjust()
    {
        sp.size = new Vector2(objb.obj.width, objb.obj.height);
        sp.transform.localPosition = new Vector3((sp.size.x - 1f) / 2, (sp.size.y - 1f) / 2, 0f);
        //Debug.Log(objb.obj.height);
    }

    public void ColorAdjust()
    {
        switch (objb.obj.color)
        {
            case 0:
                sp.color = Color.white;
                break;
            case 1:
                sp.color = Color.red;
                break;
            default:
                break;
        }
    }
}
