using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListPanelController : MonoBehaviour
{
    public RectTransform columnsContent;   // VerticalLayoutGroup 的容器（放 Column prefab）
    public Button addColumnButton;
    public GameObject columnPrefab;

    public TriggerObject trigger;

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

        // 实例化列，并插入到 addButton 之前（保持按钮在末尾）
        var go = Instantiate(columnPrefab, columnsContent);
        go.transform.SetSiblingIndex(addColumnButton.transform.GetSiblingIndex());
        var ctrl = go.GetComponent<ColumnController>();
        ctrl.Init(this); // 将当前 panel 引入到 column（以便删除时回调）
        columns.Add(ctrl);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(columnsContent as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

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
    }

    public void EffectTransform()
    {
        trigger.effectList = ExportData();
    }


    // 导出数据
    public List<SingleEffect> ExportData()
    {
        var list = new List<SingleEffect>();
        for (int i = 0; i < columns.Count; i++)
        {
            var sEffect = new SingleEffect();
            var columnData = columns[i].ToData(i);
            sEffect.objb = columnData.objb;
            sEffect.effect = columnData.effectId;

            list.Add(sEffect);
        }
        return list;
    }


}
