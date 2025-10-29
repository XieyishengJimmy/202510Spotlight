using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class ObjectHandler : MonoBehaviour
{
    //Õº∆¨π‹¿Ì
    public SpriteRenderer sp;
    public ObjectBase objb;
    public ObjectType objt;
    public TriggerGroup group;

    public Transform playerTransform;
    public Animator anim;

    public Vector2Int oSize;



    private void Awake()
    {
        sp = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        playerTransform = this.transform;

        if(transform.GetChild(0).GetComponent<Animator>()!= null)
            anim = transform.GetChild(0).GetComponent<Animator>();

        switch (objt)
        {
            case ObjectType.Player:
                objb = new ObjectPlayer();
                break;
            case ObjectType.Object101:
                objb = new Object101();
                break;
            case ObjectType.Object102:
                objb = new Object102();
                break;
            default:
                break;
        }

        objb.oData = new OriginalData();
        objb.oData.size = oSize;
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

    public void AdjustAlpha()
    {
        if(objb.obj.hollow)
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0.4f);
        else
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b,1f);
    }

    public void PlayerMoveAnim()
    {
        //StartCoroutine(PlayerMoveDoTween());
        var newPos = MapManager.instance.GridToWorld(objb.mapPos);
        playerTransform.position = newPos;
    }

    public IEnumerator PlayerMoveDoTween()
    {
        var newPos = MapManager.instance.GridToWorld(objb.mapPos);
        Tween moveTween = playerTransform.DOMove(newPos, 0.3f).SetEase(Ease.OutQuad);
        yield return moveTween.WaitForCompletion();
    }
}



public enum ObjectType
{
    Player,
    Object101,
    Object102
}
