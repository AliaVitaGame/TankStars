using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{ 
   [SerializeField] private Collider mainCollider;
   [SerializeField] private WeaponData weapon;
   [SerializeField] private Transform[] shotPoints;

   public event Action ShootEvent; 
   
   private WeaponPoolObject _poolProjectile;
   private bool _isReloading;
   
   public WeaponData Weapon => weapon;

   private void Start()
   {
       _isReloading = false;
       StopAllCoroutines();
   }

   public void StartShoot()
   {
     if(_isReloading) return;
     if(gameObject.activeSelf == false) return;
     
     StartCoroutine(Shoot()); 
   }
   
   private IEnumerator Shoot()
   {
       _isReloading = true;

       if (weapon.FireSimultaneously)
       {
           foreach (var projectile in shotPoints)
               SpawnProjectile(projectile);
       }
       else
       {
           foreach (var projectile in shotPoints)
           {
               SpawnProjectile(projectile);
               yield return new WaitForSeconds(weapon.ShotInterval);
           }
       }
       
       yield return new WaitForSeconds(weapon.TimeReload);
       _isReloading = false;
   }

   private void SpawnProjectile(Transform shotPoint)
   {
       var projectile = _poolProjectile.GetProjectile(shotPoint.position, shotPoint.rotation);
       projectile.SetStats(weapon, shotPoint.forward, mainCollider);
       ShootEvent?.Invoke();
   }

   public void SetPoolProjectile(WeaponPoolObject poolProjectile)
   {
       _poolProjectile = poolProjectile;
       _poolProjectile.SetProjectile(weapon.ProjectilePrefab);
   }
}
