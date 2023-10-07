using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PlayerInputHandler _inputHandler;
    Animator _animator;

    void Start()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        _inputHandler.IsInteracting = _animator.GetBool("IsInteracting");
    }
}
