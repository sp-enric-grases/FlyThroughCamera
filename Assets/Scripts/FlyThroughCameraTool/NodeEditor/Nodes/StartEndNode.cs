using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [Serializable]
    public class StartEndNode : BaseNode
    {
        public CameraRotation cr;
        public PathConnections paths;
        public ConnectionPoint outPoint;
        public ConnectionPoint inPoint;

        public StartEndNode(FlyThroughManager ft, Rect rect, string id, string title, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint)
        {
            windowRect = rect;
            this.typeOfNode = typeOfNode;
            OnRemoveNode = OnClickRemoveNode;
            this.id = id;
            this.title = title;     

            CreateConnections(ft, OnClickOutPoint, OnClickInPoint, true);
            CreateStartEndNode();
        }

        public void CreateConnections(FlyThroughManager ft, Action<ConnectionPoint> OnClickOutPoint, Action<ConnectionPoint> OnClickInPoint, bool newConnections)
        {
            this.ft = ft;
            outPoint = new ConnectionPoint(this, TypeOfConnection.NodeOut, TypeOfConnection.PathIn, OnClickOutPoint);
            inPoint = new ConnectionPoint(this, TypeOfConnection.NodeIn, TypeOfConnection.PathOut, OnClickInPoint);
        }

        public override void DrawNodes()
        {
            inPoint.Draw();
            outPoint.Draw();
        }

        public override void DrawWindow()
        {
            base.DrawNodes();

            EditorGUIUtility.labelWidth = 60;

            //cr = EditorGUILayout.ObjectField("Node", cr, typeof(CameraRotation), true) as CameraRotation;

            if (cr != null)
                cr.transform.position = EditorGUILayout.Vector3Field("Position", cr.transform.position, GUILayout.MaxWidth(178));
        }

        public override void LinkConnection(ConnectionIO connection, BaseNode baseNode)
        {
            if (connection == ConnectionIO.Out) paths.pathsOut.Add(ft.pathNodes.Find(i => i.id == baseNode.id).spline);
            if (connection == ConnectionIO.In) paths.pathsIn.Add(ft.pathNodes.Find(i => i.id == baseNode.id).spline);
        }

        public override void UnlinkConnection(ConnectionIO connection, BaseNode baseNode)
        {
            if (connection == ConnectionIO.Out)
                paths.pathsOut.Remove(ft.pathNodes.Find(i => i.id == baseNode.id).spline);

            if (connection == ConnectionIO.In)
                paths.pathsIn.Remove(ft.pathNodes.Find(i => i.id == baseNode.id).spline);
        }

        private void CreateStartEndNode()
        {
            if (cr == null)
            {
                node = new GameObject();
                node.name = "Start-end node";
                paths = node.AddComponent<PathConnections>();
                cr = node.AddComponent<CameraRotation>();
            }
        }
    }
}