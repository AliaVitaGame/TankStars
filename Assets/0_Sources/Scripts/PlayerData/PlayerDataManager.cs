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
    }

    public void AddPurchasedSkin(int ID)
    {
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
    
    public void AddCoin(int amount) => YG2.saves.coins += amount;
    public int GetCoin() => YG2.saves.coins;
    public void RemoveCoin(int amount) => YG2.saves.coins -= amount;
  
    public void KillEnemy() => YG2.saves.killedEnemies++;
    public int GetKilledEnemies() => YG2.saves.killedEnemies;
    
    public void SetStateCompletedQuest(int ID, bool state) => YG2.saves.completedQuests[ID] = state;
    public bool GetStateCompletedQuest(int ID) => YG2.saves.completedQuests[ID];
    
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
}
