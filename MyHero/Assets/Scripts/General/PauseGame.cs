using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public static bool GameIsPaused;
    public GameObject pauseMenuUI;
    public GameObject buttonToHide;
    public GameObject deathScreen;
    bool gameIsOver;

    // Update is called once per frame
    void Update()
    {
        if (!gameIsOver)
        { 
            if (PartyManager.instance.player.GetComponent<CharacterStats>().isDead || PartyManager.instance.hero.GetComponent<CharacterStats>().isDead)
            {
                DeathGameOver();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {   
                if (GameIsPaused)
                    {
                        Resume();
                    }
                else
                    {
                        Pause();
                    }
            }            
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        //buttonToHide.SetActive(true);
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
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
        
        {
            pauseMenuUI.SetActive(true);
            deathScreen.SetActive(true);
            Time.timeScale = 0f;
            gameIsOver = true;
            buttonToHide.SetActive(false);
        }
    }
}
