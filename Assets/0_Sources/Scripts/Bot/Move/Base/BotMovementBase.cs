using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class BotMovementBase : MonoBehaviour
{
   [SerializeField] private float moveSpeed = 5f;
   [SerializeField] private Transform pointOfInterest;
   [SerializeField] private float interestRadius = 10;
   
   protected Vector3 RandomPoint;
   protected Vector3 SavePosition;
   protected NavMeshAgent Agent;

   private const float MinDistanceSwitchPoint = 0.4f;
   
   protected virtual void Start()
   {
      Agent = GetComponent<NavMeshAgent>();
      Agent.speed = moveSpeed;
      SavePosition = transform.position;  
      GenerateRandomPoint();
   }

   public virtual void MoveTo(Vector3 position)
   {
      if(Agent)
       Agent.SetDestination(position);
   }

   public virtual void Retreat()
   {     
      if(Agent)
       Agent.SetDestination(SavePosition);
   }

   public virtual void MoveToRandomPointNearInterest()
   {
      if (pointOfInterest == null)
      {
         Debug.LogWarning("No pointOfInterest found");
         return;
      }

      if (Agent == null)
      {
         Debug.LogWarning("No agent found");
         return;
      }

      if (Vector3.Distance(transform.position, RandomPoint) > Agent.stoppingDistance + MinDistanceSwitchPoint)
         MoveTo(RandomPoint);
      else
         GenerateRandomPoint();
   }

   protected void GenerateRandomPoint()
   {
      if (pointOfInterest == null)
      {
         var currentLevel = FindObjectOfType<LevelProfile>();
         if (currentLevel)
         {
            pointOfInterest = currentLevel.PointOfInterest;
         }
         else
         {
            Debug.LogWarning("No pointOfInterest found");
            return;
         }
      }

      Vector3 randomOffset = Random.insideUnitSphere * interestRadius;
      randomOffset.y = 0;
      RandomPoint =  pointOfInterest.position + randomOffset;
   }

   public void SetPointOfInterest(Transform point) => pointOfInterest = point;
   public Transform GetPointOfInterest() => pointOfInterest;
   
   private void OnDrawGizmosSelected()
   {
      if (pointOfInterest == null) return;
      
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(pointOfInterest.position, interestRadius);
   }
}
