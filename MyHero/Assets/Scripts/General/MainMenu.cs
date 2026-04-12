using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject cutSceneCanvas;
    public AudioClip mainMenuTheme;
    public AudioClip cutSceneAudio;
    public Camera mainCamera;
    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void PlayCutScene()
    {
        cutSceneCanvas.SetActive(true);
        mainCamera.GetComponent<AudioSource>().Stop();
        mainCamera.GetComponent<AudioSource>().PlayOneShot(cutSceneAudio);
        StartCoroutine(CutsceneDelay());
    }
    IEnumerator CutsceneDelay()
    {
        yield return new WaitForSeconds(cutSceneAudio.length);
        PlayGame();
    }
}
