using UnityEngine;
using UnityEngine.AI;

public class BotObjectReference : UnitObjectReferenceBase
{
    private NavMeshAgent  _agent;
    private IUnitHealth _health;

    private void Start()
    {
        _health = GetComponentInChildren<IUnitHealth>();
        _agent =  GetComponentInChildren<NavMeshAgent>();
    }

    public override void RestoreSettings()
    {
        if (_health == null)
            _health = GetComponentInChildren<IUnitHealth>(true);

        _health?.Resurrect();
        if (_agent == null)
            _agent = GetComponentInChildren<NavMeshAgent>(true);
    }

    public override void RestorePosition(Vector3 position)
    {
        _agent.enabled = false;
        base.RestorePosition(position);
        _agent.enabled = true;
    }

}
