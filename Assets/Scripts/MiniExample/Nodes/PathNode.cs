using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    [Serializable]
    public class PathNode : BaseNode
    {
        public FlyThroughPath node;
        public Camera cam;
        public float timeToRelocate = 1;
        public AnimationCurve curveRelocation = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float pathDuration;
        public AnimationCurve curvePath = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        public PathNode(Rect rect, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, string title, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, string id)
        {
            Debug.Log("<color=green>[FLY-TROUGH]</color> Creating a new start-end node");

            windowRect = rect;
            this.typeOfNode = typeOfNode;
            OnRemoveNode = OnClickRemoveNode;
            this.id = id;
            this.title = title;

            CreateConnections(OnClickInPoint, OnClickOutPoint, true);
            CreatePathNode();
        }

        public void CreateConnections(Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, bool newConnections)
        {
            if (newConnections)
                Debug.Log("<color=green>[FLY-TROUGH]</color> Creating two new connections");
            else
                Debug.Log("<color=green>[FLY-TROUGH]</color> Recovering existing connections");

            inPoint = new ConnectionPoint(this, ConnectionPointType.NodeIn, ConnectionPointType.NodeOut, OnClickInPoint);
            outPoint = new ConnectionPoint(this, ConnectionPointType.NodeOut, ConnectionPointType.NodeIn, OnClickOutPoint);
        }

        public override void DrawNodes()
        {
            inPoint.Draw();
            outPoint.Draw();
        }

        public override void DrawWindow()
        {
            node = EditorGUILayout.ObjectField("Node", node, typeof(FlyThroughPath), true) as FlyThroughPath;
            cam = EditorGUILayout.ObjectField("Node", cam, typeof(Camera), true) as Camera;
            timeToRelocate = EditorGUILayout.FloatField("Time to Recolate", timeToRelocate);
            curveRelocation = EditorGUILayout.CurveField("Relocation ", curveRelocation);
            pathDuration = EditorGUILayout.FloatField("Path duration time", pathDuration);
            curvePath = EditorGUILayout.CurveField("Curve path", curvePath);
        }

        private void CreatePathNode()
        {
            if (node == null)
            {
                GameObject pathNode = new GameObject();
                pathNode.name = "Path node";
                node = pathNode.AddComponent<FlyThroughPath>();
            }
        }
    }
}