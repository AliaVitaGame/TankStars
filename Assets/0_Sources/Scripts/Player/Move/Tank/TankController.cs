using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankController : BaseVehicleController
{
    [SerializeField] private float moveSpeed = 150; 
    [SerializeField] private float rotationSpeedModel = 7;
    [SerializeField] private float rotationSpeedTurret = 7;
    [SerializeField] private Transform model;
    [SerializeField] private Transform turret;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundSampleRadius = 0.9f;
    [SerializeField] private float groundRayHeight = 1.6f;
    [SerializeField] private float groundRayLength = 4f;
    
    private Rigidbody _rb;
    private LayerMask _groundMask;

    private void Start()
    {
        _rb  = GetComponent<Rigidbody>();
        _groundMask = VehicleSurfaceAlignment.ResolveGroundMask(groundMask);
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

        if (model == null)
            return;

        Vector3 yawForward = movement.sqrMagnitude > 0.001f
            ? movement
            : Vector3.ProjectOnPlane(model.forward, Vector3.up);

        Vector3 normal = VehicleSurfaceAlignment.SampleNormal(
            transform.position, yawForward, _groundMask, groundSampleRadius, groundRayHeight, groundRayLength);
        Quaternion target = VehicleSurfaceAlignment.RotationOnSurface(yawForward, normal);
        model.rotation = Quaternion.Lerp(model.rotation, target, rotationSpeedModel * Time.fixedDeltaTime);
    }

    private void RotateTurret()
    {
        Vector3 mouseWorld = CameraScreenToWorldPoint.Instance.GetMousePosition();
        VehicleSurfaceAlignment.RotateTurretTowards(turret, mouseWorld, rotationSpeedTurret, Time.fixedDeltaTime);
    }
    
    private float GetMoveSpeed() =>  moveSpeed * Time.fixedDeltaTime;
}
