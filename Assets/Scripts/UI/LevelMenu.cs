using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;
    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        ButtonsToArray();
        for(int i = 0; i <buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        for (int i = 0; i < Mathf.Min(unlockedLevel, buttons.Length); i++)
        {
            buttons[i].interactable = true;
        }
        //reset lại màn chơi
       // PlayerPrefs.DeleteAll();
    }
    public void OpenLevel(int levelId)
    {
        string levelName = "Level_" + levelId;
        SceneManager.LoadScene(levelName);
    }

    void ButtonsToArray()
    {
        int childCount = levelButtons.transform.childCount;
        buttons = new Button[childCount];
        for(int i = 0; i< childCount; i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}
