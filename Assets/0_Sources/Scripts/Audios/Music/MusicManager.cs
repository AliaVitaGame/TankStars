using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musics;
    
    private AudioSource _audioSource;

    private void Start() 
        => _audioSource = GetComponent<AudioSource>();

    private void FixedUpdate()
    {
        if (_audioSource.isPlaying) return;
        
        var musicID = Random.Range(0, musics.Length);
        _audioSource.PlayOneShot(musics[musicID]);
    }
}
