using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListsManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform listsContainer;      // Horizontal or Vertical container that holds ListPanels
    public GameObject listPanelPrefab;

    [Header("��������")]
    public int maxTotalItems = 10;        // �� Inspector ���޸�

    // ע������ ListPanelController
    private List<ListPanelController> panels = new List<ListPanelController>();

    private void Start()
    {
        
    }
    public void RegisterPanel(ListPanelController panel)
    {
        if (!panels.Contains(panel)) panels.Add(panel);
    }

    public void UnregisterPanel(ListPanelController panel)
    {
        if (panels.Contains(panel)) panels.Remove(panel);
    }

    // ---------- ��ȡ��ǰȫ����Ŀ�� ----------
    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var p in panels) total += p.GetColumnCount();
        return total;
    }

    // ---------- ����Ƿ�������һ����Ŀ���У� ----------
    public bool CanAddNewItem()
    {
        if (GetTotalItemCount() >= maxTotalItems)
        {
            Debug.LogWarning($"�Ѵﵽ�����Ŀ����{maxTotalItems}�����޷�����ӡ�");
            return false;
        }
        return true;
    }
}
