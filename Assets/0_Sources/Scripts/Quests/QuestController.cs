using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    [SerializeField] private Transform panel;
    [SerializeField] private QuestSetup[] quests;

    private QuestCellBase[] _questCells;

    private void Start()
    {
        var parent = GetCellsParent();
        if (parent == null)
            return;

        EnsureContentFitter(parent);
        BuildQuestList(parent);
        InitCells(parent);
    }

    private Transform GetCellsParent()
    {
        if (panel == null)
            return null;

        var grid = panel.GetComponentInChildren<GridLayoutGroup>(true);
        return grid ? grid.transform : panel;
    }

    private void BuildQuestList(Transform parent)
    {
        var setups = GetSetups();
        var template = parent.GetComponentInChildren<StatQuestCell>(true);
        if (template == null || setups.Length == 0)
            return;

        var cells = new StatQuestCell[setups.Length];
        cells[0] = template;

        for (int i = 1; i < setups.Length; i++)
        {
            var copy = Instantiate(template, parent);
            copy.name = $"Cell (Quests) {i}";
            cells[i] = copy;
        }

        for (int i = 0; i < setups.Length; i++)
        {
            var setup = setups[i];
            cells[i].Configure(setup.type, setup.requiredAmount, setup.rewardCoins, setup.title);
        }
    }

    private void InitCells(Transform parent)
    {
        _questCells = parent.GetComponentsInChildren<QuestCellBase>(true);
        
        for (int i = 0; i < _questCells.Length; i++)
        {
            var questCell = _questCells[i];
            questCell.Init(i);
            questCell.Check();
        }
    }

    private QuestSetup[] GetSetups()
    {
        if (quests != null && quests.Length > 0)
            return quests;

        return new[]
        {
            new QuestSetup { type = QuestStatType.Kills, requiredAmount = 5, rewardCoins = 25 },
            new QuestSetup { type = QuestStatType.Kills, requiredAmount = 20, rewardCoins = 80 },
            new QuestSetup { type = QuestStatType.Wins, requiredAmount = 1, rewardCoins = 50 },
            new QuestSetup { type = QuestStatType.Wins, requiredAmount = 5, rewardCoins = 150 },
            new QuestSetup { type = QuestStatType.Matches, requiredAmount = 3, rewardCoins = 40 },
            new QuestSetup { type = QuestStatType.PlayTimeSeconds, requiredAmount = 300, rewardCoins = 60 },
            new QuestSetup { type = QuestStatType.VehiclesOwned, requiredAmount = 2, rewardCoins = 70 },
            new QuestSetup { type = QuestStatType.CoinsSpent, requiredAmount = 150, rewardCoins = 50 }
        };
    }

    private static void EnsureContentFitter(Transform parent)
    {
        if (parent.GetComponent<ContentSizeFitter>())
            return;

        var fitter = parent.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    [System.Serializable]
    public class QuestSetup
    {
        public QuestStatType type;
        public int requiredAmount = 10;
        public int rewardCoins = 25;
        public string title;
    }
}
