using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public GameObject[] Checkpoints;
    public Animator CheckpointReachedAnim;

    public float GameTimeElapsed => Time.time - _timeOfCurrentGameStart;
    private float _timeOfCurrentGameStart;

    private int _currentCheckpoint = 0;
    private List<PlayerController> _currentPlayers = new List<PlayerController>();

    public virtual void Awake()
    {
        ResetGameTime();

        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this);
    }

    public void OnApplicationQuit()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(this);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentPlayers = FindObjectsOfType<PlayerController>().ToList();
        foreach (var player in _currentPlayers)
        {
            SpawnAtCheckpoint(player);
        }
    }

    public void SpawnAtCheckpoint(PlayerController player)
    {
        if (Checkpoints != null && Checkpoints.Length > _currentCheckpoint)
        {
            player.transform.position = Checkpoints[_currentCheckpoint].transform.position;

            if (player.CameraController == null) return;
            player.CameraController.transform.position = Checkpoints[_currentCheckpoint].transform.position;
        }
    }

    public void UpdateCheckpoint(int checkpointID)
    {
        if (checkpointID <= _currentCheckpoint) return;
        if (checkpointID >= Checkpoints.Length) return;

        _currentCheckpoint = checkpointID;

        if (CheckpointReachedAnim != null)
        {
            CheckpointReachedAnim.SetTrigger("Checkpoint");
        }
    }

    public void ResetCheckpoint()
    {
        _currentCheckpoint = 0;
    }

    public void ResetGameTime()
    {
        _timeOfCurrentGameStart = -1f;
    }

    public void StartGameTime()
    {
        if (_timeOfCurrentGameStart < 0f)
        {
            _timeOfCurrentGameStart = Time.time;
        }
    }
}
