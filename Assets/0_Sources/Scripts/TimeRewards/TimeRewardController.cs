using UnityEngine;

public class TimeRewardController : MonoBehaviour
{
    [SerializeField] private Reward[] rewards;
    [SerializeField] private CellTimeReward[] cells;

    private void Start()
    {
        if(rewards.Length != cells.Length) 
            Debug.LogError("Reward count mismatch");

        for (int i = 0; i < cells.Length; i++)
        {
            var reward = rewards[i];
            cells[i].Init(reward.rewardCoin, reward.requiredSeconds);
        }
    }
    
    [System.Serializable]
    public class Reward
    {
        public int rewardCoin = 1;
        public int requiredSeconds = 120;
    }
}


