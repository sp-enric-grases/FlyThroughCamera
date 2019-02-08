using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [Serializable]
    public class StartEndNode : BaseNode
    {
        public CameraRotation node;
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        public StartEndNode(Rect rect, string id, string title, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint)
        {
            Debug.Log("<color=green>[FLY-TROUGH]</color> Creating a new start-end node");

            windowRect = rect;
            this.typeOfNode = typeOfNode;
            OnRemoveNode = OnClickRemoveNode;
            this.id = id;
            this.title = title;     

            CreateConnections(OnClickInPoint, OnClickOutPoint, true);
            CreateStartEndNode();
        }

        public void CreateConnections(Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, bool newConnections)
        {
            if (newConnections)
                Debug.Log("<color=green>[FLY-TROUGH]</color> Creating two new connections");
            else
                Debug.Log("<color=green>[FLY-TROUGH]</color> Recovering existing connections");

            inPoint = new ConnectionPoint(this, TypeOfConnection.NodeIn, TypeOfConnection.PathOut, OnClickInPoint);
            outPoint = new ConnectionPoint(this, TypeOfConnection.NodeOut, TypeOfConnection.PathIn, OnClickOutPoint);
        }

        public override void DrawNodes()
        {
            Debug.Log("<color=green>[FLY-TROUGH]</color> Drawing Start-End connections");

            inPoint.Draw();
            outPoint.Draw();
        }

        public override void DrawWindow()
        {
            EditorGUIUtility.labelWidth = 60;
            node = EditorGUILayout.ObjectField("Node", node, typeof(CameraRotation), true) as CameraRotation;

            if (node != null)
                node.transform.position = EditorGUILayout.Vector3Field("Position", node.transform.position, GUILayout.MaxWidth(178));
        }

        private void CreateStartEndNode()
        {
            if (node == null)
            {
                GameObject startEndNode = new GameObject();
                startEndNode.name = "Start-end node";
                node = startEndNode.AddComponent<CameraRotation>();
            }
        }
    }
}