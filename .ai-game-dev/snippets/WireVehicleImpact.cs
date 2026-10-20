using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class WireVehicleImpact
{
    public static string Run()
    {
        var explosion = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/1_Modules/FX/JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Explosions/CFXR Explosion 2 Bigger.prefab");
        var smoke = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/1_Modules/FX/JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Explosions/CFXR Explosion Smoke 1.prefab");

        if (explosion == null)
            return "Explosion prefab not found";

        var folders = new[]
        {
            "Assets/0_Sources/Prefabs/Characters/Player",
            "Assets/0_Sources/Prefabs/Bot"
        };

        var guids = AssetDatabase.FindAssets("t:Prefab", folders);
        var log = new StringBuilder();
        int updated = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                bool isPlayer = root.GetComponent<PlayerHealthStats>() != null;
                bool isBot = root.GetComponent<BotHealthStats>() != null;
                if (!isPlayer && !isBot)
                    continue;

                var death = root.GetComponent<VehicleDeathVfx>();
                if (death == null)
                    death = root.AddComponent<VehicleDeathVfx>();

                var deathSo = new SerializedObject(death);
                deathSo.FindProperty("explosionPrefab").objectReferenceValue = explosion;
                deathSo.FindProperty("smokePrefab").objectReferenceValue = smoke;
                deathSo.ApplyModifiedPropertiesWithoutUndo();

                if (root.GetComponent<VehicleBushCamouflage>() == null)
                    root.AddComponent<VehicleBushCamouflage>();

                if (isBot)
                {
                    var aligner = root.GetComponent<VehicleGroundAligner>();
                    if (aligner == null)
                        aligner = root.AddComponent<VehicleGroundAligner>();

                    var model = root.transform.Find("Model");
                    if (model != null)
                    {
                        var alignSo = new SerializedObject(aligner);
                        alignSo.FindProperty("model").objectReferenceValue = model;
                        alignSo.ApplyModifiedPropertiesWithoutUndo();
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(root, path);
                updated++;
                log.AppendLine(path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        AssetDatabase.SaveAssets();
        return "Updated " + updated + " prefabs\n" + log;
    }
}
