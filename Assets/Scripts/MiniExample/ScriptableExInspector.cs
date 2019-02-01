using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    [CustomEditor(typeof(ScriptableExManager))]
    public class ScriptableExInspector : Editor
    {
        private ScriptableExManager SEManager;
        private SerializedProperty nodes;
        private SerializedProperty connections;
        private SerializedProperty startEndNodes;
        private SerializedProperty pathNodes;

        void OnEnable()
        {
            SEManager = (ScriptableExManager)target;
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
                ScriptableExEditor scriptableExEditor = (ScriptableExEditor)EditorWindow.GetWindow(typeof(ScriptableExEditor));
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