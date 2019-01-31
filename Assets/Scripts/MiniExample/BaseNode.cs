using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    [Serializable]
    public class BaseNode
    {
        public string id;
        public CameraRotation node;
        public string title;
        [HideInInspector]
        public Rect windowRect;

        public Action<BaseNode> OnRemoveNode;

        [NonSerialized] public bool isDragged;
        [NonSerialized] public bool isSelected;

        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        public BaseNode(Rect rect, Action<BaseNode> OnClickRemoveNode, string title, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, string id)
        {
            Debug.Log("<color=green>[FLY-TROUGH]</color> Creating a new start-end node");

            windowRect = rect;
            OnRemoveNode = OnClickRemoveNode;
            this.title = title;
            this.id = id;

            CreateConnections(OnClickInPoint, OnClickOutPoint, true);
            CreateStartEndNode();
        }

        public void OnClickRemoveNodeEvent (Action<BaseNode> OnClickRemoveNode)
        {
            Debug.Log("<color=green>[FLY-TROUGH]</color> Recovering an existing start-end node");
            OnRemoveNode = OnClickRemoveNode;
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

        public void Drag(Vector2 delta)
        {
            windowRect.position += delta;
        }

        public void DrawNodes()
        {
            inPoint.Draw();
            outPoint.Draw();
        }

        public void DrawWindow()
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

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        isSelected = windowRect.Contains(e.mousePosition) ? true : false;
                        GUI.changed = true;
                    }

                    if (e.button == 1 && isSelected && windowRect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        private void OnClickRemoveNode()
        {
            if (OnRemoveNode != null)
                OnRemoveNode(this);
        }
    }
}