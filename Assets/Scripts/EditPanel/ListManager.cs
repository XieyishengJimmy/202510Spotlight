using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListsManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform listsContainer;      // Horizontal or Vertical container that holds ListPanels
    public GameObject listPanelPrefab;

    [Header("限制设置")]
    public int maxTotalItems = 10;        // 在 Inspector 可修改

    // 注册所有 ListPanelController
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

    // ---------- 获取当前全局条目数 ----------
    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var p in panels) total += p.GetColumnCount();
        return total;
    }

    // ---------- 检查是否能新增一个条目（列） ----------
    public bool CanAddNewItem()
    {
        if (GetTotalItemCount() >= maxTotalItems)
        {
            Debug.LogWarning($"已达到最大条目数（{maxTotalItems}），无法再添加。");
            return false;
        }
        return true;
    }
}
