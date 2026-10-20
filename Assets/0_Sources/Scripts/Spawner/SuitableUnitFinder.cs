using UnityEngine;

public class SuitableUnitFinder : MonoBehaviour
{
    [SerializeField] private GameObject[] easyBots;

    public GameObject GetUnit()
    {
        var unitID = Random.Range(0, easyBots.Length);
        return easyBots[unitID];
    }
}
