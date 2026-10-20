using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class RandomLevelInitializer : MonoBehaviour, IInitializable
{
    [SerializeField] private Transform pivotLevel;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private VolumeProfile baseVolumeProfile;
    [SerializeField] private LevelData[] levels;

    private LevelProfile _currentLevel;
    
    public void Initialize() => SpawnLevel();

    private void SpawnLevel()
    {
        if (levels == null && levels.Length == 0)
        {
            Debug.LogError("The level array is not initialized");
            return;
        }

        var currentLevel = GetRandomLevel();

        _currentLevel = Instantiate(currentLevel.levelPrefab, pivotLevel);
        ApplyLevelSettings(currentLevel);
    }

    private void ApplyLevelSettings(LevelData level)
    {
        if (level.enabledObjects != null)
        {
            foreach (var obj in level.enabledObjects)
            {
                if(obj) 
                    obj.SetActive(true);
            }
        }

        if (level.disableObjects != null) 
        {
            foreach (var obj in level.disableObjects)
            {
                if(obj) 
                    obj.SetActive(false);
            }
        }

        globalVolume.sharedProfile 
            = level.volumeProfile 
                ? level.volumeProfile 
                  : baseVolumeProfile;
        
        Debug.Log(level.volumeProfile );
        Debug.Log(  globalVolume.profile );
    }

    private LevelData GetRandomLevel()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            var currentLevel = levels[Random.Range(0, levels.Length)];
            
            if (currentLevel != null)
                return currentLevel;
        }
        
        Debug.LogError("Unable to find the appropriate level");
        return null;
    }

    public LevelProfile GetCurrentLevel() => _currentLevel;

    [System.Serializable]
    public class LevelData
    {
        public LevelProfile levelPrefab;
        public VolumeProfile volumeProfile; 
        [Space]
        public GameObject[] enabledObjects;
        public GameObject[] disableObjects;
    }
}
