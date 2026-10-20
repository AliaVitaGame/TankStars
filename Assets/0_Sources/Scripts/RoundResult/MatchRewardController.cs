using UnityEngine;

public class MatchRewardController : MonoBehaviour
{
    [SerializeField] private int winReward = 100;
    [SerializeField] private int loseReward = 25;

    private void OnEnable() 
        => RoundResultControllerBase.OnMatchResult += ApplyMatchResult;

    private void OnDisable() 
        => RoundResultControllerBase.OnMatchResult -= ApplyMatchResult;

    private void ApplyMatchResult(bool isWin)
    {
        var reward = isWin ? winReward : loseReward;
        var data = PlayerDataManager.Instance;
        data.RegisterMatch(isWin);
        data.AddCoin(reward);
        data.Save();
    }
}
