using System.Collections.Generic;
using UnityEngine;

public class FootstepSFXTrigger : MonoBehaviour
{
    public LayerMask FootstepOnLayer;

    protected List<Collider> touchingColliders = new List<Collider>();

    public AudioClip Footstep;

    public float FootstepDelay;

    [SerializeField]
    private AudioSource _audioSource;

    private float _remainingDelay;
    private bool _isTrigger;

    public void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            _isTrigger = collider.isTrigger;
        }
    }

    public void Update()
    {
        if (_remainingDelay >= 0)
        {
            _remainingDelay -= Time.deltaTime;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_isTrigger) return;

        OnTouchGround(collision.collider);
    }

    public void OnCollisionExit(Collision collision)
    {
        if (_isTrigger) return;

        OnLeaveGround(collision.collider);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!_isTrigger) return;

        OnTouchGround(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!_isTrigger) return;

        OnLeaveGround(other);
    }

    public bool isLog = false;

    protected virtual void OnTouchGround(Collider other)
    {
        if ((FootstepOnLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            if (isLog) Debug.Log($"TEST ----- Foot {transform.name} touched {other.gameObject.name}");

            if (touchingColliders.Count == 0)
            {
                PlayFootstep();
            }

            touchingColliders.Add(other);
        }
    }

    protected virtual void OnLeaveGround(Collider other)
    {
        if ((FootstepOnLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            if (isLog) Debug.Log($"TEST ----- Foot {transform.name} left {other.gameObject.name}");

            if (touchingColliders.Contains(other))
            {
                touchingColliders.Remove(other);
            }
        }
    }

    protected void PlayFootstep()
    {
        if (_remainingDelay > 0) return;

        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(Footstep);

            _remainingDelay = FootstepDelay;
        }
    }
}
