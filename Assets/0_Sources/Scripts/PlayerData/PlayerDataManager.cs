using System;
using System.Linq;
using UnityEngine;
using YG;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public static Action OnSaveProgressEvent;

    private void Awake()
    {
        Instance = this;
        CompactPurchasedSkins();
    }

    private void OnEnable()
    {
        YG2.onGetSDKData += CompactPurchasedSkins;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= CompactPurchasedSkins;
    }

    public void AddPurchasedSkin(int ID)
    {
        if (HasPurchasedSkin(ID))
            return;

        var data = YG2.saves;
        var list = data.purchasedSkinIDs.ToList();
        list.Add(ID);
        data.purchasedSkinIDs = list.ToArray();
        Save();
    }

    public void SelectSkin(int ID)
    {
        var data = YG2.saves;
        data.selectedSkinID = ID;
        Save();
    }

    public int GetSelectedSkin() => YG2.saves.selectedSkinID;
    public bool HasPurchasedSkin(int ID) => YG2.saves.purchasedSkinIDs.Contains(ID);
    public int GetPurchasedSkinCount() => YG2.saves.purchasedSkinIDs.Distinct().Count();
    
    public void AddCoin(int amount) => YG2.saves.coins += amount;
    public int GetCoin() => YG2.saves.coins;
    public void RemoveCoin(int amount)
    {
        amount = Mathf.Max(0, amount);
        YG2.saves.coins -= amount;
        YG2.saves.coinsSpent += amount;
    }
    public int GetCoinsSpent() => YG2.saves.coinsSpent;
  
    public void KillEnemy() => YG2.saves.killedEnemies++;
    public int GetKilledEnemies() => YG2.saves.killedEnemies;

    public void RegisterMatch(bool isWin)
    {
        YG2.saves.matchesPlayed++;
        if (isWin)
            YG2.saves.matchesWon++;
    }

    public int GetMatchesPlayed() => YG2.saves.matchesPlayed;
    public int GetMatchesWon() => YG2.saves.matchesWon;

    public void SetPlayTimeSeconds(int seconds) => YG2.saves.playTimeSeconds = Mathf.Max(0, seconds);
    public int GetPlayTimeSeconds() => YG2.saves.playTimeSeconds;
    
    public void SetStateCompletedQuest(int ID, bool state)
    {
        EnsureQuestArray();
        if (ID < 0 || ID >= YG2.saves.claimedQuests.Length)
            return;
        YG2.saves.claimedQuests[ID] = state;
    }

    public bool GetStateCompletedQuest(int ID)
    {
        EnsureQuestArray();
        if (ID < 0 || ID >= YG2.saves.claimedQuests.Length)
            return false;
        return YG2.saves.claimedQuests[ID];
    }

    public void SetReceivedReward(int ID, bool state)
    {
        EnsureRewardArray();
        if (ID < 0 || ID >= YG2.saves.receivedRewards.Length)
            return;
        YG2.saves.receivedRewards[ID] = state;
    }

    public bool HasReceivedReward(int ID)
    {
        EnsureRewardArray();
        if (ID < 0 || ID >= YG2.saves.receivedRewards.Length)
            return false;
        return YG2.saves.receivedRewards[ID];
    }
    
    public void SetMusicVolume(float volume) => YG2.saves.musicVolume = volume;
    public float GetMusicVolume() => YG2.saves.musicVolume;
    
    public void SetSfxVolume(float volume) => YG2.saves.sfxVolume = volume;
    public float GetSfxVolume() => YG2.saves.sfxVolume;

    public void Save()
    {
        YG2.SaveProgress();
        OnSaveProgressEvent?.Invoke();
    }

    public void ResetAllData()
    {
        YG2.SetDefaultSaves();
        YG2.SaveProgress();
        SceneLoadingManager.Instance.LoadActiveScene();
    }

    public void CompactPurchasedSkins()
    {
        var data = YG2.saves;
        if (data.purchasedSkinIDs == null || data.purchasedSkinIDs.Length == 0)
            return;

        data.purchasedSkinIDs = data.purchasedSkinIDs.Distinct().ToArray();
    }

    private static void EnsureQuestArray()
    {
        if (YG2.saves.claimedQuests == null || YG2.saves.claimedQuests.Length == 0)
            YG2.saves.claimedQuests = new bool[64];
    }

    private static void EnsureRewardArray()
    {
        if (YG2.saves.receivedRewards == null || YG2.saves.receivedRewards.Length == 0)
            YG2.saves.receivedRewards = new bool[64];
    }
}
