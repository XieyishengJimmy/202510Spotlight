using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListPanelController : MonoBehaviour
{
    public RectTransform columnsContent;   // VerticalLayoutGroup 的容器（放 Column prefab）
    public Button addColumnButton;
    public GameObject columnPrefab;

    public TriggerGroup group;

    private ListsManager manager;
    private List<ColumnController> columns = new List<ColumnController>();


    private void Start()
    {
        manager = FindObjectOfType<ListsManager>();
        manager.RegisterPanel(this);
        addColumnButton.onClick.AddListener(OnAddColumn);
    }

    // 初始化由 ListsManager 调用或在 Start 中获取
    public void Init(ListsManager mgr)
    {
        manager = mgr;
        manager.RegisterPanel(this);
        addColumnButton.onClick.AddListener(OnAddColumn);
    }

    private void OnDestroy()
    {
        if (manager != null) manager.UnregisterPanel(this);
    }

    // 新：返回该 panel 的列数（用于 manager 总数统计）
    public int GetColumnCount() => columns.Count;

    public void OnAddColumn()
    {
        // manager 是否允许新增
        if (manager != null && !manager.CanAddNewItem())
        {
            return;
        }

        if (columns.Count > 0)
        {
            var lastCol = columns[columns.Count - 1];
            bool hasObject = lastCol.objectIcon != null && lastCol.objectIcon.IsOccupied;
            bool hasEffect = lastCol.effectIcon != null && lastCol.effectIcon.IsOccupied;

            // 如果上一个没放满，就不允许继续新增
            if (!(hasObject && hasEffect))
            {
                Debug.Log("上一个列未放满，禁止新增！");
                return;
            }
        }

        // 实例化列，并插入到 addButton 之前（保持按钮在末尾）
        var go = Instantiate(columnPrefab, columnsContent);
        go.transform.SetSiblingIndex(addColumnButton.transform.GetSiblingIndex());
        var ctrl = go.GetComponent<ColumnController>();
        ctrl.Init(this); // 将当前 panel 引入到 column（以便删除时回调）
        columns.Add(ctrl);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(columnsContent as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

        CallRefreshBotton();

    }

    // 在 ColumnController 删除时调用
    public void RemoveColumn(ColumnController column)
    {
        if (columns.Contains(column))
        {
            columns.Remove(column);
            Destroy(column.gameObject);
            // VerticalLayoutGroup 会自动让下面的上移
        }
        CallRefreshBotton();
    }

    public void CallRefreshBotton()
    {
        if (manager != null)
        {
            manager.RefreshAllAddButtons();
        }
    }

    public void UpdateAddButtonState()
    {
        bool canAdd = true;
        if (manager != null && !manager.CanAddNewItem())
        {
            canAdd = false;
        }

        if (columns.Count > 0)
        {
            var lastCol = columns[columns.Count - 1];
            bool hasObject = lastCol.objectIcon != null && lastCol.objectIcon.IsOccupied;
            bool hasEffect = lastCol.effectIcon != null && lastCol.effectIcon.IsOccupied;
            if (!(hasObject && hasEffect))
            {
                canAdd = false;
            }
        }

        addColumnButton.GetComponent<Image>().color = canAdd? Color.green:Color.gray;

    }

    public void EffectTransform()
    {
        MapManager.instance.SetTrigger(group, ExportData());
    }


    // 导出数据
    public List<EffectGroup> ExportData()
    {
        var groupList = new List<EffectGroup>();
        for (int i = 0; i < columns.Count; i++)
        {
            var eGroup = new EffectGroup();
            var columnData = columns[i].ToData(i);
            eGroup.group = columnData.group;
            eGroup.effect = columnData.effectType;

            groupList.Add(eGroup);
        }
        return groupList;
    }


}
