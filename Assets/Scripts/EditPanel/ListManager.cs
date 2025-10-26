using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListsManager : MonoBehaviour
{
    public static ListsManager instance;

    [Header("UI References")]
    public Transform listsContainer;      // Horizontal or Vertical container that holds ListPanels
    public GameObject listPanelPrefab;

    [Header("限制设置")]
    public int maxTotalItems = 10;        // 在 Inspector 可修改
    public GameObject mask;
    public GameObject iconPanel;

    // 注册所有 ListPanelController
    private List<ListPanelController> panels = new List<ListPanelController>();

    public bool isEditing;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EditorModelTurnOff();
    }

    public void EditorModelTurnOn()
    {
        isEditing = true;
        mask.SetActive(false);
        iconPanel.SetActive(true);
        MapManager.instance.MapReset();
        TurnManager.instance.EffectClear();
    }

    public void EditorModelTurnOff()
    {
        isEditing = false;
        mask.SetActive(true);
        iconPanel.SetActive(false);

        foreach (var list in panels)
        {
            list.EffectTransform();
        }

    }

    public void RegisterPanel(ListPanelController panel)
    {
        if (!panels.Contains(panel)) panels.Add(panel);
    }

    public void UnregisterPanel(ListPanelController panel)
    {
        if (panels.Contains(panel)) panels.Remove(panel);
    }

    public void RefreshAllAddButtons()
    {
        foreach (var list in panels)
        {
            list.UpdateAddButtonState();
        }
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
            //Debug.LogWarning($"已达到最大条目数（{maxTotalItems}），无法再添加。");
            return false;
        }
        return true;
    }
}
