using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsData", menuName = "ScriptableObjects/Create ItemInfo")]
public class ItemsData: ScriptableObject
{
    [field: SerializeField] public int id { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public GameObject prefab { get; private set; }

#if UNITY_EDITOR
    public static void CreateAsset(int id,
                                   string name,
                                   string description)
    {
        if (AssetDatabase.FindAssets($"{name}", new string[] { $"Assets/Data/ItemData/" }).Length > 0)
        {
            // already exist
            return;
        }
        string[] iconGUIDs = AssetDatabase.FindAssets($"{name}", new string[] { "Assets/Textures/Items" });
        if (iconGUIDs.Length <= 0)
        {
            // wrong path / no asset
            return;
        }
        string[] prefabGUIDs = AssetDatabase.FindAssets($"{name}", new string[] { "Assets/Prefabs/Items" });
        if (prefabGUIDs.Length <= 0)
        {
            // wrong path / no asset
            return;
        }

        ItemsData asset = ScriptableObject.CreateInstance<ItemsData>();
        asset.icon = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(iconGUIDs[0]));
        asset.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabGUIDs[0]));
        asset.id = id;
        asset.description = description;

        AssetDatabase.CreateAsset(asset, $"Assets/08.Data/ItemInfo/{name}.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeInstanceID = asset.GetInstanceID();
    }
#endif
}