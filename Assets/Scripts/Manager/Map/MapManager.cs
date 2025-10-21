using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Pool;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    //��ͼ����������ά����
    public ObjectBase[,] mapData;

    //�ײ��ͼ��ǽ��λ��
    public Tilemap map;
    public Tilemap wall;
    public Vector2 sideLength;
    public Vector2 offset;

    //�������
    public GameObject OBJGroup;
    public List<ObjectBase> OBJList;

    public ObjectBase player;

    //��ʱ
    public ObjectHandler ob;
    public ObjectHandler oc;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        MapInit();
    }

    //��ͼ���ݳ�ʼ����ȷ��ǽ�������λ��

    public void MapInit()
    {
        //��ȡtilemap�ĳ������ݣ����丳ֵ����ά����
        BoundsInt bounds = map.cellBounds;

        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = map.GetTile(position);
                if (tile != null)
                {
                    // ������С�����߽�
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        // ����������ķ�Χ
        int correctedWidth = maxX - minX + 1;
        int correctedHeight = maxY - minY + 1;

        sideLength.x = correctedWidth;
        sideLength.y = correctedHeight;

        mapData = new ObjectBase[(int)sideLength.x, (int)sideLength.y];

        //���μ��Manager����ԭ�����Tilemap���ֵ�������ԭ�����tilemap���½�0,0���ƫ��ֵ
        for (int i = 0; i < sideLength.x; i++)
        {
            if (!map.HasTile(new Vector3Int(0 - i, 0, 0)))
            {
                offset = new Vector2(i - 1, 0);
                break;
            }
        }

        for (int i = 0; i < sideLength.y; i++)
        {
            if (!map.HasTile(new Vector3Int(0, 0 - i, 0)))
            {
                offset = new Vector2(offset.x, i - 1);
                break;
            }
        }


        //��ȡǽ��λ������
        for (int i = 0; i < sideLength.x; i++)
        {
            for (int j = 0; j < sideLength.y; j++)
            {
                // ����������(i, j)ת��ΪTilemap��ʵ�ʸ�������
                Vector2 worldPos = GridToWorld(new Vector2(i, j));
                Vector3Int tilePos = wall.WorldToCell(worldPos);

                // ����λ���Ƿ���ǽ
                if (wall.HasTile(tilePos))
                {
                    // ��ʱ1=ǽ���������ܴ�ID
                    mapData[i, j] = new ObjectBase();
                    mapData[i, j].isWall = true;
                }
                else
                {
                    // û��ǽ����Ϊ0
                    mapData[i, j] = null;
                }
            }
        }

        OBJList = new List<ObjectBase>();
        //��ȡ���λ������
        for (int i = 0; i < OBJGroup.transform.childCount; i++)
        {
            var obj = OBJGroup.transform.GetChild(i);
            var pos = WorldToGrid(obj.position);

            ObjectHandler objH = obj.gameObject.GetComponent<ObjectHandler>();
            ObjectBase objb = objH.objb;
            objb.Init();

            for (int j = 0; j < objb.obj.width; j++)
            {
                for (int k = 0; k < objb.obj.height; k++)
                {
                    mapData[(int)pos.x+j, (int)pos.y+k] = objb;
                    objb.gridLock.Add(new Vector2Int((int)pos.x + j, (int)pos.y + k));
                }
            }

            if (objb.isPlayer)
                player = objb;

            objb.mapPos = new Vector2Int((int)pos.x, (int)pos.y);
            OBJList.Add(objb);
        }

    }


    //����ƶ�����
    public void PlayerMove(Vector2Int dir, int width, int height)
    {
        if (!TurnManager.instance.readyToMove)
            return;

        bool isCanMove = true;
        ObjectBase objb = player;
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                Vector2Int checkPos = objb.mapPos + dir + new Vector2Int(w, h);

                if(!CheckEmpty(checkPos))
                {
                    if (mapData[checkPos.x, checkPos.y].isPlayer == true)
                        continue;

                    isCanMove = false;
                    break;
                }
            }
        }

        //TODO�������߾Ͳ���
        if (!isCanMove)
            return;

        foreach (var pos in objb.gridLock)
        {
            mapData[pos.x, pos.y] = null;
        }

        objb.gridLock.Clear();
        objb.mapPos += dir;
        for (int j = 0; j < objb.obj.width; j++)
        {
            for (int k = 0; k < objb.obj.height; k++)
            {
                mapData[objb.mapPos.x + j, objb.mapPos.y + k] = objb;
                objb.gridLock.Add(new Vector2Int(objb.mapPos.x + j, objb.mapPos.y + k));
            }
        }
        objb.AdjustPosition();

    }


    //��ʱ
    public void GoOb()
    {
        ActionAdd(ob.objb.effectList);
    }

    public void GoOc()
    {
        ActionAdd(oc.objb.effectList);
    }

    //����֮����غϹ������µ�Ч���б�
    public void ActionAdd(SingleEffect[] effectList)
    {
        TurnManager.instance.ActionAdd(effectList);
    }


    //ͳһ����������Ҫ�����Ч��
    public void ActionHandler(List<SingleEffect> oldEffects,List<SingleEffect> newEffects)
    {
        foreach (var effect in oldEffects)
        {
            EffectHandle(effect.objb, effect.effect, false);
        }

        foreach (var effect in newEffects)
        {
            EffectHandle(effect.objb, effect.effect, true);
        }

    }


    //����id�������������ģ��
    public void EffectHandle(ObjectBase objb, int effect, bool isNewEffect)
    {
        switch (effect)
        {
            case 0:
                break;
            case 201:
                Effect201(objb, isNewEffect);
                break;
            case 202:
                Effect202(objb, isNewEffect);
                break;
            case 203:
                Effect203(objb, isNewEffect);
                break;
            default:
                break;
        }
    }


    public void Effect201(ObjectBase objb, bool isNewEffect)
    {
        int oldHeight = objb.obj.height;
        int newHeight = isNewEffect ? oldHeight * 2 : Mathf.Max(1, oldHeight / 2);

        foreach (var pos in objb.gridLock)
        {
            mapData[pos.x, pos.y] = null;
        }

        objb.gridLock.Clear();

        if(!isNewEffect)
        {
            for (int i = 0; i < newHeight; i++)
            {
                mapData[objb.mapPos.x, objb.mapPos.y + i] = objb;
                objb.gridLock.Add(new Vector2Int(objb.mapPos.x, objb.mapPos.y + i));
            }
            
            objb.obj.height = newHeight;
            objb.AdjustAll();

            return;
        }

        int delta = newHeight - oldHeight;

        int added = TryGrowHeight(objb, delta);
        
        objb.AdjustAll();
    }

    public void Effect202(ObjectBase objb, bool isNewEffect)
    {
        if (isNewEffect)
            objb.obj.height = objb.obj.height / 2 >= 1 ? objb.obj.height / 2 : 1;
        else
            objb.obj.height = objb.obj.height * 2;
    }

    public void Effect203(ObjectBase objb, bool isNewEffect)
    {
        if (isNewEffect)
            objb.obj.color = 1;
        else
            objb.obj.color = 0;
    }


    //��ҵ�����ģ��
    public void SimGravity(ObjectBase objb)
    {
        Debug.Log(objb);
        int downDelta = 0;
        while (CheckEmpty(new Vector2Int(objb.mapPos.x,objb.mapPos.y - downDelta - 1)))
        {
            downDelta++;
            Debug.Log(12654);
        }

        PlayerMove(new Vector2Int(0,-downDelta), player.obj.width, player.obj.height);
    }



    public bool IsInsideGrid(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < mapData.GetLength(0) && p.y < mapData.GetLength(1);
    }


    //���Ŀ��λ�õĿ�����
    public bool CheckEmpty(Vector2Int checkPos)
    {
        if (!IsInsideGrid(checkPos)) 
            return false;

        return mapData[checkPos.x, checkPos.y] == null;
    }


    //����ϵ��������������ת��
    public Vector2 WorldToGrid(Vector2 pos)
    {
        var newPos = new Vector2(pos.x - transform.position.x + offset.x - 0.5f, pos.y - transform.position.y + offset.y - 0.5f);
        return newPos;
    }

    public Vector2 GridToWorld(Vector2 pos)
    {
        var newPos = new Vector2(pos.x + 0.5f + transform.position.x - offset.x, pos.y + 0.5f + transform.position.y - offset.y);
        return newPos;
    }


    // ������ objb ���ϡ��䳤�� requestedDelta��
    // ����Ϸ����ϰ���ͣ���ϰ��·�������ʵ�����ӵĸ߶�
    public int TryGrowHeight(ObjectBase objb, int requestedDelta)
    {
        bool isPlayerDie = false;

        if (requestedDelta <= 0) return 0;

        int actualAdded = 0;
        int baseX = objb.mapPos.x;

        // �Ӷ���һ��һ�����Ͽ�
        for (int layer = 1; layer <= requestedDelta; layer++)
        {
            int checkY = objb.mapPos.y + objb.obj.height - 1 + layer;

            // Խ�� �� ֹͣ
            if (!IsInsideGrid(new Vector2Int(baseX, checkY)))
                break;

            var occ = mapData[baseX, checkY];

            // �� �� ���Լ���
            if (occ == null)
            {
                actualAdded++;
                continue;
            }

            // ��ǽ �� ֹͣ
            if (occ.isWall)
                break;

            // ���������� �� ���԰����������϶�һ�񣨵ݹ飩
            if (TryPushUpOne(occ, out isPlayerDie))
            {
                actualAdded++;
            }
            else
            {
                // ������ �� ֹͣ
                break;
            }
        }

        if (isPlayerDie)
            Debug.Log("You dead��");

        // ��������
        if (actualAdded > 0)
        {
            ApplyGrowth(objb, actualAdded);
        }

        return actualAdded;
    }


    // ���԰�һ�������������϶� 1 ��
    // ���Ϸ��п�λ���ܵݹ鶥���Ϸ����壬��ɹ����� true
    private bool TryPushUpOne(ObjectBase target,out bool isPlayerDie)
    {
        isPlayerDie = false;
        int baseX = target.mapPos.x;
        int topY = target.mapPos.y + target.obj.height - 1;
        int checkY = topY + 1;

        if (!IsInsideGrid(new Vector2Int(baseX, checkY)))
            return false;

        var occ = mapData[baseX, checkY];

        if (occ == null)
        {
            // �Ϸ��ǿյģ�����ֱ������
            MoveObjectUp(target, 1);
            return true;
        }

        if (occ.isWall)
            return false;

        // ���������� �� �ݹ鳢���ȶ�����
        if (TryPushUpOne(occ,out isPlayerDie))
        {
            MoveObjectUp(target, 1);
            return true;
        }
        else
        {
            if (occ.isPlayer)
                isPlayerDie = true;
        }

        return false;
    }


    // ����������߶����� actualAdded �񣨻��㲻�䣩
    private void ApplyGrowth(ObjectBase objb, int actualAdded)
    {
        // �����ռλ
        foreach (var p in objb.gridLock)
        {
            mapData[p.x, p.y] = null;
        }
        objb.gridLock.Clear();

        // ���¸߶�
        objb.obj.height += actualAdded;

        // ����ע��ռλ
        for (int dy = 0; dy < objb.obj.height; dy++)
        {
            Vector2Int pos = new Vector2Int(objb.mapPos.x, objb.mapPos.y + dy);
            mapData[pos.x, pos.y] = objb;
            objb.gridLock.Add(pos);
        }
    }


    // ͨ�õ������ƶ����庯�������ָ߶Ȳ��䣩
    private void MoveObjectUp(ObjectBase target, int amount)
    {
        // ��վ�λ��
        foreach (var p in target.gridLock)
        {
            mapData[p.x, p.y] = null;
        }
        target.gridLock.Clear();

        // ����λ��
        target.mapPos = new Vector2Int(target.mapPos.x, target.mapPos.y + amount);
        target.AdjustPosition();

        // д����λ��
        for (int dy = 0; dy < target.obj.height; dy++)
        {
            Vector2Int pos = new Vector2Int(target.mapPos.x, target.mapPos.y + dy);
            mapData[pos.x, pos.y] = target;
            target.gridLock.Add(pos);
        }
    }

}
