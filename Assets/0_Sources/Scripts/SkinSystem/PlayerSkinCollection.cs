using UnityEngine;

[CreateAssetMenu(menuName = "Game/SkinCollection", fileName = "PlayerSkinCollection")]
public class PlayerSkinCollection : ScriptableObject
{
    [field: SerializeField] public PlayerSkinData[] Skins {get; private set;}
}
