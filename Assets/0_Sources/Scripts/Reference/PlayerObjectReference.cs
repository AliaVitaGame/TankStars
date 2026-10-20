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
        if (_health == null)
            _health = GetComponentInChildren<IUnitHealth>(true);

        _health?.Resurrect();
        PlayerInputManager.Instance?.RefreshCombat();
    }
}
