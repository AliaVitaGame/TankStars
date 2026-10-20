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
        _health.Resurrect();
    }

    public override void RestorePosition(Vector3 position)
    {
        _agent.enabled = false;
        base.RestorePosition(position);
        _agent.enabled = true;
    }

}
