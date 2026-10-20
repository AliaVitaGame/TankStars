using UnityEngine;

public class TimeRewardController : MonoBehaviour
{
    [SerializeField] private Reward[] rewards;
    [SerializeField] private CellTimeReward[] cells;

    private void Start()
    {
        if (rewards.Length != cells.Length) 
            Debug.LogError("Reward count mismatch");

        int count = Mathf.Min(rewards.Length, cells.Length);
        for (int i = 0; i < count; i++)
        {
            var reward = rewards[i];
            cells[i].Init(i, reward.rewardCoin, reward.requiredSeconds);
        }
    }
    
    [System.Serializable]
    public class Reward
    {
        public int rewardCoin = 1;
        public int requiredSeconds = 120;
    }
}
