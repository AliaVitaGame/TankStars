using System.Collections.Generic;
using UnityEngine;

public class BattleRoyaleSpawner : CharacterSpawnerBase
{
    [SerializeField] private int enemiesCount = 10;
    [SerializeField] private TeamCollection teamCollection;
    [SerializeField] private Transform[] spawnPoints;

    private List<Transform> _availableSpawnPoints;
    
    protected virtual void Awake()
    {
        _availableSpawnPoints = new List<Transform>(spawnPoints);
    }

    public override void SpawnAllCharacters()
    {
        if (teamCollection == null)
        {
            Debug.LogError("BattleRoyaleSpawner: Command collection is empty");
            return;
        }

        if (_availableSpawnPoints.Count == 0)
        {
            Debug.LogError("BattleRoyaleSpawner: No spawn points available");
            return;
        }

        var freeTeams = new List<TeamData>(teamCollection.Teams);

        var playerID = Random.Range(0, freeTeams.Count);
        var playerTeam = freeTeams[playerID];
        freeTeams.RemoveAt(playerID);

        var playerPoint = GetRandomFreeSpawnPoint();
        var player = Spawn(
            SelectedSkinPrefab, 
            playerTeam, 
            playerPoint.position, 
            playerPoint.rotation
        );
        
        InitializePlayer(player, playerTeam, false, false);

        var maxByTeams = freeTeams.Count;
        int maxBySpawnPoints = _availableSpawnPoints.Count;

        var maxPossinbleEnemies = Mathf.Min(maxByTeams, maxBySpawnPoints);
        var finalEnemiesCount = Mathf.Min(enemiesCount, maxPossinbleEnemies);

        if (finalEnemiesCount < enemiesCount)
        {
            Debug.LogWarning(
                $"BattleRoyaleSpawner: Requested {enemiesCount} enemies, " +
                $"but only {finalEnemiesCount} can be spawned " +
                $"Teams: {maxByTeams}, SpawnPoints: {maxBySpawnPoints}");
        }

        List<GameObject> enemies = new List<GameObject>(finalEnemiesCount);

        for (int i = 0; i < finalEnemiesCount; i++)
        {
            var team = freeTeams[i];
            var botPrefab = UnitFinder.GetUnit();
            var spawnPoint = GetRandomFreeSpawnPoint();

            var bot = Spawn(botPrefab, team, spawnPoint.position, spawnPoint.rotation);
            
            InitializeBot(bot, team);
            enemies.Add(bot);
        }
        
        OnAllEnemiesSpawned?.Invoke(enemies.ToArray());
    }

    public Transform GetRandomFreeSpawnPoint()
    {
        if (_availableSpawnPoints.Count == 0)
        {
            Debug.LogError("No available spawn points");
            return null;
        }
        
        int index = Random.Range(0, _availableSpawnPoints.Count);
        Transform point =  _availableSpawnPoints[index];
        _availableSpawnPoints.RemoveAt(index);
        return point;
    }
}
