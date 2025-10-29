using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Pool;
using DG.Tweening;
using System.Threading.Tasks;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public bool isPlayerDead;

    //��ͼ����������ά����
    public ObjectBase[,] mapData;

    //�ײ��ͼ��ǽ��λ��
    public Tilemap map;
    public Tilemap wall;
    public Vector2 sideLength;
    public Vector2 offset;

    //�������
    public GameObject OBJGroup;
    public GameObject functionalGroup;
    public List<ObjectBase> OBJList;

    public ObjectBase player;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
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

            objb.mapPos = new Vector2Int((int)pos.x, (int)pos.y);
            OBJList.Add(objb);
            if (objb.isPlayer)
            {
                player = objb;
                continue;
            }

            objb.oData.mapPos = new Vector2Int(objb.mapPos.x,objb.mapPos.y);
            objH.SizeAdjust();

        }

        //��ȡ������Ʒλ��
        for (int i = 0; i < functionalGroup.transform.childCount; i++)
        {
            var obj = functionalGroup.transform.GetChild(i);
            var pos = WorldToGrid(obj.position);
            var func = obj.GetComponent<FunctionalObject>();

            func.mapPos = pos;
        }


    }


    public void MapReset()
    {
        //��ȡ���λ������
        for (int i = 0; i < OBJGroup.transform.childCount; i++)
        {
            var obj = OBJGroup.transform.GetChild(i);
            ObjectHandler objH = obj.gameObject.GetComponent<ObjectHandler>();
            ObjectBase objb = objH.objb;

            if (objb.isPlayer)
                continue;

            foreach (var pos in objb.gridLock)
            {
                mapData[pos.x, pos.y] = null;
            }
        }

        for (int i = 0; i < OBJGroup.transform.childCount; i++)
        {
            var obj = OBJGroup.transform.GetChild(i);
            ObjectHandler objH = obj.gameObject.GetComponent<ObjectHandler>();
            ObjectBase objb = objH.objb;

            if (objb.isPlayer)
                continue;

            objb.mapPos = objb.oData.mapPos;
            objb.obj.hollow = objb.oData.isHollow;
            objb.obj.width = Mathf.Max(1, objb.oData.size.x);
            objb.obj.height = Mathf.Max(1, objb.oData.size.y);

            for (int j = 0; j < objb.obj.width; j++)
            {
                for (int k = 0; k < objb.obj.height; k++)
                {
                    mapData[objb.mapPos.x + j, objb.mapPos.y + k] = objb;
                    objb.gridLock.Add(new Vector2Int(objb.mapPos.x + j, objb.mapPos.y + k));
                }
            }

            objb.AdjustAll();
        }
    }


    //����ƶ�����
    public void PlayerMove(Vector2Int dir)
    {
        bool isCanMove = true;
        for (int w = 0; w < player.obj.width; w++)
        {
            for (int h = 0; h < player.obj.height; h++)
            {
                Vector2Int checkPos = player.mapPos + dir + new Vector2Int(w, h);

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

        TurnManager.instance.readyToMove = false;

        PlayerPosChange(dir);
        int fall = SimGravity(player);
        if (fall > 1)
        {
            Debug.Log(fall);
            isPlayerDead = true;
        }
            

        if (isPlayerDead)
        {
            PlayerDie(DeadType.DieOfPushY); 
            return;
        }

        TriggerCheck();
        TurnManager.instance.TurnAction();
    }


    //��ҽ�ɫ�ƶ�
    public void PlayerPosChange(Vector2Int dir)
    {
        foreach (var pos in player.gridLock)
        {
            mapData[pos.x, pos.y] = null;
        }

        player.gridLock.Clear();
        player.mapPos += dir;
        for (int j = 0; j < player.obj.width; j++)
        {
            for (int k = 0; k < player.obj.height; k++)
            {
                mapData[player.mapPos.x + j, player.mapPos.y + k] = player;
                player.gridLock.Add(new Vector2Int(player.mapPos.x + j, player.mapPos.y + k));
            }
        }
        player.PlayerMove();
    }


    //�ȴ��������
    public void TriggerCheck()
    {
        ObjectHandler[] handlers = GameObject.FindObjectsOfType<ObjectHandler>();

        foreach (var handler in handlers)
        {
            if (handler.objb == mapData[player.mapPos.x, player.mapPos.y -1])
            {
                if(handler.GetComponent<TriggerObject>()!= null)
                {
                    handler.GetComponent<TriggerObject>().TriggerEffect();
                }
            }
        }

    }

    //���ܶ��󴥷�
    public void FunctionalCheck()
    {
        for (int i = 0; i < functionalGroup.transform.childCount; i++)
        {
            var func = functionalGroup.transform.GetChild(i).GetComponent<FunctionalObject>();
            if (player.mapPos == func.mapPos)
            {
                if(func.type == FunctionalObjectType.Editor)
                {
                    ListsManager.instance.EditorModelTurnOn();
                }
                else
                {
                    LevelManager.instance.Victory();
                }
            }
        }
    }


    //����֮����غϹ������µ�Ч���б�
    public void ActionAdd(List<EffectGroup> effectGroup)
    {
        TurnManager.instance.ActionAdd(effectGroup);
    }


    //ͳһ����������Ҫ�����Ч��
    public void ActionHandler(List<EffectGroup> newEffects)
    {
        foreach (var effect in newEffects)
        {
            for (int i = 0; i < OBJGroup.transform.childCount; i++)
            {
                var obj = OBJGroup.transform.GetChild(i);
                ObjectHandler objH = obj.gameObject.GetComponent<ObjectHandler>();

                if (objH.group != effect.group)
                    continue;

                var tObj = objH.objb;
                EffectHandle(tObj, effect.effect);
            }
        }

    }


    //�������ͷ������������ģ��
    public void EffectHandle(ObjectBase objb, EffectType effect)
    {
        switch (effect)
        {
            case 0:
                break;
            case EffectType.Taller:
                EffectTaller(objb);
                break;
            case EffectType.Shorter:
                EffectShorter(objb);
                break;
            case EffectType.Hollow:
                EffectHollow(objb);
                break;
            case EffectType.Reset:
                EffectReset(objb);
                break;
            case EffectType.MoveLeft:
                EffectMoveLeft(objb);
                break;
            case EffectType.MoveRight:
                EffectMoveRight(objb);
                break;
            default:
                break;
        }
    }


    //����
    public void EffectTaller(ObjectBase objb)
    {
        TryGrowHeight(objb, 1);
        objb.AdjustAll();
    }

    //�½�
    public void EffectShorter(ObjectBase objb)
    {
        objb.obj.height = Mathf.Max(1, objb.obj.height - 1);

        if (objb.obj.hollow)
            return;

        foreach (var pos in objb.gridLock)
        {
            mapData[pos.x, pos.y] = null;
        }

        objb.gridLock.Clear();
        
        for (int i = 0; i < objb.obj.height; i++)
        {
            mapData[objb.mapPos.x, objb.mapPos.y + i] = objb;
            objb.gridLock.Add(new Vector2Int(objb.mapPos.x, objb.mapPos.y + i));
        }

        objb.AdjustAll();
    }

    //��͸��
    public void EffectHollow(ObjectBase objb)
    {
        objb.obj.hollow = true;
        foreach (var pos in objb.gridLock)
        {
            mapData[pos.x, pos.y] = null;
        }
        objb.AdjustAll();
    }

    //һ�л�ԭ
    public void EffectReset(ObjectBase objb)
    {
        objb.obj.hollow = objb.oData.isHollow;
        if(objb.oData.size.y > objb.obj.height)
        {
            int delta = objb.oData.size.y - objb.obj.height;
            TryGrowHeight(objb, delta);
            objb.AdjustAll();
        }
        else if(objb.oData.size.y < objb.obj.height)
        {
            objb.obj.height = Mathf.Max(1, objb.oData.size.y);

            foreach (var pos in objb.gridLock)
            {
                mapData[pos.x, pos.y] = null;
            }

            objb.gridLock.Clear();

            for (int i = 0; i < objb.obj.height; i++)
            {
                mapData[objb.mapPos.x, objb.mapPos.y + i] = objb;
                objb.gridLock.Add(new Vector2Int(objb.mapPos.x, objb.mapPos.y + i));
            }

            objb.AdjustAll();
        }
        else
        {
            foreach (var pos in objb.gridLock)
            {
                mapData[pos.x, pos.y] = null;
            }

            objb.gridLock.Clear();

            for (int i = 0; i < objb.obj.height; i++)
            {
                mapData[objb.mapPos.x, objb.mapPos.y + i] = objb;
                objb.gridLock.Add(new Vector2Int(objb.mapPos.x, objb.mapPos.y + i));
            }
            objb.AdjustAll();
        }
    }

    //���������ƶ�
    public void EffectMoveLeft(ObjectBase objb)
    {
        TryMoveXOne(objb, false);
        objb.AdjustAll();
    }


    //���������ƶ�
    public void EffectMoveRight(ObjectBase objb)
    {
        TryMoveXOne(objb, true);
        objb.AdjustAll();
    }


    //��ҵ�����ģ��
    public int SimGravity(ObjectBase objb)
    {
        int downDelta = 0;
        while (CheckEmpty(new Vector2Int(objb.mapPos.x,objb.mapPos.y - downDelta - 1)))
        {
            downDelta++;
        }

        if(downDelta>0)
        PlayerPosChange(new Vector2Int(0,-downDelta));

        return downDelta;
    }

    //����������
    public void PlayerDie(DeadType type)
    {
        player.PlayerDid(type);
        Debug.Log(123);
    }

    //����Ƿ����
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
    public Vector2Int WorldToGrid(Vector2 pos)
    {
        var newPos = new Vector2Int((int)(pos.x - transform.position.x + offset.x - 0.5f), (int)(pos.y - transform.position.y + offset.y - 0.5f));
        return newPos;
    }


    //����Group������д�����effectlise
    public void SetTrigger(TriggerGroup tGroup, List<EffectGroup> list)
    {
        for (int i = 0; i < OBJGroup.transform.childCount; i++)
        {
            var obj = OBJGroup.transform.GetChild(i);
            ObjectHandler objH = obj.gameObject.GetComponent<ObjectHandler>();

            if (objH.group != tGroup)
                continue;

            if (objH.GetComponent<TriggerObject>() == null)
                continue;

            var tObj = objH.GetComponent<TriggerObject>();
            tObj.effectList = list;
        }

    }

    public Vector2 GridToWorld(Vector2 pos)
    {
        var newPos = new Vector2(pos.x + 0.5f + transform.position.x - offset.x, pos.y + 0.5f + transform.position.y - offset.y);
        return newPos;
    }


    // ������ objb ���ϡ��䳤�� requestedDelta��
    // ����Ϸ����ϰ���ͣ���ϰ��·�������ʵ�����ӵĸ߶�
    public void TryGrowHeight(ObjectBase objb, int requestedDelta)
    {
        bool isPlayerDie = false;

        if (requestedDelta <= 0) return;

        int actualAdded = 0;
        int baseX = objb.mapPos.x;

        // �Ӷ���һ��һ�����Ͽ�
        for (int layer = 1; layer <= requestedDelta; layer++)
        {
            int checkY = objb.mapPos.y + objb.obj.height - 1 + layer;

            // Խ�� �� ֹͣ
            if (!IsInsideGrid(new Vector2Int(baseX, checkY)))
            {
                break;
            }

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
            PlayerDie(DeadType.DieOfPushY);

        // ��������
        if (actualAdded > 0)
        {
            ApplyGrowth(objb, actualAdded);
        }

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
        {
            if (target.isPlayer)
            {
                isPlayerDie = true;
                MoveObjectUp(target, 1);
                return true;
            }

            return false;
        }

        // ���������� �� �ݹ鳢���ȶ�����
        if (TryPushUpOne(occ,out isPlayerDie))
        {
            MoveObjectUp(target, 1);
            return true;
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

        if (objb.obj.hollow)
            return;

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


    //���峢���������ƶ�����������Y��
    public void TryMoveXOne(ObjectBase objb, bool isRight)
    {
        int baseX = objb.mapPos.x;
        int baseY = objb.mapPos.y;
        int topY = baseY + objb.obj.height - 1;

        bool isCarryP = false;
        if (mapData[baseX, topY + 1] != null)
        {
            if (mapData[baseX, topY + 1].isPlayer)
                isCarryP = true;
        }

        bool isSelfCanMove = false;

        List<ObjectBase> waitToPush = new List<ObjectBase>();
        for (int i = 0; i < objb.obj.height; i++)
        {
            Vector2Int checkPos = new Vector2Int(isRight ? baseX + 1 : baseX - 1, baseY + i);
            if (!IsInsideGrid(checkPos))
                return;

            var occ = mapData[checkPos.x,checkPos.y];
            if (occ == null)
            {
                continue;
            }

            if (occ.isWall)
            {
                return;
            }

            // ���������� �� �ݹ鳢���ȶ�����
            waitToPush.Add(occ);
        }

        if (waitToPush.Count == 0)
            isSelfCanMove = true;
        else
        {
            foreach (var item in waitToPush)
            {
                if (!TryPushX(item, isRight))
                {
                    isSelfCanMove = false;
                    break;
                }
                else
                    isSelfCanMove = true;
            }
        }

        if (isCarryP)
            TryPushX(player, isRight);

        if (isSelfCanMove)
        {
            objb.readyToBePush = true;
        }
        
        foreach (var item in OBJList)
        {
            if (item.readyToBePush)
            {
                MoveObjectX(item, isRight);
                
            }

        }

        foreach (var item in OBJList)
        {
            if (item.readyToBePush)
            {
                MoveObjectXWrite(item, isRight);
                item.readyToBePush = false;
            }

        }

    }


    public bool TryPushX(ObjectBase objb, bool isRight)
    {
        int baseX = objb.mapPos.x;
        int baseY = objb.mapPos.y;
        int topY = baseY + objb.obj.height - 1;

        bool isCarryP = false;
        if (mapData[baseX, topY + 1]!=null)
        {
            if(mapData[baseX, topY + 1].isPlayer)
                isCarryP = true;
        }
            

        bool isSelfCanMove = false;
        List<ObjectBase> waitToPush = new List<ObjectBase>();
        for (int i = 0; i < objb.obj.height; i++)
        {
            Vector2Int checkPos = new Vector2Int(isRight ? baseX + 1 : baseX - 1, baseY + i);
            if (!IsInsideGrid(checkPos))
                return false;

            var occ = mapData[checkPos.x, checkPos.y];

            if (occ == null)
            {
                continue;
            }

            if (occ.isWall)
            {
                return false;
            }

            // ���������� �� �ݹ鳢���ȶ�����
            waitToPush.Add(occ);
        }

        if (waitToPush.Count == 0)
            isSelfCanMove = true;
        else
        {
            foreach (var item in waitToPush)
            {
                if (!TryPushX(item, isRight))
                {
                    isSelfCanMove = false;
                    break;
                }
                else
                    isSelfCanMove = true;
            }
        }

        if (isCarryP)
            TryPushX(player, isRight);


        if (isSelfCanMove)
        {
            objb.readyToBePush = true;
            return true;
        }
        else
            return false;

    }

    private void MoveObjectX(ObjectBase target, bool isRight)
    {
        // ��վ�λ��
        foreach (var p in target.gridLock)
        {
            mapData[p.x, p.y] = null;
        }
        target.gridLock.Clear();
        // ����λ��
        target.mapPos = new Vector2Int(target.mapPos.x + (isRight?1:-1), target.mapPos.y);
    }

    private void MoveObjectXWrite(ObjectBase target, bool isRight)
    {
        // д����λ��
        for (int dy = 0; dy < target.obj.height; dy++)
        {
            Vector2Int pos = new Vector2Int(target.mapPos.x, target.mapPos.y + dy);
            mapData[pos.x, pos.y] = target;
            target.gridLock.Add(pos);
        }
        target.AdjustPosition();
    }



}


public enum EffectType
{
    None,
    Taller,
    Shorter,
    Hollow,
    Reset,
    MoveLeft,
    MoveRight
}

public enum DeadType
{
    DieOfPushX,
    DieOfPushY
}