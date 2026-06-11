using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public static bool GameIsPaused;
    public GameObject pauseMenuUI;
    public GameObject buttonToHide;
    public GameObject deathScreen;

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameManager.instance?.NotifyResumed();
        //buttonToHide.SetActive(true);
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        EventSystem.current.SetSelectedGameObject(null);
        //buttonToHide.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
        GameIsPaused = false;
           
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        GameIsPaused = false;
        Time.timeScale = 1f;
    
    }
    public void DeathGameOver()
    {
        pauseMenuUI.SetActive(true);
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
        buttonToHide.SetActive(false);
    }
}
