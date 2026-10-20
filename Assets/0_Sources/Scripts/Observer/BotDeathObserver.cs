using UnityEngine;

public class BotDeathObserver : MonoBehaviour
{
   [SerializeField] private CharacterSpawnerBase characterSpawner;
   
   private GameObject _player;
   private GameObject[] _enemies;
   
   private void OnEnable()
   {
      characterSpawner.OnAllEnemiesSpawned += GetAllEnemies;
      characterSpawner.OnPlayerSpawned += GetPlayer;
   }

   private void OnDisable()
   {
      characterSpawner.OnAllEnemiesSpawned -= GetAllEnemies;
      characterSpawner.OnPlayerSpawned -= GetPlayer;
   }

   private void OnDestroy()
   {
      if(_enemies == null) return;
      
      foreach (var enemy in _enemies)
      {
         if (enemy)
         {
            if (enemy.TryGetComponent(out IUnitHealth health))
               health.OnDeathContextEvent -= EnemyDeathContext;
         }
      }
   }

   private void GetAllEnemies(GameObject[] enemies)
   {
      _enemies = enemies;

      foreach (var enemy in enemies)
      {
         if (enemy.TryGetComponent(out IUnitHealth health))
            health.OnDeathContextEvent += EnemyDeathContext;
      }
   }

   private void GetPlayer(GameObject player, TeamData team) => _player = player;
   
   private void EnemyDeathContext(KillInfo killInfo)
   {
      if(killInfo.Killer == _player)
       PlayerDataManager.Instance.KillEnemy();
   }
   
}
