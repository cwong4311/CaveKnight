using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody _rb;
    public float Lifetime;
    public float Speed;
    public float Tracking;
    public float Damage;
    public GameObject ExplosionPF;

    private float _remainingTime;
    private Transform _target;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _remainingTime = Lifetime;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.velocity = transform.forward.normalized * Speed;
        _rb.velocity += transform.up.normalized * -1;  // Downward force

        _remainingTime -= Time.fixedDeltaTime;
        RotateProjectile();

        if (_remainingTime <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void RotateProjectile()
    {
        if (_target != null)
        {
            var direction = Quaternion.LookRotation(_target.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.deltaTime * Tracking);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") 
            || collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            var damageDealt = DealDamage(collision);
            if (damageDealt)
            {
                Explode();
                Destroy(gameObject);
            }
        }
    }

    private bool DealDamage(Collider collision)
    {
        if (IsDamageable(collision) && Damage > 0.01f)
        {
            if (collision.gameObject.TryGetComponent<PlayerHealth>(out var player))
            {
                if (player.IsInvulnerable)
                {
                    return false;
                }

                player.TakeDamage(Damage);
            }
            // Else maybe a destructable. Handle later
        }

        return true;
    }

    private void Explode()
    {
        if (ExplosionPF != null)
        {
            Instantiate(ExplosionPF, transform.position, Quaternion.identity);
        }
    }

    public bool IsDamageable(Collider collision)
    {
        if (collision.tag == "Player" || collision.tag == "Enemy" || collision.tag == "Destructable")
        {
            return true;
        }

        return false;
    }
}
