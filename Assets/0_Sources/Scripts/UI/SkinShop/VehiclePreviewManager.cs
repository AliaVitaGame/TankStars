using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class VehiclePreviewManager : MonoBehaviour
{
    [SerializeField] private Transform privewRoot;
    [SerializeField] private PlayerSkinCollection skinCollection;

    private Dictionary<int, GameObject> _spawnedPreviews = new();
    private string _currentSkinID;

    private void OnEnable()
    {
        YG2.onGetSDKData += LoadedData;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= LoadedData;
    }

    private void Start()
    {
        if(YG2.isSDKEnabled)
            LoadedData();
    }

    private void LoadedData()
    {
        var skinID = YG2.saves.selectedSkinID;

        foreach (var skin in skinCollection.Skins)
        {
            if (skin.SkinID == skinID)
            {
                ShowPreview(skin);
                return;
            }
        }
    }

    public void ShowPreview(PlayerSkinData skinData)
    {
        if (skinData == null)
        {
            Debug.LogWarning("VehiclePreviewManager: skinData is null");
            return;
        }
        
        DisableAllPreviews();
    
        if (_spawnedPreviews.TryGetValue(skinData.SkinID, out GameObject preview))
            preview.SetActive(true);
        else
            SpawnPreview(skinData);
    }

    private void SpawnPreview(PlayerSkinData skinData)
    {
        var modelPreview = Instantiate(skinData.ModelPreview, privewRoot);

        modelPreview.transform.position = privewRoot.position;
        modelPreview.transform.rotation = privewRoot.rotation;
        
        _spawnedPreviews.Add(skinData.SkinID, modelPreview);
    }

    private void DisableAllPreviews()
    {
        foreach (var preview in _spawnedPreviews.Values)
            preview.SetActive(false);
    }
}
