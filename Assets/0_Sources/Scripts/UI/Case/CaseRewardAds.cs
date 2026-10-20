using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;

public class CaseRewardAds : MonoBehaviour
{
    [SerializeField] private float delayBeforeAd = 0.5f;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private GameObject rewardImage;
    [SerializeField] private Button openCaseButton;
    [SerializeField] private RawardData[] rewards;
    [Space]
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private string nameBoxAnimatin = "Open";
    
    private bool _isActive;

    private const string CaseRawardID = "CaseRaward";
    
    private void Start()
    {
        openCaseButton.onClick.AddListener(OnOpenCaseClicked);
    }

    private void OnOpenCaseClicked()
    {
        _isActive = true;
        StartCoroutine(OpenCaseRoutine());
    }

    private IEnumerator OpenCaseRoutine()
    {
        RefreshUI();
        boxAnimator.SetTrigger(nameBoxAnimatin);
        yield return new WaitForSeconds(delayBeforeAd);
        RefreshRewardImage(false);
        ShowRewardedAd();
        RefreshUI();
    }

    private void ShowRewardedAd()
    {
        YG2.RewardedAdvShow(CaseRawardID, OnAdComplited);
    }

    private void OnAdComplited()
    {
        if(_isActive == false) return;

        var reward = GetRawardData();
        PlayerDataManager.Instance.AddCoin(reward.rewardCoin);
        _isActive = false;
        RefreshRewardImage(true, reward.rewardCoin.ToString());
        RefreshUI();
    }

    private void RefreshRewardImage(bool active, string reward = "")
    {
        rewardImage.SetActive(active);
        rewardText.text = $"+{reward}";
    }

    private void RefreshUI()
    {
        openCaseButton.interactable = !_isActive;
    }

    private RawardData GetRawardData()
    {
        var totalChance = 0;

        foreach (var reward in rewards)
            totalChance += reward.chance;

        var randomValue = Random.Range(0, totalChance);
        var currentChance = 0;

        foreach (var reward in rewards)
        {
            currentChance += reward.chance;

            if (randomValue < currentChance)
                return reward;
        }

        return rewards[0];
    }


    [System.Serializable]
    public class RawardData
    {
        public int rewardCoin;
        [Range(0,100)]  
        public int chance;
    }
}
