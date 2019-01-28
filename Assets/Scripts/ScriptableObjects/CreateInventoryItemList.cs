using UnityEngine;
using UnityEditor;

public class CreateInventoryItemList
{
    [MenuItem("Assets/Create/Inventory Item List")]
    public static InventoryItemList Create()
    {
        InventoryItemList asset = ScriptableObject.CreateInstance<InventoryItemList>();
        asset.itemList = new System.Collections.Generic.List<InventoryItem>();

        AssetDatabase.CreateAsset(asset, "Assets/InventoryItemList.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}