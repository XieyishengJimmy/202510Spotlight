using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionalObject : MonoBehaviour
{
    public FunctionalObjectType type;
    public Vector2Int mapPos;
}


public enum FunctionalObjectType
{
    Editor,
    Destination
}
