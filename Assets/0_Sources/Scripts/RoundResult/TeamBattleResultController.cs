using System;
using UnityEngine;

public class TeamBattleResultController : RoundResultControllerBase
{
    [SerializeField] private int roundToWin = 3;
    [SerializeField] private RestartRoundController restartController;

    private int[] _teamRoundWin;

    private void Start()
    {
        _teamRoundWin =  new int[TeamCollection.Teams.Length];
    }
    
    protected override void OnRoundDraw()
    {
       PanelController.Instance.ActiveDrawPanel();
       StartNextRound();
    }

    protected override void RoundWinningTeam(TeamData winnerTeam)
    {
       int ID = GetTeamID(winnerTeam);

       _teamRoundWin[ID]++;

       if (_teamRoundWin[ID] >= roundToWin)
       {
           OnMathVictory(winnerTeam);
       }
       else
       {
           StartNextRound(winnerTeam);
       }
    }
    
    protected override void StartNextRound(TeamData winnerTeam = null)
    {
        restartController.Restart();
        
        if (winnerTeam == null) return;
        
        base.StartNextRound(winnerTeam);
        
        if(winnerTeam == PlayerTeam) PanelController.Instance.ActiveRoundWinPanel();
        else PanelController.Instance.ActiveRoundLosePanel();
    }

    private void OnMathVictory(TeamData winnerTeam)
    {
        var isWin = winnerTeam == PlayerTeam;
        
        if(isWin) PanelController.Instance.ActiveWinPanel();
        else PanelController.Instance.ActiveLosePanel();
        
        OnMatchResult?.Invoke(isWin);
    }
}
