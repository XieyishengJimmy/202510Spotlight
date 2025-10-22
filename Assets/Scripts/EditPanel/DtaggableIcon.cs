using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public enum IconType
{ 
    Object,
    Effect,
}

public class DraggableIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int effectId;
    public ObjectHandler objh;
    public ObjectBase objb;
    public IconType iconType;
    Canvas canvas;
    RectTransform rect;
    CanvasGroup cg;
    Transform originalParent;

    public int maxCount;
    public int availableCount;
    public TextMeshProUGUI countBadge;
    public GameObject bg;

    [HideInInspector] public bool isSource = true;  // Դͼ�꣨����ģ�
    [HideInInspector] public bool isGhost = false;  // ��ק����Ӱ
    [HideInInspector] public bool canDrag = true;   // �Ƿ�������ק������ʵ��Ϊ false��
    [HideInInspector] public DraggableIcon sourceIcon; // ����ʵ������Ӱָ��Դͼ��

    // ��������ǰ��קʱ����Ӱ���ã�ֻ��Դͼ����ʹ�ã�
    GameObject dragGhost;
    RectTransform ghostRect;

    public void Awake()
    {
        rect = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        var c = GetComponentInParent<Canvas>();
        canvas = c != null ? c.rootCanvas : null;

        availableCount = maxCount;

        UpdateBadge();
    }

    private void Start()
    {
        if(iconType == IconType.Object)
        objb = objh.objb;
    }

    public void UpdateBadge()
    {
        if (countBadge != null)
        {
            countBadge.text = availableCount.ToString();
            // �ɸ�����Ҫ�ı���ɫ������
            countBadge.color = availableCount > 0 ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        }
    }

    public void ConfigureAsPlaced(DraggableIcon source)
    {
        isSource = false;
        isGhost = false;
        canDrag = false;
        sourceIcon = source;
        if (countBadge != null) bg.gameObject.SetActive(false);
    }

    public void ReturnOneToSource()
    {
        if (sourceIcon != null)
        {
            sourceIcon.availableCount++;
            sourceIcon.UpdateBadge();
        }
    }

    public void DestroyGhost()
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
            ghostRect = null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (!isSource) return;              // ��Դͼ��ɷ�����ק
        if (availableCount <= 0) return;    // û�������Ͳ�������ק
        if (canvas == null) return;

        // ������Ӱ����¡����
        dragGhost = Instantiate(gameObject, canvas.transform, false);
        var ghostIcon = dragGhost.GetComponent<DraggableIcon>();
        ghostIcon.isGhost = true;
        ghostIcon.isSource = false;
        ghostIcon.canDrag = true;
        ghostIcon.sourceIcon = this;

        var ghostCg = dragGhost.GetComponent<CanvasGroup>();
        if (ghostCg != null)
        {
            ghostCg.blocksRaycasts = false; // ������Ӱ�赲Ͷ�����������
            ghostCg.alpha = 0.75f;          // ��͸��Ч��
        }
        // ������Ӱ�ĽǱ꣬������
        if (ghostIcon.countBadge != null) ghostIcon.bg.gameObject.SetActive(false);

        ghostRect = dragGhost.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostRect == null || canvas == null) return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos);
        ghostRect.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyGhost();
    }
}
