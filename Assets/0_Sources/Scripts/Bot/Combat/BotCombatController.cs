using System;
using UnityEngine;

public class BotCombatController : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private WeaponPoolObject poolObject;
    [SerializeField] private Transform turret;
    [SerializeField] private LayerMask obstacleLayer; 
    [SerializeField] private float speedRotation = 10;
    [SerializeField] private float stopDistance = 12;

    private Transform _currentTarget;
    private BotTargetDetector _botTargetDetector;
    private BotMovementBase _movement;

    private void Start()
    {
        weaponController.SetPoolProjectile(poolObject);
    }

    public void Attack(Transform target)
    {
        _currentTarget = target;
        Shot();
        RotateToTarget();
    }

    private void Shot()
    {
        if(IsTargetVisible())
         weaponController.StartShoot();
    }

    public void MoveToTarget()
    {
        if(_currentTarget ==null) return;
        
        bool isStopped = Vector3.Distance(transform.position, _currentTarget.position) < stopDistance && IsTargetVisible();
        var movePosition = isStopped  ? transform.position : _currentTarget.position;
        
        _movement.MoveTo(movePosition);
    }

    private void RotateToTarget()
    {      
        if(_currentTarget ==null) return;
        VehicleSurfaceAlignment.RotateTurretTowards(turret, _currentTarget.position, speedRotation, Time.deltaTime);
    }

    public void ResetRotate()
    {
        Vector3 point = turret.position + transform.forward;
        VehicleSurfaceAlignment.RotateTurretTowards(turret, point, speedRotation, Time.deltaTime);
    }

    private bool IsTargetVisible()
    { 
        if(_currentTarget ==null) return false;

        if (Physics.Raycast(turret.position, turret.forward, out var hit, _botTargetDetector.Radius, obstacleLayer))
           return hit.transform == _currentTarget;
        
        return false;
    }
    
    public void Initialization(BotMovementBase movement, BotTargetDetector botTargetDetector)
    {
        _movement = movement;
        _botTargetDetector = botTargetDetector;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        
        if(turret == null) return;
        
        Gizmos.color = Color.chartreuse;
        Gizmos.DrawRay(turret.position, turret.forward * float.MaxValue); 
    }
}