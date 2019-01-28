using System;
using UnityEditor;
using UnityEngine;

namespace SocialPoint.Tools.FlyThrough
{
    [Serializable]
    public class BaseNode
    {
        public GUID id;
        public TypeOfNode typeOfNode;
        public string title;
        public string name;
        public Rect windowRect;
        public Action<BaseNode> OnRemoveNode;
        [NonSerialized] public bool isDragged;
        [NonSerialized] public bool isSelected;

        public void Drag(Vector2 delta)
        {
            windowRect.position += delta;
        }

        public virtual void DrawWindow() { }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (windowRect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;
                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;
                        }
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