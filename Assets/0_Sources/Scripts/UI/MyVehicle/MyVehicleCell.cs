using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyVehicleCell : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button selectButton;
    [SerializeField] private TMP_Text ownedText;
    [SerializeField] private TMP_Text selectedText;
    [Space]
    [SerializeField] private Image damageBar; 
    [SerializeField] private Image speedBar;
    [SerializeField] private Image healthBar;

    public MyVehicleUIController _controller { get; private set; }
    public PlayerSkinData _skinData{ get; private set; }

    public void Init(PlayerSkinData skinData, MyVehicleUIController controller)
    {
        _skinData = skinData;
        _controller = controller;
      
        icon.sprite = _skinData.Icon;
      
        selectButton.onClick.AddListener(TryBuyOrSelect);

        RefreshUI();
    }

    public void InitBar(float damageMax, float speedMax, float healthMax)
    {
        damageBar.fillAmount = _skinData.Weapon.Damage / damageMax;
        speedBar.fillAmount = _skinData.SpeedMove / speedMax;
        healthBar.fillAmount = _skinData.MaxHealth / healthMax;
    }

    public void TryBuyOrSelect()
    {
        _controller.TryBuyOrSelect(_skinData);
    }

    public void RefreshUI()
    {
        var playerData = PlayerDataManager.Instance;
        var isOwned = playerData.HasPurchasedSkin(_skinData.SkinID);
        var isSelected = playerData.GetSelectedSkin() == _skinData.SkinID;
        
        ownedText.gameObject.SetActive(isOwned && !isSelected);
        selectedText.gameObject.SetActive(isSelected && isOwned);
    }
}
