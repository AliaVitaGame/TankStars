using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankController : BaseVehicleController
{
    [SerializeField] private float moveSpeed = 150; 
    [SerializeField] private float rotationSpeedModel = 7;
    [SerializeField] private float rotationSpeedTurret = 7;
    [SerializeField] private Transform model;
    [SerializeField] private Transform turret;
    
    private Rigidbody _rb;

    private void Start()
    {
        _rb  = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
        RotateTurret();
    }

    private void Move()
    {
        var invertedValueZ = IsInvertedZ ? -1 : 1;
        var invertedValueX = IsInvertedX ? -1 : 1;
        Vector3 movement = new Vector3(MoveInput.x * invertedValueX, 0, MoveInput.y * invertedValueZ) * GetMoveSpeed();
        _rb.linearVelocity = new Vector3(movement.x, _rb.linearVelocity.y, movement.z);

        var lookDirection = movement;
        
        if(movement.sqrMagnitude > 0.001f)
         model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(lookDirection), rotationSpeedModel * Time.fixedDeltaTime);
    }

    private void RotateTurret()
    {
        Vector3 mouseWorld = CameraScreenToWorldPoint.Instance.GetMousePosition();
        
        Vector3 lookDirection = (mouseWorld - turret.position).normalized;
        lookDirection.y = 0;
        
        turret.rotation = Quaternion.Lerp(turret.rotation, Quaternion.LookRotation(lookDirection), rotationSpeedTurret * Time.fixedDeltaTime);
    }
    
    private float GetMoveSpeed() =>  moveSpeed * Time.fixedDeltaTime;
}
