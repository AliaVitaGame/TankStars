using System.Collections.Generic;
using UnityEngine;

public class WeaponPoolObject : MonoBehaviour
{
   [SerializeField] private Transform container;

   private Projectile _projectilePrefab;
   private List<Projectile> _poolProjectile = new List<Projectile>();
   
   public Projectile GetProjectile(Vector3 position, Quaternion rotation)
   {
      for (int i = 0; i < _poolProjectile.Count; i++)
      {
         if (_poolProjectile[i].gameObject.activeSelf == false)
         {
            _poolProjectile[i].gameObject.SetActive(true);
            _poolProjectile[i].transform.position = position;
            _poolProjectile[i].transform.rotation = rotation;
            return _poolProjectile[i];
         }
      }
      
      var projectile = Instantiate(_projectilePrefab, position, rotation, container);
      _poolProjectile.Add(projectile);
      return projectile;
   }
   
   public void SetProjectile(Projectile projectile) 
      => _projectilePrefab = projectile;
}
