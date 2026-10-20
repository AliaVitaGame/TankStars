using UnityEngine;

public class KillQuestCell : QuestCellBase
{
   [SerializeField] private int requiredKills= 10;
   
   public override void Check()
   {
      var data = PlayerDataManager.Instance;
      if (data.GetKilledEnemies() >= requiredKills)
      {
         IsComplete = true;
         PlayerDataManager.Instance.SetStateCompletedQuest(CellID, true);
      }
      
      RefreshUI();
   }
}
