using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Playing, Paused, GameOver }
    public GameState State { get; private set; } = GameState.Playing;

    PauseGame pauseGame;
    PlayerController playerController;
    CameraFollow cameraFollow;

    [Header("Game Over Cinematic")]
    [SerializeField] float slowTimeScale = 0.3f;
    [SerializeField] float cinematicDuration = 2f;
    [SerializeField] float cinematicZoomTarget = 3f;
    [SerializeField] float cinematicFOVTarget = 3f;
    [SerializeField] float cinematicPanSpeed = 2f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pauseGame = FindFirstObjectByType<PauseGame>();
        playerController = FindFirstObjectByType<PlayerController>();
        cameraFollow = FindFirstObjectByType<CameraFollow>();

        Transform playerTransform = PartyManager.instance.player.transform;
        Transform heroTransform = PartyManager.instance.hero.transform;
        PartyManager.instance.player.GetComponent<CharacterStats>().OnDeath += () => TriggerGameOver(playerTransform);
        PartyManager.instance.hero.GetComponent<CharacterStats>().OnDeath += () => TriggerGameOver(heroTransform);
    }

    void Update()
    {
        if (State == GameState.GameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GameState.Paused)
                SetState(GameState.Playing);
            else
                SetState(GameState.Paused);
        }
    }

    void TriggerGameOver(Transform dyingUnit)
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;
        StartCoroutine(GameOverCinematic(dyingUnit));
    }

    IEnumerator GameOverCinematic(Transform dyingUnit)
    {
        playerController.DisableInput();
        Time.timeScale = slowTimeScale;
        cameraFollow.StartCinematic(dyingUnit, cinematicZoomTarget, cinematicFOVTarget, cinematicPanSpeed);

        yield return new WaitForSecondsRealtime(cinematicDuration);

        Time.timeScale = 0f;
        pauseGame.DeathGameOver();
    }

    // Called by PauseGame when resumed via UI button, so State stays in sync
    public void NotifyResumed()
    {
        if (State == GameState.Paused)
            State = GameState.Playing;
    }

    void SetState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.Playing:
                pauseGame.Resume();
                break;
            case GameState.Paused:
                pauseGame.Pause();
                break;
        }
    }
}
