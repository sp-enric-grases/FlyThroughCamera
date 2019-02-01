using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    [Serializable]
    public class StartEndNode : BaseNode
    {
        public CameraRotation node;
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        public StartEndNode(Rect rect, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, string title, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, string id)
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