using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TestNodeEditor
{
    [CustomEditor(typeof(NodeList))]
    public class NodeListInspector : Editor
    {
        private NodeList nodeList;
        private SerializedProperty nodes;

        void OnEnable()
        {
            nodeList = (NodeList)target;
            nodes = serializedObject.FindProperty("listOfNodes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();            
            if (GUILayout.Button("Open in Editor"))
            {
                NodeEditor flyThroughEditor = (NodeEditor)EditorWindow.GetWindow(typeof(NodeEditor));
                flyThroughEditor.titleContent = new GUIContent("Node Editor");
            }
            EditorGUILayout.PropertyField(nodes, new GUIContent("Nodes"), true);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(nodeList);
        }
    }
}