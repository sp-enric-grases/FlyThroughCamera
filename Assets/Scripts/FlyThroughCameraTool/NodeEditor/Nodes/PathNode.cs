using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [Serializable]
    public class PathNode : BaseNode
    {
        public FlyThroughPath ftp;
        public BezierSpline spline;
        public Camera cam;
        public float timeToRelocate = 1;
        public AnimationCurve curveRelocation = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float pathDuration;
        public AnimationCurve curvePath = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public ConnectionPoint outPoint;
        public ConnectionPoint inPoint;

        public string idIn = string.Empty;
        public string idOut = string.Empty;

        public PathNode(FlyThroughManager ft, Rect rect, string id, string title, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint)
        {
            windowRect = rect;
            this.typeOfNode = typeOfNode;
            OnRemoveNode = OnClickRemoveNode;
            this.id = id;
            this.title = title;

            CreateConnections(ft, OnClickOutPoint, OnClickInPoint, true);
            CreatePathNode();
        }

        public void CreateConnections(FlyThroughManager ft, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint, bool newConnections)
        {
            this.ft = ft;
            outPoint = new ConnectionPoint(this, TypeOfConnection.PathOut, TypeOfConnection.NodeIn, OnClickOutPoint);
            inPoint = new ConnectionPoint(this, TypeOfConnection.PathIn, TypeOfConnection.NodeOut, OnClickInPoint);

        }

        public override void DrawNodes()
        {
            inPoint.Draw();
            outPoint.Draw();
        }

        public override void DrawWindow()
        {
            base.DrawNodes();

            //ftp = EditorGUILayout.ObjectField("Node", ftp, typeof(FlyThroughPath), true) as FlyThroughPath;
            cam = EditorGUILayout.ObjectField("Node", cam, typeof(Camera), true) as Camera;
            timeToRelocate = EditorGUILayout.FloatField("Time to Recolate", timeToRelocate);
            curveRelocation = EditorGUILayout.CurveField("Relocation ", curveRelocation);
            pathDuration = EditorGUILayout.FloatField("Path duration time", pathDuration);
            curvePath = EditorGUILayout.CurveField("Curve path", curvePath);
        }

        public override void LinkConnection(ConnectionIO connection, BaseNode node)
        {
            if (connection == ConnectionIO.Out)
            {
                spline.nodeOutConnection = ft.startEndNodes.Find(i => i.id == node.id).cr.gameObject.GetComponent<PathConnections>();
                spline.nodeOutConnection.ModeNode();
            }

            if (connection == ConnectionIO.In)
            {
                spline.nodeInConnection = ft.startEndNodes.Find(i => i.id == node.id).cr.gameObject.GetComponent<PathConnections>();
                spline.nodeInConnection.ModeNode();
            }
        }

        public override void UnlinkConnection(ConnectionIO connection, BaseNode id)
        {
            if (connection == ConnectionIO.Out)
                spline.nodeOutConnection = null;

            if (connection == ConnectionIO.In)
                spline.nodeInConnection = null;
        }

        private void CreatePathNode()
        {
            if (ftp == null)
            {
                node = new GameObject();
                node.name = "Path node";
                ftp = node.AddComponent<FlyThroughPath>();
                spline = ftp.GetComponent<BezierSpline>();
            }
        }
    }
}