using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public int playerWidth = 1;
    public int playerheight = 2;

    private void Update()
    {
        if(TurnManager.instance.readyToMove)
        GetKeys();
    }

    public void GetKeys()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayerControl(PlayerAction.W);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            PlayerControl(PlayerAction.S);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerControl(PlayerAction.A);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerControl(PlayerAction.D);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerControl(PlayerAction.Sp);
        }
    }

    public void PlayerControl(PlayerAction key)
    {
        if(key == PlayerAction.Sp)
        {

        }
        else
        {
            switch (key)
            {
                case PlayerAction.W:
                    MapManager.instance.PlayerMove(new Vector2Int(0,1),playerWidth,playerheight);
                    break;
                case PlayerAction.S:
                    MapManager.instance.PlayerMove(new Vector2Int(0, -1), playerWidth, playerheight);
                    break;
                case PlayerAction.A:
                    MapManager.instance.PlayerMove(new Vector2Int(-1, 0), playerWidth, playerheight);
                    break;
                case PlayerAction.D:
                    MapManager.instance.PlayerMove(new Vector2Int(1, 0), playerWidth, playerheight);
                    break;
                default:
                    break;
            }
        }
    }

}
