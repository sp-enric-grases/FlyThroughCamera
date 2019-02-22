using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [Serializable]
    public class StartNode : BaseNode
    {
        public CameraRotation cr;
        public PathConnections paths;
        public ConnectionPoint outPoint;

        public StartNode(FlyThroughManager ft, Rect rect, string id, string title, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint)
        {
            windowRect = rect;
            this.typeOfNode = typeOfNode;
            OnRemoveNode = OnClickRemoveNode;
            this.id = id;
            this.title = title;     

            CreateConnections(ft, OnClickOutPoint, OnClickInPoint, true);
            CreateStartNode();
        }

        public void CreateConnections(FlyThroughManager ft, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint, bool newConnections)
        {
            this.ft = ft;
            outPoint = new ConnectionPoint(this, TypeOfConnection.NodeOut, TypeOfConnection.PathIn, OnClickOutPoint);
        }

        public override void DrawNodes()
        {
            outPoint.Draw();
        }

        public override void DrawWindow()
        {
            base.DrawNodes();

            EditorGUIUtility.labelWidth = 60;

            if (cr != null)
                cr.transform.position = EditorGUILayout.Vector3Field("Position", cr.transform.position, GUILayout.MaxWidth(178));
        }

        public override void LinkConnection(ConnectionIO connection, BaseNode baseNode)
        {
            if (connection == ConnectionIO.Out) paths.pathsOut.Add(ft.pathNodes.Find(i => i.id == baseNode.id).spline);
        }

        public override void UnlinkConnection(ConnectionIO connection, BaseNode baseNode)
        {
            if (connection == ConnectionIO.Out)
                paths.pathsOut.Remove(ft.pathNodes.Find(i => i.id == baseNode.id).spline);
        }

        private void CreateStartNode()
        {
            if (cr == null)
            {
                node = new GameObject();
                node.name = "Start node";
                paths = node.AddComponent<PathConnections>();
                cr = node.AddComponent<CameraRotation>();
            }
        }
    }
}