using UnityEngine;

public class RandomSoundPlayer: MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _potentialSounds;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private bool DestroyAfterAudioComplete;

    public void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void Start()
    {
        if (_audioSource == null) return;
        if (_potentialSounds == null || _potentialSounds.Length <= 0) return;

        var randomIndex = Random.Range(0, _potentialSounds.Length);

        _audioSource.clip = _potentialSounds[randomIndex];
        _audioSource.Play();

        var length = _potentialSounds[randomIndex].length;
        Destroy(this, length + 0.01f);
    }
}
