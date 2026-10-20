using System;
using UnityEngine;

public class BotCoverPointDetector : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private float coverTime = 5;
    [SerializeField] private float cooldownTime = 15;

    private float _coverTimer;
    private float _cooldownTimer;
    private bool _isInCover;
    private bool _isOnCooldown;

    private void Update()
    {
        if (_isInCover)
        {
            _coverTimer -= Time.deltaTime;
            
            if (_coverTimer <= 0)
                StartCooldown();
        }

        if (_isOnCooldown)
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0)
            {
                _isOnCooldown = false;
                _coverTimer = coverTime;
            }
        }
    }

    private void StartCooldown()
    {
        _isInCover = false;
        _isOnCooldown = true;
        _cooldownTimer = cooldownTime;
    }

    public Transform GetTarget()
    {
        if (_isOnCooldown) 
            return null;
        
        var hits = Physics.OverlapSphere(transform.position, radius, targetLayer);
        
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        
        foreach (var hit in hits)
        {
            float sqr = Vector3.Distance(transform.position, hit.transform.position);

            if (sqr < minDistance)
            {
                minDistance = sqr;
                nearest = hit.transform;
            }
        }

        _isInCover = nearest;
        
        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}