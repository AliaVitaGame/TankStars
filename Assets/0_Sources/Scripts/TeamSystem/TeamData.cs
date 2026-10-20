using UnityEngine;

[CreateAssetMenu(menuName = "Game/Team", fileName = "NewTeam")]
public class TeamData : ScriptableObject
{
  [field: SerializeField] public string NameTeam { get; private set; }
  [field: SerializeField] public Color ColorTeamColor { get; private set; }
  [field: SerializeField] public Material MaterialTeam { get; private set; }
}
