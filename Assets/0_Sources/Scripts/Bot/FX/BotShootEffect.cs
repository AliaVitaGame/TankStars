using UnityEngine;
using Random = UnityEngine.Random;

public class BotShootEffect : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;
    [SerializeField] private ParticleSystem shootEffect;
    
    private AudioFX _audioFX;
    
    private void OnEnable() =>  weapon.ShootEvent += StartEffect;
    private void OnDisable() => weapon.ShootEvent -= StartEffect;

    private void Start() => _audioFX = GetComponentInChildren<AudioFX>();
    
    private void StartEffect()
    {
        shootEffect.Play();
        var shotAudios = weapon.Weapon.ShotAudios;
        _audioFX.PlayAndRandomPitch(shotAudios[Random.Range(0, shotAudios.Length)]);
    }
}
