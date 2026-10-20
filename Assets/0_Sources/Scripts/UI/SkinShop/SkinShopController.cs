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
    [SerializeField] private RectTransform purchaseFlyTarget;
    [Space] 
    [SerializeField] private VehiclePreviewManager previewManager; 

    private readonly List<SkinShopCell> _cells = new List<SkinShopCell>();
    private bool _initialized;

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

        if (_initialized == false)
        {
            InitializeCells();
            _initialized = true;
        }
        else
        {
            RemoveOwnedCells(false);
        }

        RefreshAllCells();
        RefreshUI();
    }

    public void TryBuyOrSelect(PlayerSkinData data, SkinShopCell cell)
    {
        var playerData = PlayerDataManager.Instance;
        bool isNewPurchase = playerData.HasPurchasedSkin(data.SkinID) == false;

        if (isNewPurchase)
        {
            if (playerData.GetCoin() < data.Price)
                return;
            
            playerData.RemoveCoin(data.Price);
            playerData.AddPurchasedSkin(data.SkinID);
        }
        
        playerData.SelectSkin(data.SkinID);
        RefreshUI();
        previewManager.ShowPreview(data);

        if (isNewPurchase && cell)
            PlayPurchaseLeave(cell);
        else
            RefreshAllCells();
    }

    private void PlayPurchaseLeave(SkinShopCell cell)
    {
        _cells.Remove(cell);

        if (cell.TryGetComponent(out Button button))
            button.interactable = false;

        var fly = cell.GetComponent<UICardFlyAnimator>() ?? cell.gameObject.AddComponent<UICardFlyAnimator>();
        var target = GetPurchaseFlyTarget();

        fly.Play(target, () =>
        {
            if (cell)
                Destroy(cell.gameObject);
        });
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
        var playerData = PlayerDataManager.Instance;
        
        foreach (var skin in skinCollection.Skins)
        {
            if (playerData.HasPurchasedSkin(skin.SkinID))
                continue;

            var cell = Instantiate(cellPrefab, panelParent);
            cell.Init(skin, this);
            cell.InitBar(damageMax, speedMax, healthMax);
            _cells.Add(cell);
        }
    }

    private void RemoveOwnedCells(bool animate)
    {
        var playerData = PlayerDataManager.Instance;
        for (int i = _cells.Count - 1; i >= 0; i--)
        {
            var cell = _cells[i];
            if (cell == null)
            {
                _cells.RemoveAt(i);
                continue;
            }

            if (playerData.HasPurchasedSkin(cell._skinData.SkinID) == false)
                continue;

            _cells.RemoveAt(i);
            if (animate)
                PlayPurchaseLeave(cell);
            else
                Destroy(cell.gameObject);
        }
    }

    private void UnlockDefaultSkin()
    {
        var defaultSkinID = baseSkin.SkinID;
        var data = PlayerDataManager.Instance;
        data.CompactPurchasedSkins();
        
        data.AddPurchasedSkin(defaultSkinID);

        if (data.HasPurchasedSkin(data.GetSelectedSkin()) == false)
            data.SelectSkin(defaultSkinID);
        
        previewManager.ShowPreview(GetSkinById(data.GetSelectedSkin()) ?? baseSkin);
    }

    private RectTransform GetPurchaseFlyTarget()
    {
        if (purchaseFlyTarget)
            return purchaseFlyTarget;

        var technique = GameObject.Find("Button (My technique)");
        if (technique)
            return technique.transform as RectTransform;

        return currentSkinIcon ? currentSkinIcon.rectTransform : null;
    }

    private PlayerSkinData GetSkinById(int id)
    {
        foreach (var skin in skinCollection.Skins)
        {
            if (skin.SkinID == id)
                return skin;
        }

        return null;
    }
}
