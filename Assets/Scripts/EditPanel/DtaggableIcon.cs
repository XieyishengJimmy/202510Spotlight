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

    [HideInInspector] public bool isSource = true;  // 源图标（池里的）
    [HideInInspector] public bool isGhost = false;  // 拖拽的虚影
    [HideInInspector] public bool canDrag = true;   // 是否允许拖拽（放置实例为 false）
    [HideInInspector] public DraggableIcon sourceIcon; // 放置实例或虚影指向源图标

    // 新增：当前拖拽时的虚影引用（只在源图标上使用）
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
            // 可根据需要改变颜色或显隐
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
        if (!isSource) return;              // 仅源图标可发起拖拽
        if (availableCount <= 0) return;    // 没有数量就不允许拖拽
        if (canvas == null) return;

        // 生成虚影（克隆自身）
        dragGhost = Instantiate(gameObject, canvas.transform, false);
        var ghostIcon = dragGhost.GetComponent<DraggableIcon>();
        ghostIcon.isGhost = true;
        ghostIcon.isSource = false;
        ghostIcon.canDrag = true;
        ghostIcon.sourceIcon = this;

        var ghostCg = dragGhost.GetComponent<CanvasGroup>();
        if (ghostCg != null)
        {
            ghostCg.blocksRaycasts = false; // 避免虚影阻挡投放区域的射线
            ghostCg.alpha = 0.75f;          // 半透明效果
        }
        // 隐藏虚影的角标，避免误导
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
