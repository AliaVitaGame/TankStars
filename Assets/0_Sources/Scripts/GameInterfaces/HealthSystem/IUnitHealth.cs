using System;
using UnityEngine;

public interface IUnitHealth
{
   public float Health { get; }
   public float MaxHealth { get; }
   
   public event Action<GameObject> OnDeathEvent;
   public event Action<KillInfo> OnDeathContextEvent;

   public void Resurrect();
   public void AddHealth(float value);
   public void TakeDamage(float damage, GameObject attacker);
   public void Die(GameObject attacker);
}

public class KillInfo
{
   public GameObject Victim { get; }
   public GameObject Killer { get; }

   public KillInfo(GameObject victim, GameObject killer)
   {
      Victim = victim;
      Killer = killer;
   }
}

