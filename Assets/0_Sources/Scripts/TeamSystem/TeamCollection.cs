using UnityEngine;

[CreateAssetMenu(menuName = "Game/TeamCollection", fileName = "NewTeamCollection")]
public class TeamCollection : ScriptableObject
{
   [field: SerializeField] public TeamData[] Teams { get; private set; }
}
