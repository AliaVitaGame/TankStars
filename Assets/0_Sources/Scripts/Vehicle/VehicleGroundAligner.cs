using UnityEngine;

[DefaultExecutionOrder(80)]
public class VehicleGroundAligner : MonoBehaviour
{
    [SerializeField] private Transform model;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float alignSpeed = 10f;
    [SerializeField] private float sampleRadius = 0.85f;
    [SerializeField] private float rayHeight = 1.6f;
    [SerializeField] private float rayLength = 4f;

    private IUnitHealth _health;

    private void Awake()
    {
        _health = GetComponent<IUnitHealth>();
        if (model == null)
        {
            var found = transform.Find("Model");
            model = found != null ? found : null;
        }

        groundMask = VehicleSurfaceAlignment.ResolveGroundMask(groundMask);

        if (model == null)
            enabled = false;
    }

    private void LateUpdate()
    {
        if (model == null)
            return;
        if (_health != null && _health.Health <= 0)
            return;

        Vector3 yawForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (yawForward.sqrMagnitude < 0.0001f)
            yawForward = Vector3.ProjectOnPlane(model.forward, Vector3.up);

        Vector3 normal = VehicleSurfaceAlignment.SampleNormal(
            transform.position, yawForward, groundMask, sampleRadius, rayHeight, rayLength);
        Quaternion target = VehicleSurfaceAlignment.RotationOnSurface(yawForward, normal);
        model.rotation = Quaternion.Slerp(model.rotation, target, alignSpeed * Time.deltaTime);
    }
}
