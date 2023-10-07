using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator _animator;
    private int _verticalHash;
    private int _horizontalHash;

    public void Initialise()
    {
        _animator = GetComponent<Animator>();
        _verticalHash = Animator.StringToHash("Vertical");
        _horizontalHash = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimation(float vertical, float horizontal)
    {
        float clampVertical = vertical switch
        {
            > 0.55f => 1f,
            > 0f => 0.5f,
            < -0.55f => -1f,
            < 0f => -0.5f,
            _ => 0f
        };
        _animator.SetFloat(_verticalHash, clampVertical, 0.1f, Time.deltaTime);


        float clampHorizontal = horizontal switch
        {
            > 0.55f => 1f,
            > 0f => 0.5f,
            < -0.55f => -1f,
            < 0f => -0.5f,
            _ => 0f
        };
        _animator.SetFloat(_horizontalHash, clampHorizontal, 0.1f, Time.deltaTime);
    }
}
