using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [CustomEditor(typeof(FlyThroughManager))]
    public class FlyThroughInspector : Editor
    {
        private FlyThroughManager SEManager;
        private SerializedProperty nodes;
        private SerializedProperty connections;
        private SerializedProperty startEndNodes;
        private SerializedProperty pathNodes;

        void OnEnable()
        {
            SEManager = (FlyThroughManager)target;
            nodes = serializedObject.FindProperty("nodes");
            connections = serializedObject.FindProperty("connections");
            startEndNodes = serializedObject.FindProperty("startEndNodes");
            pathNodes = serializedObject.FindProperty("pathNodes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();            
            if (GUILayout.Button("Open in Editor"))
            {
                FlyThroughEditor scriptableExEditor = (FlyThroughEditor)EditorWindow.GetWindow(typeof(FlyThroughEditor));
                scriptableExEditor.titleContent = new GUIContent("Scr Editor");
            }

            EditorGUILayout.PropertyField(nodes, new GUIContent("Nodes"), true);
            EditorGUILayout.PropertyField(startEndNodes, new GUIContent("Start-End Nodes"), true);
            EditorGUILayout.PropertyField(pathNodes, new GUIContent("Paths"), true);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(connections, new GUIContent("Connections"), true);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(SEManager);
        }
    }
}