using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CellTimeReward : MonoBehaviour
{
    [SerializeField] private TMP_Text textRewardCoin;
    [SerializeField] private TMP_Text textStatus;
    [SerializeField] private Button[] buttons;

    private int _cellId;
    private int _rewardCoin;
    private int _requiredSeconds;
    private bool _isClaimed;
    private bool _bound;
    
    private GameTimeTracker _gameTimeTracker;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        CacheButtons();
        CacheStatusText();
        EnsureCanvasGroup();
    }

    public void Init(int cellId, int rewardCoin, int requiredSeconds)
    {
        _cellId = cellId;
        _rewardCoin = rewardCoin;
        _requiredSeconds = requiredSeconds;
        _gameTimeTracker = GameTimeTracker.Instance;

        CacheButtons();
        CacheStatusText();
        EnsureCanvasGroup();
        BindButtons();

        var data = PlayerDataManager.Instance;
        _isClaimed = data && data.HasReceivedReward(_cellId);

        if (_gameTimeTracker != null)
        {
            _gameTimeTracker.OnSecondsChangedEvent -= CheckRewardAvailability;
            _gameTimeTracker.OnSecondsChangedEvent += CheckRewardAvailability;
            CheckRewardAvailability(_gameTimeTracker.TotalSeconds);
        }
        else
        {
            RefreshUI(0);
        }
    }

    private void OnDestroy()
    {
        if (_gameTimeTracker != null)
            _gameTimeTracker.OnSecondsChangedEvent -= CheckRewardAvailability;
    }

    private void CheckRewardAvailability(int totalSeconds)
    {
        RefreshUI(totalSeconds);
    }

    private void GiveReward()
    {
        if (_isClaimed)
            return;

        int totalSeconds = _gameTimeTracker ? _gameTimeTracker.TotalSeconds : 0;
        if (totalSeconds < _requiredSeconds)
            return;

        _isClaimed = true;

        var data = PlayerDataManager.Instance;
        if (data)
        {
            data.AddCoin(_rewardCoin);
            data.SetReceivedReward(_cellId, true);
            data.Save();
        }

        RefreshUI(totalSeconds);
    }

    private void RefreshUI(int totalSeconds)
    {
        bool isReady = totalSeconds >= _requiredSeconds && _isClaimed == false;

        if (textRewardCoin)
            textRewardCoin.text = $"+{_rewardCoin}\n{GameUiText.FormatDuration(_requiredSeconds)}";

        if (textStatus)
        {
            if (_isClaimed)
                textStatus.text = GameUiText.Pick("Получено", "Received", "Alındı");
            else if (isReady)
                textStatus.text = GameUiText.Pick("Забрать", "Claim", "Al");
            else
                textStatus.text = GameUiText.FormatClock(_requiredSeconds - totalSeconds);
        }

        SetInteractable(isReady);

        if (_canvasGroup)
            _canvasGroup.alpha = _isClaimed ? 0.5f : 1f;
    }

    private void SetInteractable(bool isInteractable)
    {
        CacheButtons();
        foreach (var button in buttons)
        {
            if (button)
                button.interactable = isInteractable;
        }
    }

    private void BindButtons()
    {
        if (_bound)
            return;

        _bound = true;
        foreach (var button in buttons)
        {
            if (button == null)
                continue;
            button.onClick.RemoveListener(GiveReward);
            button.onClick.AddListener(GiveReward);
        }
    }

    private void CacheButtons()
    {
        if (buttons == null || buttons.Length == 0)
            buttons = GetComponentsInChildren<Button>(true);
    }

    private void CacheStatusText()
    {
        if (textStatus)
            return;

        CacheButtons();
        foreach (var button in buttons)
        {
            if (button == null)
                continue;
            textStatus = button.GetComponentInChildren<TMP_Text>(true);
            if (textStatus)
                return;
        }
    }

    private void EnsureCanvasGroup()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }
}
