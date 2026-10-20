using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RewardedAdHandler))]
[RequireComponent(typeof(InterstitialAdHandler))]
public class AdController : MonoBehaviour
{
    [SerializeField] private Button[] interstitialButtons;
    [SerializeField] private Rewards[] rewards;
    
    private InterstitialAdHandler _interstitial;
    private RewardedAdHandler _rewardedAd;

    private void Start()
    {
        _interstitial = GetComponent<InterstitialAdHandler>();
        _rewardedAd = GetComponent<RewardedAdHandler>();
        
        foreach (var button in interstitialButtons)
            button.onClick.AddListener(ShowInterstitial);
        
        for (int i = 0; i < rewards.Length; i++)
        {
            int ID = i;
            rewards[i].Button.onClick.AddListener(() => ShowRewarded(ID));
        }
    }

    public void ShowInterstitial() => _interstitial.Show();
    public void ShowRewarded(int ID) => _rewardedAd.Show(rewards[ID].RewardName);
    
    [System.Serializable]
    public class Rewards
    {
        public string RewardName;
        public IssuingRewardBase Reward;
        public Button Button;
    }
    
}
