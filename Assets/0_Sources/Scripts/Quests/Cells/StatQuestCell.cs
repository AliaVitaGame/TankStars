using UnityEngine;
using UnityEngine.Serialization;

public enum QuestStatType
{
    Kills,
    Wins,
    Matches,
    PlayTimeSeconds,
    VehiclesOwned,
    CoinsSpent
}

public class StatQuestCell : QuestCellBase
{
    [SerializeField, FormerlySerializedAs("requiredKills")] private int requiredAmount = 10;
    [SerializeField] private QuestStatType statType = QuestStatType.Kills;
    [SerializeField] private string questTitle;

    public void Configure(QuestStatType type, int required, int reward, string title = null)
    {
        statType = type;
        requiredAmount = Mathf.Max(1, required);
        questTitle = title;
        SetRewardCoins(reward);
    }

    public override void Check()
    {
        int current = GetCurrentProgress();
        IsComplete = current >= requiredAmount;
        RefreshState();
        RefreshUI();
        SetDescription(BuildDescription(current));
    }

    private int GetCurrentProgress()
    {
        var data = PlayerDataManager.Instance;
        if (data == null)
            return 0;

        switch (statType)
        {
            case QuestStatType.Kills:
                return data.GetKilledEnemies();
            case QuestStatType.Wins:
                return data.GetMatchesWon();
            case QuestStatType.Matches:
                return data.GetMatchesPlayed();
            case QuestStatType.PlayTimeSeconds:
                return GameTimeTracker.Instance
                    ? GameTimeTracker.Instance.TotalSeconds
                    : data.GetPlayTimeSeconds();
            case QuestStatType.VehiclesOwned:
                return data.GetPurchasedSkinCount();
            case QuestStatType.CoinsSpent:
                return data.GetCoinsSpent();
            default:
                return 0;
        }
    }

    private string BuildDescription(int current)
    {
        int shown = Mathf.Min(current, requiredAmount);
        string title = string.IsNullOrEmpty(questTitle) ? GetDefaultTitle() : questTitle;
        string progress = $"{shown}/{requiredAmount}";

        if (IsClaimed)
            return $"{title}  ·  {GameUiText.Pick("награда получена", "reward claimed", "ödül alındı")}";

        if (IsComplete)
            return $"{title}  ·  {GameUiText.Pick("забрать", "claim", "al")}  ({progress})";

        return $"{title}  ({progress})";
    }

    private string GetDefaultTitle()
    {
        switch (statType)
        {
            case QuestStatType.Kills:
                return GameUiText.Pick($"Уничтожь {requiredAmount} врагов", $"Destroy {requiredAmount} enemies", $"{requiredAmount} düşman yok et");
            case QuestStatType.Wins:
                return GameUiText.Pick($"Выиграй {requiredAmount} боёв", $"Win {requiredAmount} battles", $"{requiredAmount} savaş kazan");
            case QuestStatType.Matches:
                return GameUiText.Pick($"Сыграй {requiredAmount} боёв", $"Play {requiredAmount} battles", $"{requiredAmount} savaş oyna");
            case QuestStatType.PlayTimeSeconds:
                return GameUiText.Pick(
                    $"Играй {GameUiText.FormatDuration(requiredAmount)}",
                    $"Play for {GameUiText.FormatDuration(requiredAmount)}",
                    $"{GameUiText.FormatDuration(requiredAmount)} oyna");
            case QuestStatType.VehiclesOwned:
                return GameUiText.Pick("Купи новую технику", "Buy a new vehicle", "Yeni araç satın al");
            case QuestStatType.CoinsSpent:
                return GameUiText.Pick($"Потрать {requiredAmount} монет", $"Spend {requiredAmount} coins", $"{requiredAmount} madeni harca");
            default:
                return questTitle;
        }
    }
}
