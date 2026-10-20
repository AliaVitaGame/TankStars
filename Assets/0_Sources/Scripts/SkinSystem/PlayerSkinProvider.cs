using System;
using UnityEngine;
using YG;

public class PlayerSkinProvider : MonoBehaviour
{
    [SerializeField] private PlayerSkinCollection skinCollection;

    public static PlayerSkinProvider Instance;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("PlayerSkinProvider instance already exists in the scene");
            return;
        }
        
        Instance = this;
    }

    public GameObject GetSelectedSkinPrefab()
    {
        var selectedSkinID = YG2.saves.selectedSkinID;

        foreach (var skin in skinCollection.Skins)
        {
            if (skin.SkinID == selectedSkinID)
                return skin.SkinPrefab;
        }

        Debug.LogError("The selected skin was not found int the collections");
        return null;
    }
}
