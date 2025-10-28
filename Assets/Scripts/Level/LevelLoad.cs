using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour
{
    public int levelIndex;
    public string levelName;

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelIndex + 1);
    }

    public void LoadLevelByName()
    {
        SceneManager.LoadScene(levelName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
