using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<Sprite> guideGroup;

    public Image guideImage;

    public int index = 0;

    private void Awake()
    {
        instance = this;
    }

    public void Victory()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void Reload()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void ToTheMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenTheGuide()
    {
        if(index < guideGroup.Count)
        {
            guideImage.gameObject.SetActive(true);
            guideImage.sprite = guideGroup[index];
            index++;
        }
        else
        {
            guideImage.gameObject.SetActive(false);
            index = 0;
        }
    }




}
