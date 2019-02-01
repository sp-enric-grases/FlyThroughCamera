using UnityEditor;
using UnityEngine;

namespace SocialPoint.Tools
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : BoxLayoutInspector
    {
        private BezierSpline spline;

        private const int STEPS_PER_CURVE = 10;
        private const float DIRECTION_SCALE = 0.5f;
        private const float HANDLE_SIZE = 0.06f;
        private const float PICK_SIZE = 0.1f;
        private static Color[] modeColors = { Color.white, Color.yellow, Color.cyan };
        private Transform handleTransform;
        private Quaternion handleRotation;
        private int selectedIndex = -1;

        void OnEnable()
        {
            spline = (BezierSpline)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SectionBasicProperties();
            SectionDrawSelectedPoint();
            SectionAddCurve();
        }

        private void SectionBasicProperties()
        {
            Header("Basic Properties");

            spline.showPath = EditorGUILayout.Toggle("Is Visible", spline.showPath);
            EditorGUI.BeginDisabledGroup(!spline.showPath);
            spline.steps = Mathf.Clamp(EditorGUILayout.IntField("Number of Steps", spline.steps), 1, 100);
            spline.pathColor = EditorGUILayout.ColorField("Color", spline.pathColor);
            EditorGUI.EndDisabledGroup();

            Footer();
        }

        private void SectionDrawSelectedPoint()
        {
            if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
            {
                Header("Selected Point");

                GUILayout.Label("Selected Point");
                EditorGUI.BeginChangeCheck();
                Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(selectedIndex, point);
                }

                EditorGUI.BeginChangeCheck();
                BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Change Point Mode");
                    spline.SetControlPointMode(selectedIndex, mode);
                    EditorUtility.SetDirty(spline);
                }

                Footer();
            }
        }

        private void SectionAddCurve()
        {
            Header("Add Curve");

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Add New Curve"))
            {
                Undo.RecordObject(spline, "Add Curve");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }

            Footer();
        }

        public void OnSceneGUI()
        {
            handleTransform = spline.transform;
            handleRotation = UnityEditor.Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
            
            Vector3 p0 = ShowPoint(0, Color.white);

            for (int i = 1; i < spline.ControlPointCount; i += 3)
            {
                Vector3 p1 = ShowPoint(i, Color.yellow);
                Vector3 p2 = ShowPoint(i + 1, Color.yellow);
                Vector3 p3 = ShowPoint(i + 2, Color.white);

                Handles.color = Color.black;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.cyan, null, 2f);
                p0 = p3;
            }
        }

        private Vector3 ShowPoint(int index, Color color)
        {
            Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
            float size = HandleUtility.GetHandleSize(point);
            if (index == 0)
            {
                size *= 2f;
            }
            Handles.color = color;
            //Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
            if (Handles.Button(point, handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotCap))
            {
                selectedIndex = index;
                Repaint();
            }

            if (selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                //point = Handles.FreeMoveHandle(point, Quaternion.identity, size * HANDLE_SIZE * 2, Vector3.one * 0.5f, Handles.RectangleHandleCap);
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                }
            }
            return point;
        }
    }
}