using System.Collections;
using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public float HealPotency = 30f;
    public GameObject HealSpellPF;
    private PlayerAnimationHandler _animatorHandler;
    private PlayerSoundManager _soundManager;

    [SerializeField]
    private PlayerStatus _playerStatus;

    public void Initialise()
    {
        _animatorHandler = GetComponent<PlayerAnimationHandler>();
        _soundManager = GetComponent<PlayerSoundManager>();
    }

    public void CastHeal()
    {
        _soundManager.PlayHealSound();

        // Generate the heal circle, then let a coroutine handle the heal behaviour
        var healCircle = Instantiate(HealSpellPF, transform.position, Quaternion.identity, transform);
        StartCoroutine(HealCircleFlow(healCircle));
    }

    /// <summary>
    /// Couroutine that controls heal-over-time behaviour
    /// </summary>
    /// <param name="healCircle"></param>
    /// <returns></returns>
    public IEnumerator HealCircleFlow(GameObject healCircle)
    {
        var totalDuration = 3f;
        var timeElapsed = 0f;
        var despawning = false;

        healCircle.transform.localScale = Vector3.zero;

        // Over 3 seconds, make the healing circle grow in size, and heal by ticks
        while (timeElapsed < totalDuration)
        {
            if (healCircle != null)
                healCircle.transform.localScale = Vector3.Slerp(healCircle.transform.localScale, Vector3.one / 2, Time.deltaTime);

            _playerStatus?.RestoreHealth(HealPotency * (Time.deltaTime / totalDuration));

            timeElapsed += Time.deltaTime;

            // Start fading out the particles midway through (only do this once).
            if (timeElapsed > 1.3f && !despawning)
            {
                despawning = true;
                healCircle?.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            yield return null;
        }        
    }
}
