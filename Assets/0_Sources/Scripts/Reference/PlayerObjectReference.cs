using UnityEngine;

public class PlayerObjectReference : UnitObjectReferenceBase
{
    private IUnitHealth _health;

    private void Start()
    {
        _health = GetComponentInChildren<IUnitHealth>();
    }

    public override void RestoreSettings()
    {
        _health.Resurrect();
    }
}
