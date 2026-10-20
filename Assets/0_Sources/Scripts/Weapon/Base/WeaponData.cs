using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/WeaponData", fileName = "Weapon")] 
public class WeaponData : ScriptableObject
{
   [field: SerializeField] public int Damage { get; private set; } = 10;
   [field: SerializeField] public float TimeReload { get; private set; } = 10;
   [field: SerializeField] public float SpeedProjectile { get; private set; } = 10;
   [field: SerializeField] public float LiveTimeProjectile { get; private set; } = 10;
   [field: SerializeField] public bool FireSimultaneously  { get; private set; }
   [field: SerializeField] public float ShotInterval { get; private set; } = 0.1f;
   [field: SerializeField] public AudioClip[] ShotAudios { get; private set; }
   [field: Space]
   [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
}
