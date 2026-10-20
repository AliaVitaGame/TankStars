using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerSkinCollection))]
public class SkinIDAssignerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUILayout.Space(10);

        if (GUILayout.Button("Assign Unique IDs"))
            AssignUniqueIDs();
    }

    private void AssignUniqueIDs()
    {
        var assigner  = (PlayerSkinCollection)target;

        if (assigner.Skins == null || assigner.Skins.Length == 0)
        {
            Debug.LogWarning("Skin list is emty");
            return;
        }

        HashSet<int> usedIDs = new HashSet<int>();

        foreach (var skin in assigner.Skins)
        {
            if (skin != null && skin.SkinID >= 0)
                usedIDs.Add(skin.SkinID);
        }

        int nextID = 0;

        foreach (var skin in assigner.Skins)
        {
           if(skin == null) continue;

           if (skin.SkinID < 0 || usedIDs.Contains(skin.SkinID))
           {
               while (usedIDs.Contains(nextID))
                   nextID++;

               skin.SetSkinID(nextID);
               usedIDs.Add(nextID);
           }
           
           EditorUtility.SetDirty(skin);
        }
        
        AssetDatabase.SaveAssets();
        
        Debug.Log("Skin IDs assigned successfully!");
    }
}
