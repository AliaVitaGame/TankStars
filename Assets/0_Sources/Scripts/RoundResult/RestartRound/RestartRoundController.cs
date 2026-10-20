using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartRoundController : MonoBehaviour
{
    [SerializeField] private float restartTime = 3;
    [SerializeField] private CharacterSpawnerBase spawner;
    
    private List<UnitObjectReferenceBase> _units  = new List<UnitObjectReferenceBase>();
    private List<Vector3> _startPositions =  new List<Vector3>();

    private void OnEnable() => spawner.OnCharacterSpawned += SpawnUnit;
    private void OnDisable() => spawner.OnCharacterSpawned -= SpawnUnit;
    
    public void Restart()
    {
        StartCoroutine(RestartTimer());
    }

    private IEnumerator RestartTimer()
    {
        yield return new WaitForSeconds(restartTime);
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].RestoreSettings();
            _units[i].RestorePosition(_startPositions[i]);
        }
        
        PanelController.Instance.DeactivateAllPanels();
    }

    private void SpawnUnit(GameObject unit, TeamData team)
    {
        if (unit.TryGetComponent(out UnitObjectReferenceBase unitRef))
        {
            _units.Add(unitRef);
            _startPositions.Add(unit.transform.position);
        }
    }
}
