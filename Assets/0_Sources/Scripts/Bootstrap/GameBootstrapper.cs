using System;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private RandomLevelInitializer level;
    [SerializeField] private CharacterSpawnerBase characterSpawner;

    private LevelProfile _currentLevel;
    
    private void Start()
    {
        level.Initialize();
        _currentLevel = level.GetCurrentLevel();
        characterSpawner.Initialize();
    }
}
