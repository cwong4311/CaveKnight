using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameLogicManager : MonoBehaviour
{
    public Animator Menu;
    public GameObject PauseScreen;
    public PlayerController Player;

    public AudioSource MenuAudio;
    public AudioClip GameOverSound;
    public AudioMixerSnapshot DefaultMixerSnapshot;
    public AudioMixerSnapshot GameOverMixerSnapshot;
    public AudioMixerSnapshot PausedMixerSnapshot;

    public GameObject EnemyPool;

    public TextMeshProUGUI FinalScore;
    public TextMeshProUGUI TimeTaken;
    public TextMeshProUGUI EnemiesSlain;

    public static Action OnPause;
    public static Action OnGameOver;
    public static Action OnLevelComplete;

    public static bool IsPaused;

    private Coroutine _currentGameEndScreen;
    private float _originalTimeScale;
    private int _enemiesAtGameStart;

    public void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        IsPaused = false;
        _currentGameEndScreen = null;

        PlayMenuAnimation("Intro");

        if (MenuAudio == null) MenuAudio = GetComponent<AudioSource>();
        DefaultMixerSnapshot.TransitionTo(.01f);

        _enemiesAtGameStart = EnemyPool.GetComponentsInChildren<EnemyHealth>().Length;

        OnGameOver += PlayGameOverScreen;
        OnLevelComplete += PlayLevelCompletedScreen;
        OnPause += OnPausePressed;
    }

    public void OnDisable()
    {
        OnGameOver = null;
        OnLevelComplete = null;
        OnPause = null;
    }

    protected void PlayGameOverScreen()
    {
        if (_currentGameEndScreen != null) return;

        // Play gameover sound immediately. Includes Player onDeath
        MenuAudio?.PlayOneShot(GameOverSound);
        GameOverMixerSnapshot.TransitionTo(3f);

        // Display GameEnd anim after 2 second
        StartCoroutine(
            PerformAfterDelay(2f, 
            () => {
                PlayMenuAnimation("GameOver"); 
            })
        );

        //Reload the scene after GameOver anim finishes playing
        _currentGameEndScreen = StartCoroutine(
            PerformAfterDelay(9f,
            () => ReloadLevel())
        );
    }

    protected void PlayLevelCompletedScreen()
    {
        if (_currentGameEndScreen != null) return;

        _currentGameEndScreen = StartCoroutine(
            PerformAfterDelay(5,
            () => {
                OnPause = null;
                PausedMixerSnapshot.TransitionTo(5f);
                SetWinScreenStats();
                PlayMenuAnimation("YouWin");

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            })
        );
    }

    private void PlayMenuAnimation(string animName)
    {
        Menu?.SetTrigger(animName);
    }

    private IEnumerator PerformAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected void SetWinScreenStats()
    {
        float t = Time.timeSinceLevelLoad;
        float milliseconds = (Mathf.Floor(t * 100) % 100); // calculate the milliseconds for the timer
        int seconds = (int)(t % 60); // return the remainder of the seconds divide by 60 as an int
        t /= 60; // divide current time y 60 to get minutes
        int minutes = (int)(t % 60); //return the remainder of the minutes divide by 60 as an int
        t /= 60; // divide by 60 to get hours
        int hours = (int)(t % 24); // return the remainder of the hours divided by 60 as an int

        string timeTaken = "";
        if (hours > 0)
        {
            timeTaken = hours.ToString() + "h " + minutes.ToString() + "m " + seconds.ToString() + "s " + milliseconds.ToString() + "ms ";
        }
        else if (minutes > 0)
        {
            timeTaken = minutes.ToString() + "m " + seconds.ToString() + "s " + milliseconds.ToString() + "ms ";
        }
        else
        {
            timeTaken = seconds.ToString() + "s " + milliseconds.ToString() + "ms ";
        }
        TimeTaken.text = timeTaken;

        var enemiesAtGameEnd = EnemyPool.GetComponentsInChildren<EnemyHealth>().Length - 1; //Boss won't get destroyed
        var enemiesKilled = _enemiesAtGameStart - enemiesAtGameEnd;
        EnemiesSlain.text = enemiesKilled.ToString();

        var baseScore = 100;
        baseScore -= (enemiesAtGameEnd / 2);
        baseScore -= ((hours * 60) + (minutes * 1));

        var finalScore = baseScore switch
        {
            >90 => "SS",
            >80 => "S",
            >70 => "A",
            >60 => "B",
            >50 => "C",
            _ => "D"
        };
        FinalScore.text = finalScore;
    }

    protected void OnPausePressed()
    {
        var isNowPaused = !IsPaused;
        if (isNowPaused)
        {
            Pause();
        }
        else
        {
            UnPause();
        }
    }

    public void Pause()
    {
        if (IsPaused) return;
        if (_currentGameEndScreen != null) return;

        IsPaused = true;

        _originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PauseScreen.SetActive(true);

        PausedMixerSnapshot.TransitionTo(.01f);
    }

    public void UnPause()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = _originalTimeScale;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PauseScreen.SetActive(false);

        DefaultMixerSnapshot.TransitionTo(0.5f);
    }

    public void ReturnToMainMenu()
    {

    }
}
