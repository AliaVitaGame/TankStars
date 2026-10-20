using System;
using UnityEngine;

public abstract class CharacterSpawnerBase : MonoBehaviour, IInitializable
{
    [SerializeField] private PlayerSkinProvider skinProvider;
    [SerializeField] private SuitableUnitFinder unitFinder;
    [Space]
    [SerializeField] private PlayerInitializer playerInitializer;
    [SerializeField] private BotInitializer botInitializer;

    public Action<GameObject, TeamData> OnCharacterSpawned;
    public Action<GameObject, TeamData> OnPlayerSpawned;
    
    public Action<GameObject[]> OnAllEnemiesSpawned;
    
    public static event Action PlayerInitializedEvent;
    public static event Action<GameObject> PlayerInitializedContextEvent;
    
    protected GameObject SelectedSkinPrefab => skinProvider.GetSelectedSkinPrefab();
    protected SuitableUnitFinder UnitFinder => unitFinder;

    private LevelProfile _currentLevel;

    public void Initialize() => SpawnAllCharacters();
    public void SetCurrentLevel(LevelProfile level) => _currentLevel = level;
    public abstract void SpawnAllCharacters();

    public virtual GameObject Spawn(GameObject character, TeamData team, Vector3 position, Quaternion rotation)
    {
      var tempCharacter = Instantiate(character, position, rotation);
      return tempCharacter;
    }

    protected virtual void InitializePlayer(GameObject character, TeamData team, bool isInvertedZ, bool isInvertedX)
    {
        playerInitializer.Initialize(character, team);
        playerInitializer.InitializeInverted(isInvertedZ, isInvertedX);
        OnCharacterSpawned?.Invoke(character, team);
        OnPlayerSpawned?.Invoke(character, team);
        
        PlayerInitializedEvent?.Invoke();
        PlayerInitializedContextEvent?.Invoke(character);
    }

    protected virtual void InitializeBot(GameObject character, TeamData team)
    {
        botInitializer.Initialize(character, team);
        OnCharacterSpawned?.Invoke(character, team);

        if (character.TryGetComponent(out BotMovementBase movementBase))
        {
            var point = _currentLevel ? _currentLevel.PointOfInterest : null;
            movementBase.SetPointOfInterest(point);
        }
    }
}