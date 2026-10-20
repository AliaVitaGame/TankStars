using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class QuestCellBase : MonoBehaviour
{
    [SerializeField] private int rewardCoins = 10;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text descriptionText;

    protected int CellID { get; private set; }
    protected bool IsComplete;
    protected bool IsClaimed;
    
    private Button _rewardButton;
    private CanvasGroup _canvasGroup;
    private bool _inited;

    public void Init(int cellID)
    {
        if (_rewardButton == null)
            _rewardButton = GetComponent<Button>();

        CacheTexts();
        EnsureCanvasGroup();

        if (_inited == false)
            _rewardButton.onClick.AddListener(ClaimReward);

        CellID = cellID;
        _inited = true;
        RefreshState();
        RefreshUI();
    }

    public abstract void Check();

    protected void SetRewardCoins(int amount) => rewardCoins = Mathf.Max(0, amount);

    protected int RewardCoins => rewardCoins;

    protected void RefreshState()
    {
        IsClaimed = PlayerDataManager.Instance && PlayerDataManager.Instance.GetStateCompletedQuest(CellID);
    }

    protected virtual void OnEnable()
    {
        if (GameTimeTracker.Instance)
            GameTimeTracker.Instance.OnSecondsChangedEvent += OnTrackedTime;

        PlayerDataManager.OnSaveProgressEvent += Check;

        if (_inited == false)
            return;

        Check();
    }

    protected virtual void OnDisable()
    {
        if (GameTimeTracker.Instance)
            GameTimeTracker.Instance.OnSecondsChangedEvent -= OnTrackedTime;

        PlayerDataManager.OnSaveProgressEvent -= Check;
    }

    private void OnTrackedTime(int _)
    {
        if (_inited)
            Check();
    }

    private void ClaimReward()
    {
        if (IsComplete == false || IsClaimed)
            return;
        
        var data = PlayerDataManager.Instance;
        data.AddCoin(rewardCoins);
        data.SetStateCompletedQuest(CellID, true);
        data.Save();
        IsClaimed = true;
        RefreshUI();
    }

    protected virtual void RefreshUI()
    {
        CacheTexts();
        EnsureCanvasGroup();
        RefreshState();

        if (rewardText)
            rewardText.text = IsClaimed
                ? GameUiText.Pick("Получено", "Claimed", "Alındı")
                : $"+{rewardCoins}";

        if (_rewardButton)
            _rewardButton.interactable = IsComplete && IsClaimed == false;

        if (_canvasGroup)
            _canvasGroup.alpha = IsClaimed ? 0.55f : 1f;
    }

    protected void SetDescription(string text)
    {
        CacheTexts();
        if (descriptionText)
            descriptionText.text = text;
    }

    private void CacheTexts()
    {
        if (descriptionText == null)
        {
            var child = transform.Find("Text (Description)");
            if (child)
                descriptionText = child.GetComponent<TMP_Text>();
        }

        if (rewardText == null)
        {
            var child = transform.Find("Text (Reward)");
            if (child)
                rewardText = child.GetComponent<TMP_Text>();
        }
    }

    private void EnsureCanvasGroup()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }
}
