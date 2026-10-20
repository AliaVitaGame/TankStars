using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopCell : MonoBehaviour
{
   [SerializeField] private Image icon;
   [SerializeField] private Button buyButton;
   [SerializeField] private TMP_Text priceText;
   [SerializeField] private TMP_Text ownedText;
   [SerializeField] private TMP_Text selectedText;
   [Space]
   [SerializeField] private Image damageBar; 
   [SerializeField] private Image speedBar;
   [SerializeField] private Image healthBar;

   public SkinShopController _controller { get; private set; }
   public PlayerSkinData _skinData{ get; private set; }

   public void Init(PlayerSkinData skinData, SkinShopController controller)
   {
      _skinData = skinData;
      _controller = controller;
      
      icon.sprite = _skinData.Icon;
      priceText.text = _skinData.Price.ToString();
      
      buyButton.onClick.AddListener(TryBuyOrSelect);

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
      _controller.TryBuyOrSelect(_skinData, this);
   }

   public void RefreshUI()
   {
      var playerData = PlayerDataManager.Instance;
      var isOwned = playerData.HasPurchasedSkin(_skinData.SkinID);

      if (!isOwned) 
         priceText.text = _skinData.Price.ToString();
      
      priceText.gameObject.SetActive(!isOwned);
      if (ownedText)
         ownedText.gameObject.SetActive(false);
      if (selectedText)
         selectedText.gameObject.SetActive(false);
   }
}
