using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColumnController : MonoBehaviour
{
    public ColumnSlot objectIcon;
    public ColumnSlot effectIcon;
    public Button deleteButton;       // 新：右侧删除按钮
    private ListPanelController parentPanel;
    private IconPool iconPool;        // 用于把 icon 返回池

    public void Init(ListPanelController parent)
    {
        parentPanel = parent;
        deleteButton.onClick.AddListener(OnDeleteClicked);

        // 找 IconPool（你也可以通过注入引用）
        iconPool = FindObjectOfType<IconPool>();
    }

    private void OnDeleteClicked()
    {
        // 1) 将本列上放的 icon（如果有）返回到 pool
        if (objectIcon != null && objectIcon.IsOccupied)
        {
            var icon = objectIcon.placedIcon;
            objectIcon.ForceRemoveAndReturnToPool(iconPool);
        }
        if (effectIcon != null && effectIcon.IsOccupied)
        {
            var icon = effectIcon.placedIcon;
            effectIcon.ForceRemoveAndReturnToPool(iconPool);
        }

        // 2) 通知父 panel 删除自己（父会负责 Destroy）
        parentPanel.RemoveColumn(this);
    }


    public ColumnData ToData(int index)
    {
        return new ColumnData
        {
            columnIndex = index,
            group = objectIcon.IsOccupied ? objectIcon.placedIcon.group : TriggerGroup.None,
            effectType = effectIcon.IsOccupied ? effectIcon.placedIcon.effectType : EffectType.None
        };
    }
}
