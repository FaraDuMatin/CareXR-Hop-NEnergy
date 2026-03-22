using UnityEditor;
using UnityEngine;
using System.IO;

public class MaterialAssigner : EditorWindow
{
    [MenuItem("Tools/Assign Crystal Materials")]
    public static void AssignMaterials()
    {
        Debug.Log("[MaterialAssigner] Starting material assignment...");

        // Path to materials and prefabs
        string materialsPath = "Assets/Aztech Games/GemsAndCrystals/Materials";
        string prefabsPath = "Assets/Aztech Games/GemsAndCrystals/Prefabs";

        // Load materials
        Material crystalMat = AssetDatabase.LoadAssetAtPath<Material>(materialsPath + "/Crystals.mat");
        Material gemMat = AssetDatabase.LoadAssetAtPath<Material>(materialsPath + "/Gems.mat");

        if (crystalMat == null || gemMat == null)
        {
            Debug.LogError("[MaterialAssigner] Materials not found! Check paths.");
            return;
        }

        // Get all prefabs
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsPath });

        int assignedCount = 0;

        foreach (string guid in prefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null) continue;

            // Load prefab contents
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            Renderer renderer = instance.GetComponent<Renderer>();

            if (renderer == null)
            {
                renderer = instance.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                // Assign material based on prefab name
                if (prefabPath.Contains("Crystal"))
                {
                    renderer.material = crystalMat;
                    Debug.Log($"[MaterialAssigner] Assigned Crystals.mat to {prefab.name}");
                    assignedCount++;
                }
                else if (prefabPath.Contains("Gem"))
                {
                    renderer.material = gemMat;
                    Debug.Log($"[MaterialAssigner] Assigned Gems.mat to {prefab.name}");
                    assignedCount++;
                }
            }

            // Save changes
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
        }

        AssetDatabase.Refresh();
        Debug.Log($"[MaterialAssigner] ✅ Assigned materials to {assignedCount} prefabs!");
        EditorUtility.DisplayDialog("Success", $"Assigned materials to {assignedCount} prefabs!", "OK");
    }
}
