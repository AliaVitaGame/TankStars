using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyVehicleUIController : MonoBehaviour
{
    [SerializeField] private Button activePanelButton;  
    [SerializeField] private PlayerSkinCollection skinCollection;
    [SerializeField] private MyVehicleCell cellPrefab;
    [SerializeField] private Transform panelParent;
    [SerializeField] private Image currentSkinIcon;
    [Space] 
    [SerializeField] private VehiclePreviewManager previewManager;

    private List<MyVehicleCell> _cells = new();

    private void Start()
    {
        activePanelButton.onClick.AddListener(ActivePanel);
    }

    private void ActivePanel()
    {
        Clear();
        InitializeCells();
        RefreshAllCells();
        RefreshUI();
    }
    
    public void TryBuyOrSelect(PlayerSkinData data)
    {
        var playerData = PlayerDataManager.Instance;
        playerData.SelectSkin(data.SkinID);
        RefreshAllCells();
        RefreshUI();
        previewManager.ShowPreview(data);
    }

    private void RefreshAllCells()
    {
        foreach (var cell in _cells)
        {
            if(cell)
                cell.RefreshUI();
        }
    }

    private void RefreshUI()
    {
        var data = PlayerDataManager.Instance;
        foreach (var skin in skinCollection.Skins)
        {
            if (skin.SkinID == data.GetSelectedSkin())
            {
                currentSkinIcon.sprite = skin.Icon;
                return;
            }
        }
    }
 
    private void InitializeCells()
    {
        var damageMax = skinCollection.Skins.Max(v => v.Weapon.Damage);
        var speedMax = skinCollection.Skins.Max(v => v.SpeedMove);
        var healthMax = skinCollection.Skins.Max(v => v.MaxHealth);
        
        foreach (var skin in skinCollection.Skins)
        {
            if(!PlayerDataManager.Instance.HasPurchasedSkin(skin.SkinID))
                continue;
                
            var cell = Instantiate(cellPrefab, panelParent);
            cell.Init(skin, this);
            cell.InitBar(damageMax, speedMax, healthMax);
            _cells.Add(cell);
        }
    }

    private void Clear()
    {
        if(_cells == null || _cells.Count <= 0) return;
        
        foreach (var cell in _cells)
            Destroy(cell.gameObject);
        
        _cells.Clear();
    }
}
