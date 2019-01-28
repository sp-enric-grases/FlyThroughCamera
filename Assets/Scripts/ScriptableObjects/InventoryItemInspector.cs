using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(InventoryItemList))]
public class InventoryItemInspector : Editor
{
    private SerializedProperty items;

    void OnEnable()
    {
        items = serializedObject.FindProperty("itemList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        InventoryItemList itemsList = (InventoryItemList)target;
        if (GUILayout.Button("Open in Editor"))
        {
            InventoryItemEditor inventoryItemEditor = (InventoryItemEditor)EditorWindow.GetWindow(typeof(InventoryItemEditor));
            inventoryItemEditor.inventoryItemList = itemsList;
        }
        EditorGUILayout.PropertyField(items, new GUIContent("Items"), true);

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(itemsList);
    }
}
