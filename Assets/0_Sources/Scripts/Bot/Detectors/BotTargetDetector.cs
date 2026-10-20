using System;
using UnityEngine;

public class BotTargetDetector : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private TeamProfile teamProfile;

    private void Start()
    {
        if(teamProfile == null)
            teamProfile = GetComponentInChildren<TeamProfile>();
    }
 
    public float Radius => radius;

    public Transform GetTarget()
    {
        if(teamProfile == null) return null; 
        
        var hits = Physics.OverlapSphere(transform.position, radius, targetLayer);
        
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        
        foreach (var hit in hits)
        {
            float sqr = Vector3.Distance(transform.position, hit.transform.position);

            if (sqr < minDistance)
            {
                if(hit.gameObject == this.gameObject) continue;
                
                if (hit.TryGetComponent(out TeamProfile hitTeam))
                {
                    if(hitTeam.MyTeam == teamProfile.MyTeam)
                        continue;
                }
                
                minDistance = sqr;
                nearest = hit.transform;
            }
        }
        
        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}