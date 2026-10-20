using System;
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
        PlayerDataManager.Instance.AddCoin(reward);
    }
}
