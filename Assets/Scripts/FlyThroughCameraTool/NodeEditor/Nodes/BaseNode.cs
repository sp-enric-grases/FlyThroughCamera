using SocialPoint.Tools;
using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [Serializable]
    public class Node
    {
        public string id;
        public TypeOfNode typeOfNode;
    }

    [Serializable]
    public class BaseNode
    {
        [NonSerialized] public FlyThroughManager ft;
        public GameObject node;
        public string id;
        public string title;
        public TypeOfNode typeOfNode;
        public Action<BaseNode> OnRemoveNode;
        public Rect windowRect;
        [NonSerialized] public bool isDragged;
        [NonSerialized] public bool isSelected;

        public void OnClickRemoveNodeEvent (Action<BaseNode> OnClickRemoveNode)
        {
            OnRemoveNode = OnClickRemoveNode;
        }

        public void Drag(Vector2 delta)
        {
            windowRect.position += delta;
        }

        public virtual void DrawNodes()
        {
            if (node == null)
                OnRemoveNode(this);
        }

        public virtual void DrawWindow() { }

        public virtual void LinkConnection(ConnectionIO connection, BaseNode id) { }

        public virtual void UnlinkConnection(ConnectionIO connection, BaseNode id) { }

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