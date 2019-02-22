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
        private SerializedProperty cam;
        private SerializedProperty nodes;
        private SerializedProperty connections;
        private SerializedProperty startNode;
        private SerializedProperty endNode;
        private SerializedProperty battleNodes;
        private SerializedProperty pathNodes;

        void OnEnable()
        {
            SEManager = (FlyThroughManager)target;
            cam = serializedObject.FindProperty("cam");
            nodes = serializedObject.FindProperty("nodes");
            startNode = serializedObject.FindProperty("startNode");
            endNode = serializedObject.FindProperty("endNode");
            connections = serializedObject.FindProperty("connections");
            battleNodes = serializedObject.FindProperty("battleNodes");
            pathNodes = serializedObject.FindProperty("pathNodes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(cam, new GUIContent("Camera"), true);
            GUILayout.Space(10);
            if (GUILayout.Button("Open in Editor"))
            {
                FlyThroughEditor scriptableExEditor = (FlyThroughEditor)EditorWindow.GetWindow(typeof(FlyThroughEditor));
                scriptableExEditor.titleContent = new GUIContent("Scr Editor");
            }

            EditorGUILayout.PropertyField(nodes, new GUIContent("Nodes"), true);
            EditorGUILayout.PropertyField(startNode, new GUIContent("Start Node"), true);
            EditorGUILayout.PropertyField(endNode, new GUIContent("End Node"), true);
            EditorGUILayout.PropertyField(battleNodes, new GUIContent("Battle Nodes"), true);
            EditorGUILayout.PropertyField(pathNodes, new GUIContent("Paths"), true);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(connections, new GUIContent("Connections"), true);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(SEManager);
        }
    }
}