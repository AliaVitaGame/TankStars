using UnityEngine;

public static class VehicleSurfaceAlignment
{
    public static Vector3 SampleNormal(
        Vector3 origin,
        Vector3 yawForward,
        LayerMask groundMask,
        float sampleRadius,
        float rayHeight,
        float rayLength)
    {
        yawForward = Vector3.ProjectOnPlane(yawForward, Vector3.up);
        if (yawForward.sqrMagnitude < 0.0001f)
            yawForward = Vector3.forward;
        yawForward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, yawForward);
        if (right.sqrMagnitude < 0.0001f)
            right = Vector3.right;
        right.Normalize();

        Vector3 accumulated = Vector3.zero;
        int hits = 0;

        TrySample(origin, Vector3.zero);
        TrySample(origin, yawForward * sampleRadius);
        TrySample(origin, -yawForward * sampleRadius);
        TrySample(origin, right * sampleRadius * 0.7f);
        TrySample(origin, -right * sampleRadius * 0.7f);

        if (hits == 0)
            return Vector3.up;

        Vector3 normal = (accumulated / hits).normalized;
        if (normal.y < 0.35f)
            normal = Vector3.Slerp(normal, Vector3.up, 0.65f).normalized;

        return normal;

        void TrySample(Vector3 root, Vector3 offset)
        {
            Vector3 start = root + Vector3.up * rayHeight + offset;
            if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, rayLength, groundMask, QueryTriggerInteraction.Ignore))
            {
                accumulated += hit.normal;
                hits++;
            }
        }
    }

    public static Quaternion RotationOnSurface(Vector3 yawForward, Vector3 normal)
    {
        Vector3 forward = Vector3.ProjectOnPlane(yawForward, normal);
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.ProjectOnPlane(Vector3.forward, normal);

        return Quaternion.LookRotation(forward.normalized, normal);
    }

    public static void RotateTurretTowards(Transform turret, Vector3 worldPoint, float speed, float deltaTime)
    {
        if (turret == null)
            return;

        Vector3 up = turret.parent != null ? turret.parent.up : Vector3.up;
        Vector3 planar = Vector3.ProjectOnPlane(worldPoint - turret.position, up);
        if (planar.sqrMagnitude < 0.0001f)
            return;

        Quaternion target = Quaternion.LookRotation(planar.normalized, up);
        turret.rotation = Quaternion.Lerp(turret.rotation, target, speed * deltaTime);
    }

    public static LayerMask ResolveGroundMask(LayerMask serialized)
    {
        if (serialized.value != 0)
            return serialized;

        int ground = LayerMask.NameToLayer("Ground");
        return ground >= 0 ? 1 << ground : Physics.DefaultRaycastLayers;
    }
}
