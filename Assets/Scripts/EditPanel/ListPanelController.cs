using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListPanelController : MonoBehaviour
{
    public RectTransform columnsContent;   // VerticalLayoutGroup ���������� Column prefab��
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

    // ��ʼ���� ListsManager ���û��� Start �л�ȡ
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

    // �£����ظ� panel ������������ manager ����ͳ�ƣ�
    public int GetColumnCount() => columns.Count;

    public void OnAddColumn()
    {
        // manager �Ƿ���������
        if (manager != null && !manager.CanAddNewItem())
        {
            return;
        }

        // ʵ�����У������뵽 addButton ֮ǰ�����ְ�ť��ĩβ��
        var go = Instantiate(columnPrefab, columnsContent);
        go.transform.SetSiblingIndex(addColumnButton.transform.GetSiblingIndex());
        var ctrl = go.GetComponent<ColumnController>();
        ctrl.Init(this); // ����ǰ panel ���뵽 column���Ա�ɾ��ʱ�ص���
        columns.Add(ctrl);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(columnsContent as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

    }

    // �� ColumnController ɾ��ʱ����
    public void RemoveColumn(ColumnController column)
    {
        if (columns.Contains(column))
        {
            columns.Remove(column);
            Destroy(column.gameObject);
            // VerticalLayoutGroup ���Զ������������
        }
    }

    public void EffectTransform()
    {
        trigger.effectList = ExportData();
    }


    // ��������
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
