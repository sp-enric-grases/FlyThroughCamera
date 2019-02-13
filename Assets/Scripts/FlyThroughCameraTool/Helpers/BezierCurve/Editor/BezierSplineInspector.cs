using SocialPoint.Tools;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : BoxLayoutInspector
    {
        private const int STEPS_PER_CURVE = 10;
        private const float DIRECTION_SCALE = 0.5f;
        private const float HANDLE_SIZE = 0.06f;
        private const float PICK_SIZE = 0.1f;

        private BezierSpline spline;

        private SerializedProperty nodeInConnection;
        private SerializedProperty nodeOutConnection;
        private Transform handleTransform;
        private Quaternion handleRotation;
        private int selectedIndex = -1;
        private SerializedProperty points;

        void OnEnable()
        {
            spline = (BezierSpline)target;
            nodeInConnection = serializedObject.FindProperty("nodeInConnection");
            nodeOutConnection = serializedObject.FindProperty("nodeOutConnection");
            points = serializedObject.FindProperty("points");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            SectionBasicProperties();
            SectionStartEndNodes();
            SectionInnerPoints();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Bezier Spline");
                EditorUtility.SetDirty(spline);
            }
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

        private void SectionStartEndNodes()
        {
            Header("Star-End Path Nodes");
            DebugNodes(spline.nodeInConnection, "Start");
            DebugNodes(spline.nodeOutConnection, "End");
            Footer();
        }

        private void DebugNodes(PathConnections node, string type)
        {
            if (node != null)
            {
                Vector3 v = node.transform.position;
                GUILayout.Label(string.Format("    {3} Node Position\tx: {0:0.000}   y: {1:0.000}   z: {2:0.000}", v.x, v.y, v.z, type));
            }
            else
                GUILayout.Label(string.Format("    {0} Node Position:\tNot yet connected", type));
        }


        private void SectionInnerPoints()
        {
            Header("Inner Points");

            if (spline.newPoints.Count == 0 && GUILayout.Button("Add New Point"))
                spline.AddNewPoint(2);
            else
                DrawInnerPoints();

            GUILayout.Space(3);
            SceneView.RepaintAll();

            Footer();
        }

        private void DrawInnerPoints()
        {
            for (int i = 0; i < spline.newPoints.Count; i++)
            {
                int index = i * 3 + 3;

                EditorGUILayout.BeginHorizontal();
                {
                    int SIZE = 22;

                    if (GUILayout.Button("R", GUILayout.MaxWidth(SIZE), GUILayout.MaxHeight(SIZE)))
                    {
                    }

                    if (GUILayout.Button("+", GUILayout.MaxWidth(SIZE), GUILayout.MaxHeight(SIZE)))
                    {
                        spline.AddNewPoint(i + 2);
                    }

                    InnerPointPosition(index);
                    GUILayout.Space(5);
                    if (GUILayout.Button("+", GUILayout.MaxWidth(SIZE), GUILayout.MaxHeight(SIZE)))
                    {
                        spline.AddNewPoint(i + 5);
                    }

                    if (GUILayout.Button("x", GUILayout.MaxWidth(SIZE), GUILayout.MaxHeight(SIZE)))
                    {
                        spline.DeletePoint(i);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void InnerPointPosition(int index)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(4);
                Vector3 initPos = spline.points[index];
                Vector3 newPos = EditorGUILayout.Vector3Field(GUIContent.none, spline.points[index]);
                Vector3 deltaPos = newPos - initPos;
                spline.points[index] = newPos;
                spline.points[index - 1] += deltaPos;
                spline.points[index + 1] += deltaPos;

                if (deltaPos.magnitude != 0) Debug.Log(index);
            }
            EditorGUILayout.EndVertical();
        }

        public void OnSceneGUI()
        {
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
            
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
            if (index == 0) size *= 2f;

            Handles.color = color;
            if (Handles.Button(point, handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap))
            {
                selectedIndex = index;
                //Debug.Log(index);
                Repaint();
            }

            //if (selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.FreeMoveHandle(point, Quaternion.identity, size * HANDLE_SIZE * 3, Vector3.one * 0.5f, Handles.RectangleHandleCap);
                //point = Handles.DoPositionHandle(point, handleRotation);
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