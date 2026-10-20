using UnityEngine;

public abstract class CharacterInitializerBase : MonoBehaviour
{
    public abstract void Initialize(GameObject character, TeamData teamData);
}
