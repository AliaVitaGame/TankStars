using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class SkinShopController : MonoBehaviour
{
    [SerializeField] private Image currentSkinIcon; 
    [SerializeField] private PlayerSkinCollection skinCollection;
    [SerializeField] private SkinShopCell cellPrefab;
    [SerializeField] private Transform panelParent;
    [SerializeField] private PlayerSkinData baseSkin;
    [Space] 
    [SerializeField] private VehiclePreviewManager previewManager; 

    private List<SkinShopCell> _cells = new List<SkinShopCell>();

    private void OnEnable()
    {
        YG2.onGetSDKData += Active;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= Active;
    }

    private void Start()
    {
        if(YG2.isSDKEnabled)
            Active();
    }

    private void Active()
    {
        UnlockDefaultSkin();
        InitializeCells();
        RefreshUI();
    }

    public void TryBuyOrSelect(PlayerSkinData data)
    {
        var playerData = PlayerDataManager.Instance;

        if (!playerData.HasPurchasedSkin(data.SkinID))
        {
            if(playerData.GetCoin() < data.Price) return;
            
            playerData.RemoveCoin(data.Price);
            playerData.AddPurchasedSkin(data.SkinID);
        }
        
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
            var cell = Instantiate(cellPrefab, panelParent);
            cell.Init(skin, this);
            cell.InitBar(damageMax, speedMax, healthMax);
            _cells.Add(cell);
        }
    }

    private void UnlockDefaultSkin()
    {
        var defaultSkinID = baseSkin.SkinID;
        var data = PlayerDataManager.Instance;
        
        data.AddPurchasedSkin(defaultSkinID);
        data.SelectSkin(defaultSkinID);
        
        RefreshAllCells();
        RefreshUI();
        previewManager.ShowPreview(baseSkin);
    }
}
