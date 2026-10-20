using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TeamBattleSpawner : CharacterSpawnerBase
{
    [SerializeField] private TeamData teamA;
    [SerializeField] private TeamData teamB;
    [SerializeField] private int botPerTeam = 5;
    [Space]
    [SerializeField] private Transform[] spawnPointsTeamA;
    [SerializeField] private Transform[] spawnPointsTeamB;
    
    private List<Transform> _freePointA;
    private List<Transform> _freePointB;
    
    public override void SpawnAllCharacters()
    {
        _freePointA =  new List<Transform>(spawnPointsTeamA);
        _freePointB =  new List<Transform>(spawnPointsTeamB);
        
        var playerTeam = Random.value > 0.5f ? teamA : teamB;
        var enemyTeam = playerTeam == teamA  ? teamB : teamA;

        var playerPoint = GetRandomSpawnPoint(playerTeam);
        var player = Spawn(SelectedSkinPrefab, playerTeam, playerPoint.position, playerPoint.rotation);
        var isInvertedZ = playerTeam == teamB;
        var isInvertedX = playerTeam == teamB;
        InitializePlayer(player, playerTeam, isInvertedZ, isInvertedX);

        for (int i = 0; i < botPerTeam - 1; i++)
        {
            var botPrefab = UnitFinder.GetUnit();
            var spawnPoint = GetRandomSpawnPoint(playerTeam);
            var bot = Spawn(botPrefab, playerTeam, spawnPoint.position, spawnPoint.rotation);
            InitializeBot(bot, playerTeam);
        }
        
        List<GameObject> enemies = new List<GameObject>();
        for (int i = 0; i < botPerTeam; i++)
        {
            var botPrefab = UnitFinder.GetUnit();
            var spawnPoint = GetRandomSpawnPoint(enemyTeam);
            var bot = Spawn(botPrefab, enemyTeam, spawnPoint.position, spawnPoint.rotation);
            enemies.Add(bot);
            InitializeBot(bot, enemyTeam);
        }
        
        OnAllEnemiesSpawned?.Invoke(enemies.ToArray());
    }
    
    private Transform GetRandomSpawnPoint(TeamData team)
    {
       List<Transform> spawnPoints = team ==  teamA ? _freePointA : _freePointB;
       
       if (spawnPoints.Count == 0)
       {
           Debug.LogError("No Spawn Points Available");
           return null;
       }
       
       int index = Random.Range(0, spawnPoints.Count);
       Transform spawnPoint = spawnPoints[index];
       spawnPoints.RemoveAt(index);
       
       return spawnPoint;
       
    }
}