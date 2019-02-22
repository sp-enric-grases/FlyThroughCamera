using UnityEngine;
using UnityEditor;
using SocialPoint.Tools;

namespace QGM.FlyThrougCamera
{
    [CustomEditor(typeof(PathConnections))]
    public class PathConnectionsInspector : BoxLayoutInspector
    {
        private PathConnections paths;
        private SerializedProperty pathsIn;
        private SerializedProperty pathsOut;

        void OnEnable()
        {
            paths = (PathConnections)target;
            paths.hideFlags = HideFlags.HideInInspector;

            pathsIn = serializedObject.FindProperty("pathsIn");
            pathsOut = serializedObject.FindProperty("pathsOut");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            GUILayout.Space(5);

            SectionBasicProperties();

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(paths, "Move Start-End node");

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(paths);
        }

        private void SectionBasicProperties()
        {
            Header("Paths Linked");

            EditorGUILayout.PropertyField(pathsIn, new GUIContent("Paths In"), true);
            EditorGUILayout.PropertyField(pathsOut, new GUIContent("Paths Out"), true);

            Footer();
        }
    }
}