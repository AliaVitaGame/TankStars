using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField] private Transform panel;
    
    private QuestCellBase[] _questCells;

    private void Start()
    {
        _questCells = panel.GetComponentsInChildren<QuestCellBase>();
        
        for (int i = 0; i < _questCells.Length; i++)
        {
            var questCell = _questCells[i];
            questCell.Init(i);
            questCell.Check();
        }
    }
}
