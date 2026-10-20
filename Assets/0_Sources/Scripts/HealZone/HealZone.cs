using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class HealZone : MonoBehaviour
{
   [SerializeField] private TeamData team;
   [SerializeField] private float healPerTick = 5;
   [SerializeField] private float tickRate = 0.5f;

   private HashSet<IUnitHealth> _targets = new();

   private void Start()
   {
      GetComponent<Collider>().isTrigger = true;
      GetComponent<Rigidbody>().isKinematic = true;
      StartCoroutine(HealTick());
   }

   private IEnumerator HealTick()
   {
      while (true)
      {
         yield return new WaitForSeconds(tickRate);

         foreach (var target in _targets)
            target.AddHealth(healPerTick);
      }
   }

   private void OnTriggerEnter(Collider other)
   {
      if(!other.TryGetComponent(out TeamProfile teamProfile)) return;
      if(teamProfile.MyTeam != team) return;
      
      if(other.TryGetComponent(out IUnitHealth health))
         _targets.Add(health);
   }

   private void OnTriggerExit(Collider other)
   {  
      if(other.TryGetComponent(out IUnitHealth health))
       _targets.Remove(health);
   }
}
