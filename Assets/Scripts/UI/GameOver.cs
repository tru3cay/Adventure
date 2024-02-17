using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject OverMenu;

    public void Over()
    {
        OverMenu.SetActive(true);
    }
    public void goHome()
    {
        SceneManager.LoadScene("MainMenu");
        OverMenu.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        OverMenu.SetActive(false);
        PlayerMovements.Instance.ResetHealth(); // Đặt lại máu của người chơi
    }

}
