using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody _rb;
    public float Lifetime;
    public float Speed;

    private float _remainingTime;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _remainingTime = Lifetime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.AddForce(transform.forward.normalized * Speed);

        _remainingTime -= Time.fixedDeltaTime;
        if (_remainingTime <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
