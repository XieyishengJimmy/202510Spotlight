using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColumnController : MonoBehaviour
{
    public ColumnSlot objectIcon;
    public ColumnSlot effectIcon;
    public Button deleteButton;       // �£��Ҳ�ɾ����ť
    private ListPanelController parentPanel;
    private IconPool iconPool;        // ���ڰ� icon ���س�

    public void Init(ListPanelController parent)
    {
        parentPanel = parent;
        deleteButton.onClick.AddListener(OnDeleteClicked);

        // �� IconPool����Ҳ����ͨ��ע�����ã�
        iconPool = FindObjectOfType<IconPool>();
    }

    private void OnDeleteClicked()
    {
        // 1) �������Ϸŵ� icon������У����ص� pool
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

        // 2) ֪ͨ�� panel ɾ���Լ������Ḻ�� Destroy��
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
