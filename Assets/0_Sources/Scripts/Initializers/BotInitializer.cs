using UnityEngine;

public class BotInitializer : CharacterInitializerBase
{
    [SerializeField] private Transform pointOfInterest;
    
    public override void Initialize(GameObject character, TeamData teamData)
    {
        if(character.TryGetComponent(out BotMovementBase movement))
            movement.SetPointOfInterest(pointOfInterest);
        
        if(character.TryGetComponent(out TeamProfile teamProfile))
            teamProfile.Initialize(teamData);
    }
}