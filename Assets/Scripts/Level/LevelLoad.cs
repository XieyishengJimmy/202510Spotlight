using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour
{
    public int levelIndex;

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelIndex + 1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
