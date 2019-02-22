using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [Serializable]
    public class EndNode : BaseNode
    {
        public PathConnections paths;
        public ConnectionPoint inPoint;

        public EndNode(FlyThroughManager ft, Rect rect, string id, string title, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint)
        {
            windowRect = rect;
            this.typeOfNode = typeOfNode;
            OnRemoveNode = OnClickRemoveNode;
            this.id = id;
            this.title = title;

            CreateConnections(ft, OnClickOutPoint, OnClickInPoint, true);
            CreateEndNode();
        }

        public void CreateConnections(FlyThroughManager ft, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint, bool newConnections)
        {
            this.ft = ft;
            inPoint = new ConnectionPoint(this, TypeOfConnection.NodeIn, TypeOfConnection.PathOut, OnClickInPoint);
        }

        public override void DrawNodes()
        {
            inPoint.Draw();
        }

        public override void DrawWindow()
        {
            base.DrawNodes();

            EditorGUIUtility.labelWidth = 60;

            if (node != null)
                node.transform.position = EditorGUILayout.Vector3Field("Position", node.transform.position, GUILayout.MaxWidth(178));
        }

        public override void LinkConnection(ConnectionIO connection, BaseNode baseNode)
        {
            if (connection == ConnectionIO.In) paths.pathsIn.Add(ft.pathNodes.Find(i => i.id == baseNode.id).spline);
        }

        public override void UnlinkConnection(ConnectionIO connection, BaseNode baseNode)
        {
            if (connection == ConnectionIO.In)
                paths.pathsIn.Remove(ft.pathNodes.Find(i => i.id == baseNode.id).spline);
        }

        private void CreateEndNode()
        {
            node = new GameObject();
            node.name = "End node";
            paths = node.AddComponent<PathConnections>();
        }
    }
}
