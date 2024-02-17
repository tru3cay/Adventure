using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockNewLevel();
            // go to next level
            if (SceneController.instance != null)
            {
                int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextLevelIndex == 6) // Kiểm tra xem màn chơi hiện tại có phải là màn chơi thứ 5 không
                {
                    SceneManager.LoadScene("MainMenu"); // Chuyển đến cảnh mainmenu
                }
                else
                {
                    SceneController.instance.NextLevel();
                    // Thêm âm thanh khi người chơi đi vào cảnh mới
                    audioManager.PlaySFX(audioManager.transitionn);
                }
            }
            else
            {
                Debug.LogError("SceneController.instance is null");
            }
        }
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
