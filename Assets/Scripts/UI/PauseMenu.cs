using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject paseMenu;
    public void Pause()
    {
        paseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        paseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void Resume()
    {
        paseMenu.SetActive(false);
        Time.timeScale = 1;

    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        paseMenu.SetActive(false);
        PlayerMovements.Instance.ResetHealth(); // Đặt lại máu của người chơi
    }
}
