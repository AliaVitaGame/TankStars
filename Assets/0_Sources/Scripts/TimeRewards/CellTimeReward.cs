using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CellTimeReward : MonoBehaviour
{
    [SerializeField] private TMP_Text textRewardCoin;
    [SerializeField] private Button[] buttons;

    private int _rewardCoin;
    private int _requiredSeconds;

    private bool _isClaimed;
    
    private GameTimeTracker _gameTimeTracker;

    private void Start()
    {
        if(buttons == null  || buttons.Length == 0)
         buttons = GetComponentsInChildren<Button>();
        
        
        foreach(var button in buttons)
            button.onClick.AddListener(GiveReward);
        
        RefreshUI();
    }

    public void Init(int rewardCoin, int requiredSeconds)
    {
        _rewardCoin =  rewardCoin;
        _requiredSeconds = requiredSeconds;
        _gameTimeTracker = GameTimeTracker.Instance;

        _gameTimeTracker.OnSecondsChangedEvent += CheckRewardAvailability;

        RefreshUI();
    }

    private void OnDestroy()
    {
        _gameTimeTracker.OnSecondsChangedEvent -= CheckRewardAvailability;
    }

    private void CheckRewardAvailability(int totalSeconds)
    {
        bool isRewardAvailable = totalSeconds >= _requiredSeconds;
        SetInteractable(isRewardAvailable && !_isClaimed);
    }

    private void GiveReward()
    {
        _isClaimed =  true;
        PlayerDataManager.Instance.AddCoin(_rewardCoin);
        SetInteractable(false);
    }

    private void RefreshUI()
    {
        textRewardCoin.text = $"+{_rewardCoin}";
    }

    private void SetInteractable(bool isInteractable)
    {
        foreach(var button in buttons)
            button.interactable =  isInteractable;
    }
}
