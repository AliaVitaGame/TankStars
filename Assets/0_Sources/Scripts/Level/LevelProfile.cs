using UnityEngine;

public class LevelProfile : MonoBehaviour
{
   [SerializeField] private Transform pointOfInterest;

   public Transform PointOfInterest => pointOfInterest;
}
