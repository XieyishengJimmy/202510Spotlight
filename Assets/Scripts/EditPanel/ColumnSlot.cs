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

        // ������ӡ�Դͼ�ꡱ���루����ģ�����ֹ���ѷ���ʵ����ק
        if (!source.isSource) return;

        if (source.iconType != slotType) return;

        // û�������Ͳ�����Ͷ��
        if (source.availableCount <= 0) return;

        if (IsOccupied)
        {
            // �黹�ɵ�����
            placedIcon.ReturnOneToSource();

            // ���پɵķ���ʵ��
            Destroy(placedIcon.gameObject);
            placedIcon = null;
        }

        // �ڲ�λ�д���һ��������ʵ����
        var placedGO = GameObject.Instantiate(source.gameObject, this.transform, false);
        var placed = placedGO.GetComponent<DraggableIcon>();
        placed.ConfigureAsPlaced(source);
        placed.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        placedIcon = placed;

        // Դͼ������ -1 �����½Ǳ�
        source.availableCount--;
        source.UpdateBadge();

        // Դͼ����������Ӱ
        source.DestroyGhost();
    }

    // �� ColumnController ���ã�ǿ���Ƴ���ǰͼ�겢���� pool
    public void ForceRemoveAndReturnToPool(IconPool pool)
    {
        if (placedIcon == null) return;

        // �黹������Դ
        placedIcon.ReturnOneToSource();

        // ���ٷ���ʵ��
        Destroy(placedIcon.gameObject);
        placedIcon = null;
    }

    // �����ⲿ������������ icon �Լ������ slot.RemovePlacedIcon��
    public void RemovePlacedIcon()
    {
        if (placedIcon != null)
        {
            var pool = FindObjectOfType<IconPool>();
            ForceRemoveAndReturnToPool(pool);
        }
    }
}
