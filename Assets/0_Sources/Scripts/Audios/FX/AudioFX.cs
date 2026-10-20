using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class AudioFX : MonoBehaviour
{
    private AudioSource _audioSource;

    private float _startPitch = 1;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _startPitch = _audioSource.pitch;
    }

    public void Play(AudioClip clip)
    {
        _audioSource.pitch = _startPitch;
        _audioSource.PlayOneShot(clip);
    }

    public void PlayAndRandomPitch(AudioClip clip, float pitchMin = 0.9f, float pitchMax = 1.1f)
    {
        var pitch = Random.Range(pitchMin,  pitchMax);
        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(clip);
    }
}
