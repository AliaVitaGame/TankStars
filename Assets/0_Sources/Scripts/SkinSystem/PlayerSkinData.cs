using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerSkinData", fileName = "NewPlayerSkinData")]
public class PlayerSkinData : ScriptableObject
{
     [field: SerializeField] public int SkinID {get; private set;}
     [field: SerializeField] public Sprite Icon {get; private set;}
     [field: SerializeField] public GameObject ModelPreview {get; private set;}
     [field: SerializeField] public int Price { get; private set; } = 100;
     [field: Space] 
     [field: SerializeField] public float MaxHealth { get; private set; } = 100;
     [field: SerializeField] public float SpeedMove { get; private set; } = 500;
     [field: SerializeField] public float RotationSpeedModel { get; private set; } = 7;
     [field: SerializeField] public float RotationSpeedTurret { get; private set; } = 100;
     [field: SerializeField] public WeaponData Weapon {get; private set;}
     [field: SerializeField] public GameObject SkinPrefab {get; private set;}

     public void SetSkinID(int ID) => SkinID = ID;
}
