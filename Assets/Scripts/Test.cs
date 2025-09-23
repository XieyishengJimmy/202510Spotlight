using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int word = 1001;

    public void PushButton()
    {
        Debug.Log(ConfigManager.Instance.GetConfig<WordConfig>(word).desc);
        word = ConfigManager.Instance.GetConfig<WordConfig>(word).nextid;
    }
}
