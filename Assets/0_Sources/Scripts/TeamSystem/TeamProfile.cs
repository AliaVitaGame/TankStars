using UnityEngine;

public class TeamProfile : MonoBehaviour
{
   [SerializeField] private MeshRenderer[] characterMeshRenderers;
      
   public TeamData MyTeam { get; private set; }


   public void Initialize(TeamData data)
   {
      MyTeam = data;
      
      if(characterMeshRenderers.Length <= 0)
         characterMeshRenderers = GetComponentsInChildren<MeshRenderer>();

      foreach (var characterMeshRenderer in characterMeshRenderers)
         characterMeshRenderer.material = MyTeam.MaterialTeam;
   }
}
