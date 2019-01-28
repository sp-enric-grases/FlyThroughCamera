using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SocialPoint.Tools
{
    [CustomEditor(typeof(Influencer))]
    public class InfluencerInspector : BoxLayoutInspector
    {
        private Influencer inf;
        private SerializedProperty alternativeTarget;
        private SerializedProperty targetingCurve;
        private SerializedProperty relaxingCurve;
        private float sizeOfInfluence;

        void OnEnable()
        {
            inf = (Influencer)target;
            alternativeTarget = serializedObject.FindProperty("alternativeTarget");
            targetingCurve = serializedObject.FindProperty("targetingCurve");
            relaxingCurve = serializedObject.FindProperty("relaxingCurve");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            BasicSection();
            SectionTime();

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(inf);
        }

        private void BasicSection()
        {
            Header("Influence Area & Target");

            EditorGUIUtility.labelWidth = 250;
            inf.enableInfluencer = EditorGUILayout.Toggle("Enable influencer", inf.enableInfluencer);
            inf.areaOfInfluence = Mathf.Clamp(EditorGUILayout.FloatField("Area of influence: ", inf.areaOfInfluence), 0, 100);
            inf.useAlternativeTarget = EditorGUILayout.Toggle("Use alternative target", inf.useAlternativeTarget);

            EditorGUI.indentLevel++;
            if (inf.useAlternativeTarget)
                EditorGUILayout.PropertyField(alternativeTarget);
            else
                inf.targetPosition = EditorGUILayout.Vector3Field("Target initial position", inf.targetPosition);

            EditorGUI.indentLevel--;
            Footer();
        }

        private void SectionTime()
        {
            Header("Time");
            EditorGUI.BeginChangeCheck();
            float timeToTargeting = EditorGUILayout.FloatField("Time to Targeting: ", inf.timeToTargeting);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Targeting Time");
                inf.timeToTargeting = timeToTargeting;
            }

            EditorGUI.BeginChangeCheck();
            AnimationCurve tCurve = EditorGUILayout.CurveField("Curve", inf.targetingCurve, inf.orange, new Rect(0, 0, 1, 1));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Curve Time");
                inf.targetingCurve = tCurve;
            }

            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();
            float timeToRelax = EditorGUILayout.FloatField("Time to Relax: ", inf.timeToRelax);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Relaxing Time");
                inf.timeToRelax = timeToRelax;
            }

            EditorGUI.BeginChangeCheck();
            AnimationCurve rCurve = EditorGUILayout.CurveField("Curve", inf.relaxingCurve, inf.orange, new Rect(0, 0, 1, 1));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Curve Time");
                inf.relaxingCurve = rCurve;
            }

            Footer();
        }

        public void OnSceneGUI()
        {
            SizeOfInfluence();

            if (!inf.useAlternativeTarget)
                DrawTarget();
            else
                DrawAlternativeTarget();
        }

        private void DrawAlternativeTarget()
        {
            if (inf.alternativeTarget != null)
            {
                Handles.color = new Color(1, 0.5f, 0, 0.5f);
                Handles.SphereHandleCap(0, inf.alternativeTarget.transform.position, Quaternion.identity, 2, EventType.Repaint);
                Handles.color = Color.black;
                Handles.DrawLine(inf.transform.position, inf.alternativeTarget.transform.position);
            }
        }

        private void DrawTarget()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 targetPos = inf.transform.localPosition + inf.targetPosition;
            Handles.color = new Color(1, 0.5f, 0, 0.5f);
            Handles.SphereHandleCap(0, targetPos, Quaternion.identity, 2, EventType.Repaint);
            Vector3 newTargetPosition = Handles.PositionHandle(targetPos, Quaternion.identity);
            Handles.color = Color.black;
            Handles.DrawLine(inf.transform.position, targetPos);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Target Position");
                inf.targetPosition = newTargetPosition - inf.transform.localPosition;
            }
        }

        private void SizeOfInfluence()
        {
            EditorGUI.BeginChangeCheck();
            Handles.color = new Color(1, 0.5f, 0);
            HandleUtility.GetHandleSize(Vector3.one * 10);
            sizeOfInfluence = Handles.RadiusHandle(Quaternion.identity, inf.transform.position, inf.areaOfInfluence);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Area Of Influence");
                inf.areaOfInfluence = sizeOfInfluence;
            }
        }
    }
}
