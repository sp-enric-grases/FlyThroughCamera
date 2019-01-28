using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SocialPoint.Tools.FlyThrough
{
    [CustomEditor(typeof(FlyThroughManager))]
    public class FlyThroughInspector : Editor
    {
        private FlyThroughManager nodeList;
        private SerializedProperty flyThrough;
        private SerializedProperty baseNodes;
        private SerializedProperty startEnd;
        private SerializedProperty paths;

        void OnEnable()
        {
            nodeList = (FlyThroughManager)target;
            flyThrough = serializedObject.FindProperty("flyThrough");
            baseNodes = serializedObject.FindProperty("baseNodes");
            //startEnd = serializedObject.FindProperty("startEndNodes");
            paths = serializedObject.FindProperty("pathNodes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(flyThrough, new GUIContent("Fly Through Controller"), true);

            EditorGUI.BeginChangeCheck();            
            if (GUILayout.Button("Open in Editor"))
            {
                FlyThroughEditor flyThroughEditor = (FlyThroughEditor)EditorWindow.GetWindow(typeof(FlyThroughEditor));
                flyThroughEditor.titleContent = new GUIContent("Fly Through");
            }

            EditorGUILayout.PropertyField(baseNodes, new GUIContent("Nodes"), true);
            EditorGUILayout.PropertyField(startEnd, new GUIContent("Start-End Nodes"), true);
            EditorGUILayout.PropertyField(paths, new GUIContent("Paths"), true);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(nodeList);
        }
    }
}