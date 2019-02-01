using UnityEngine;
using UnityEditor;

namespace SocialPoint.Tools
{
    [CustomEditor(typeof(FlyThroughPath))]
    public class FlyThroughPathInspector : BoxLayoutInspector
    {
        private FlyThroughPath ft;
        private SerializedProperty cam;
        private SerializedProperty trigger;
        private SerializedProperty influencers;

        void OnEnable()
        {
            ft = (FlyThroughPath)target;
            cam = serializedObject.FindProperty("cam");
            trigger = serializedObject.FindProperty("trigger");
            influencers = serializedObject.FindProperty("inf");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            GUILayout.Space(5);

            SectionPathProperties();
            SectionRelocate();
            SectionInfluencers();
            
            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(ft, "Fly Through Path");

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(ft);
        }

        private void SectionPathProperties()
        {
            Header("Path Properties");

            EditorGUILayout.PropertyField(cam, new GUIContent("Camera"));
            EditorGUILayout.PropertyField(trigger, new GUIContent("Trigger"));
            ft.pathDuration = Mathf.Clamp(EditorGUILayout.FloatField("Path Duration", ft.pathDuration), 2.0f, Mathf.Infinity);
            ft.curvePath = EditorGUILayout.CurveField("Curve", ft.curvePath, Color.green, new Rect(0, 0, 1, 1));
            
            CheckPathTime();
            Footer();
        }

        private void SectionRelocate()
        {
            Header("Camera Behaviour");

            EditorGUILayout.LabelField("Initial Relocation", EditorStyles.boldLabel);
            ft.timeToInitRelocatation = Mathf.Clamp(EditorGUILayout.FloatField("Time to Initial Relocation", ft.timeToInitRelocatation), 0.1f, Mathf.Infinity);
            ft.curveInitRelocation = EditorGUILayout.CurveField("Curve", ft.curveInitRelocation, Color.cyan, new Rect(0, 0, 1, 1));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Final Relocation", EditorStyles.boldLabel);
            ft.finalRotation = EditorGUILayout.Vector3Field("Final Camera Rotation", ft.finalRotation);
            ft.timeToFinalRelocation = Mathf.Clamp(EditorGUILayout.FloatField("Time to Final Relocation", ft.timeToFinalRelocation), 0.1f,  Mathf.Infinity);
            ft.curveFinalRelocation = EditorGUILayout.CurveField("Curve", ft.curveFinalRelocation, Color.cyan, new Rect(0, 0, 1, 1));

            CheckPathTime();
            Footer();
        }

        private void SectionInfluencers()
        {
            Header("Influencers");

            EditorGUILayout.PropertyField(influencers, new GUIContent("Influencers"), true);

            Footer();
        }

        private void CheckPathTime()
        {
            float totalTime = ft.pathDuration - (ft.timeToInitRelocatation + ft.timeToFinalRelocation);

            if (totalTime <= 0)
                EditorGUILayout.HelpBox("The addition of the camera's init and final time relocation, should not be more than the path's duration time.", MessageType.Warning);
        }
    }
}