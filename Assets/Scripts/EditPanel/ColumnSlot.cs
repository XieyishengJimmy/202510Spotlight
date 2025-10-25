using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColumnSlot : MonoBehaviour, IDropHandler
{
    public IconType slotType;
    public DraggableIcon placedIcon;

    public bool IsOccupied => placedIcon != null;

    public void OnDrop(PointerEventData eventData)
    {
        var d = eventData.pointerDrag;
        if (d == null) return;

        var source = d.GetComponent<DraggableIcon>();
        if (source == null) return;

        // 仅允许从“源图标”拖入（池里的），禁止从已放置实例拖拽
        if (!source.isSource) return;

        if (source.iconType != slotType) return;

        // 没有数量就不允许投放
        if (source.availableCount <= 0) return;

        if (IsOccupied)
        {
            // 归还旧的数量
            placedIcon.ReturnOneToSource();

            // 销毁旧的放置实例
            Destroy(placedIcon.gameObject);
            placedIcon = null;
        }

        // 在槽位中创建一个“放置实例”
        var placedGO = GameObject.Instantiate(source.gameObject, this.transform, false);
        var placed = placedGO.GetComponent<DraggableIcon>();
        placed.ConfigureAsPlaced(source);
        placed.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        placedIcon = placed;

        // 源图标数量 -1 并更新角标
        source.availableCount--;
        source.UpdateBadge();

        // 源图标销毁其虚影
        source.DestroyGhost();
    }

    // 由 ColumnController 调用，强制移除当前图标并返回 pool
    public void ForceRemoveAndReturnToPool(IconPool pool)
    {
        if (placedIcon == null) return;

        // 归还数量给源
        placedIcon.ReturnOneToSource();

        // 销毁放置实例
        Destroy(placedIcon.gameObject);
        placedIcon = null;
    }

    // 允许外部撤销（例如点击 icon 自己会调用 slot.RemovePlacedIcon）
    public void RemovePlacedIcon()
    {
        if (placedIcon != null)
        {
            var pool = FindObjectOfType<IconPool>();
            ForceRemoveAndReturnToPool(pool);
        }
    }
}
