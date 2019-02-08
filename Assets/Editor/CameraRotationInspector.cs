using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SocialPoint.Tools;

namespace QGM.FlyThrougCamera
{
    [CustomEditor(typeof(CameraRotation))]
    public class CameraRotationInspector : BoxLayoutInspector
    {
        private CameraRotation cam;
        private SerializedProperty a, b;

        void OnEnable()
        {
            cam = (CameraRotation)target;
            a = serializedObject.FindProperty("a");
            b = serializedObject.FindProperty("b");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            GUILayout.Space(5);

            SectionProperties();
            SectionLimits();

            //EditorGUILayout.PropertyField(a);
            //EditorGUILayout.PropertyField(b);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(cam, "Camera Rotation");

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(cam);
        }

        public void OnSceneGUI()
        {
            Handles.color = new Color(0, 1, 0, 0.2f);
            var rotX = Quaternion.AngleAxis(cam.limitX.x + cam.offsetRotX,  Vector3.up);
            var lDirectionX = rotX * Vector3.back;
            Handles.DrawSolidArc(cam.transform.position, Vector3.up, lDirectionX, cam.limitX.y - cam.limitX.x, 1);
            cam.intLimitX.x = cam.limitX.x + cam.offsetRotX - 180;
            cam.intLimitX.y = cam.limitX.y + cam.offsetRotX - 180;

            Handles.color = new Color(1, 0, 0, 0.2f);
            var rotY = Quaternion.AngleAxis(cam.limitY.x + cam.offsetRotY, Vector3.left);
            var lDirectionY = rotY * Vector3.forward;
            Handles.DrawSolidArc(cam.transform.position, Vector3.left, lDirectionY, cam.limitY.y - cam.limitY.x, 1);
            cam.intLimitY.x = cam.limitY.x + cam.offsetRotY;
            cam.intLimitY.y = cam.limitY.y + cam.offsetRotY;
        }

        private void SectionProperties()
        {
            Header("Basic Properties");

            cam.sensibility = EditorGUILayout.FloatField("Sensibility", cam.sensibility);
            cam.invertDirection = EditorGUILayout.Toggle("Invert Controllers", cam.invertDirection);
            cam.inertia = EditorGUILayout.Toggle("Inertia", cam.inertia);
            EditorGUI.BeginDisabledGroup(!cam.inertia);
            cam.decelerationRate = EditorGUILayout.FloatField("Deceleration Rate", cam.decelerationRate);
            EditorGUI.EndDisabledGroup();

            Footer();
        }

        private void SectionLimits()
        {
            Header("Limits");

            EditorGUIUtility.labelWidth = 90;
            CreateSlider("Horizontal", ref cam.limitX, 0, 360);
            cam.offsetRotX = EditorGUILayout.Slider("Offset: ", cam.offsetRotX, 0, 360);
            GUILayout.Space(3);
            CreateSlider("Vertical", ref cam.limitY, -90, 90);
            cam.offsetRotY = EditorGUILayout.Slider("Offset: ", cam.offsetRotY, 0, 360);

            Footer();
        }

        private void CreateSlider(string label, ref Vector2 reference, float minLimit, float maxLimit)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.MaxWidth(80));
                float limitX = EditorGUILayout.FloatField("Min value: ", reference.x);
                GUILayout.FlexibleSpace();
                float limitY = EditorGUILayout.FloatField("Max value: ", reference.y);
                reference = new Vector2(limitX, limitY);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref reference.x, ref reference.y, minLimit, maxLimit);

        }
    }
}