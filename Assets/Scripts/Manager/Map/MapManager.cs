using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Pool;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    //地图物件管理储存二维数组
    public ObjectBase[,] mapData;

    //底层地图，墙体位置
    public Tilemap map;
    public Tilemap wall;
    public Vector2 sideLength;
    public Vector2 offset;

    //物件集合
    public GameObject OBJGroup;
    public List<ObjectBase> OBJList;

    public ObjectBase player;

    //临时
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

    //地图数据初始化，确认墙体和物体位置

    public void MapInit()
    {
        //读取tilemap的长宽数据，将其赋值给二维数组
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
                    // 更新最小和最大边界
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        // 计算修正后的范围
        int correctedWidth = maxX - minX + 1;
        int correctedHeight = maxY - minY + 1;

        sideLength.x = correctedWidth;
        sideLength.y = correctedHeight;

        mapData = new ObjectBase[(int)sideLength.x, (int)sideLength.y];

        //依次检查Manager坐标原点距离Tilemap外的值，来获得原点基于tilemap左下角0,0点的偏移值
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


        //获取墙体位置数据
        for (int i = 0; i < sideLength.x; i++)
        {
            for (int j = 0; j < sideLength.y; j++)
            {
                // 将数组坐标(i, j)转换为Tilemap的实际格子坐标
                Vector2 worldPos = GridToWorld(new Vector2(i, j));
                Vector3Int tilePos = wall.WorldToCell(worldPos);

                // 检查该位置是否有墙
                if (wall.HasTile(tilePos))
                {
                    // 暂时1=墙，后续可能存ID
                    mapData[i, j] = new ObjectBase();
                    mapData[i, j].isWall = true;
                }
                else
                {
                    // 没有墙体则为0
                    mapData[i, j] = null;
                }
            }
        }

        OBJList = new List<ObjectBase>();
        //获取物件位置数据
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


    //玩家移动控制
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

        //TODO：不能走就颤抖
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


    //临时
    public void GoOb()
    {
        ActionAdd(ob.objb.effectList);
    }

    public void GoOc()
    {
        ActionAdd(oc.objb.effectList);
    }

    //触发之后给回合管理器新的效果列表
    public void ActionAdd(SingleEffect[] effectList)
    {
        TurnManager.instance.ActionAdd(effectList);
    }


    //统一处理所有需要处理的效果
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


    //根据id分配给各个处理模块
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


    //玩家的重力模拟
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


    //检查目标位置的可用性
    public bool CheckEmpty(Vector2Int checkPos)
    {
        if (!IsInsideGrid(checkPos)) 
            return false;

        return mapData[checkPos.x, checkPos.y] == null;
    }


    //坐标系坐标和世界坐标的转换
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


    // 尝试让 objb 向上“变长” requestedDelta格
    // 如果上方有障碍则停在障碍下方，返回实际增加的高度
    public int TryGrowHeight(ObjectBase objb, int requestedDelta)
    {
        bool isPlayerDie = false;

        if (requestedDelta <= 0) return 0;

        int actualAdded = 0;
        int baseX = objb.mapPos.x;

        // 从顶部一层一层往上看
        for (int layer = 1; layer <= requestedDelta; layer++)
        {
            int checkY = objb.mapPos.y + objb.obj.height - 1 + layer;

            // 越界 → 停止
            if (!IsInsideGrid(new Vector2Int(baseX, checkY)))
                break;

            var occ = mapData[baseX, checkY];

            // 空 → 可以继续
            if (occ == null)
            {
                actualAdded++;
                continue;
            }

            // 是墙 → 停止
            if (occ.isWall)
                break;

            // 是其他物体 → 尝试把它整体往上顶一格（递归）
            if (TryPushUpOne(occ, out isPlayerDie))
            {
                actualAdded++;
            }
            else
            {
                // 顶不上 → 停止
                break;
            }
        }

        if (isPlayerDie)
            Debug.Log("You dead！");

        // 真正更新
        if (actualAdded > 0)
        {
            ApplyGrowth(objb, actualAdded);
        }

        return actualAdded;
    }


    // 尝试把一个物体整体往上顶 1 格
    // 若上方有空位或能递归顶走上方物体，则成功返回 true
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
            // 上方是空的，可以直接上移
            MoveObjectUp(target, 1);
            return true;
        }

        if (occ.isWall)
            return false;

        // 是其他物体 → 递归尝试先顶起它
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


    // 真正让物体高度增加 actualAdded 格（基点不变）
    private void ApplyGrowth(ObjectBase objb, int actualAdded)
    {
        // 清除旧占位
        foreach (var p in objb.gridLock)
        {
            mapData[p.x, p.y] = null;
        }
        objb.gridLock.Clear();

        // 更新高度
        objb.obj.height += actualAdded;

        // 重新注册占位
        for (int dy = 0; dy < objb.obj.height; dy++)
        {
            Vector2Int pos = new Vector2Int(objb.mapPos.x, objb.mapPos.y + dy);
            mapData[pos.x, pos.y] = objb;
            objb.gridLock.Add(pos);
        }
    }


    // 通用的向上移动物体函数（保持高度不变）
    private void MoveObjectUp(ObjectBase target, int amount)
    {
        // 清空旧位置
        foreach (var p in target.gridLock)
        {
            mapData[p.x, p.y] = null;
        }
        target.gridLock.Clear();

        // 更新位置
        target.mapPos = new Vector2Int(target.mapPos.x, target.mapPos.y + amount);
        target.AdjustPosition();

        // 写入新位置
        for (int dy = 0; dy < target.obj.height; dy++)
        {
            Vector2Int pos = new Vector2Int(target.mapPos.x, target.mapPos.y + dy);
            mapData[pos.x, pos.y] = target;
            target.gridLock.Add(pos);
        }
    }

}
