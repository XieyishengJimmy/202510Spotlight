using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListPanelController : MonoBehaviour
{
    public RectTransform columnsContent;   // VerticalLayoutGroup ���������� Column prefab��
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

        if (columns.Count > 0)
        {
            var lastCol = columns[columns.Count - 1];
            bool hasObject = lastCol.objectIcon != null && lastCol.objectIcon.IsOccupied;
            bool hasEffect = lastCol.effectIcon != null && lastCol.effectIcon.IsOccupied;

            // �����һ��û�������Ͳ������������
            if (!(hasObject && hasEffect))
            {
                Debug.Log("��һ����δ��������ֹ������");
                return;
            }
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

        CallRefreshBotton();

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


    // ��������
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
