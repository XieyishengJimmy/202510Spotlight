using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuidePic : MonoBehaviour, IPointerClickHandler
{
    public LevelManager lm;
    public void OnPointerClick(PointerEventData eventData)
    {
        lm.OpenTheGuide();
    }
}
