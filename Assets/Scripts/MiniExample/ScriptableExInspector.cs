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
        //private SerializedProperty startEnd;
        //private SerializedProperty paths;

        void OnEnable()
        {
            SEManager = (ScriptableExManager)target;
            nodes = serializedObject.FindProperty("nodes");
            //startEnd = serializedObject.FindProperty("startEndNodes");
            //paths = serializedObject.FindProperty("pathNodes");
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

            //SEManager.number = EditorGUILayout.IntField("Number", SEManager.number);
            //SEManager.word = EditorGUILayout.TextField("Word", SEManager.word);

            EditorGUILayout.PropertyField(nodes, new GUIContent("Nodes"), true);
            //EditorGUILayout.PropertyField(startEnd, new GUIContent("Start-End Nodes"), true);
            //EditorGUILayout.PropertyField(paths, new GUIContent("Paths"), true);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(SEManager);
        }
    }
}