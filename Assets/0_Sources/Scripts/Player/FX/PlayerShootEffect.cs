using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerShootEffect : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;
    [SerializeField] private ParticleSystem shootEffect;
    [FormerlySerializedAs("amlitude")]
    [Space] 
    [SerializeField] private float amplitudeShake = 1.3f;
    [SerializeField] private float durationShake = 0.3f;
    
    private AudioFX _audioFX;
    
    private void OnEnable() =>  weapon.ShootEvent += StartEffect;
    private void OnDisable() => weapon.ShootEvent -= StartEffect;

    private void Start() => _audioFX = GetComponentInChildren<AudioFX>();
    
    private void StartEffect()
    {
        CameraShakeController.Instance.Shake(amplitudeShake, durationShake);
        shootEffect.Play();
        var shotAudios = weapon.Weapon.ShotAudios;
        _audioFX.PlayAndRandomPitch(shotAudios[Random.Range(0, shotAudios.Length)]);
    }
}
