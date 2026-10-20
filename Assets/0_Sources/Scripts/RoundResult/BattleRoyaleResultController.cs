using UnityEngine;

public class BattleRoyaleResultController : RoundResultControllerBase
{
    [SerializeField] private RestartRoundController restartRoundController;

    private IUnitHealth _player;

    protected override void OnEnable()
    {
        base.OnEnable();
        CharacterSpawnerBase.PlayerInitializedContextEvent += GetPlayer;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CharacterSpawnerBase.PlayerInitializedContextEvent -= GetPlayer;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_player != null)
            _player.OnDeathEvent -= PlayerDeath;
    }

    protected override void OnRoundDraw()
    {
       PanelController.Instance.ActiveDrawPanel();
       restartRoundController.Restart();
    }

    protected override void RoundWinningTeam(TeamData winnerTeam)
    {
        var isWin = winnerTeam == PlayerTeam;
        Reward(isWin);
    }

    private void Reward(bool isWin)
    {
        if(isWin) PanelController.Instance.ActiveWinPanel();
        else PanelController.Instance.ActiveLosePanel();
        OnMatchResult?.Invoke(isWin);
    }

    private void PlayerDeath(GameObject player) => Reward(false);

    private void GetPlayer(GameObject player)
    {
        if (player.TryGetComponent(out IUnitHealth health))
        {
            _player = health;
            health.OnDeathEvent += PlayerDeath;
        }
    }
}
