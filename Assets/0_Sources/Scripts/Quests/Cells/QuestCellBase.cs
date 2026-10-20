using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class QuestCellBase : MonoBehaviour
{
    [SerializeField] private int rewardCoins = 10;
    [SerializeField] private TMP_Text rewardText;

    protected int CellID { get; private set; }
    protected bool IsComplete = false;
    
    private Button _rewardButton;

    public void Init(int cellID)
    {
        _rewardButton = GetComponent<Button>();
        _rewardButton.onClick.AddListener(ClaimReward);
        CellID =  cellID;
        RefreshUI();
    }

    public abstract void Check();

    private void ClaimReward()
    {
        if(IsComplete == false) return;
        
        var data = PlayerDataManager.Instance;
        data.AddCoin(rewardCoins);
        data.Save();
    }

    protected void RefreshUI()
    {
        rewardText.text = $"+{rewardCoins}";
        var state = PlayerDataManager.Instance.GetStateCompletedQuest(CellID);
        _rewardButton.interactable = IsComplete && state == false;
    }
}
