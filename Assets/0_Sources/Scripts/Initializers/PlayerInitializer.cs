using Unity.Cinemachine;
using UnityEngine;

public class PlayerInitializer : CharacterInitializerBase
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Transform parentPoint;
    [SerializeField] private PlayerInputManager playerInput;

    public override void Initialize(GameObject character, TeamData teamData)
    {
        playerCamera.Follow = character.transform;
        
        if(character.TryGetComponent(out TeamProfile teamProfile))
            teamProfile.Initialize(teamData);
        
        character.transform.SetParent(parentPoint);
    }

    public void InitializeInverted(bool isInvertedZ, bool isInvertedX) => playerInput.SetInverted(isInvertedZ, isInvertedX);
}