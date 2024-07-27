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

    private int _currentCheckpoint = 1;
    private List<PlayerController> _currentPlayers = new List<PlayerController>();

    public virtual void Awake()
    {
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
}
