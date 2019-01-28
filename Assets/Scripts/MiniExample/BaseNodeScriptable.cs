using System;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    [Serializable]
    public class BaseNodeScriptable
    {
        public GUID id;
        public GameObject go;
        public string title;
        public int number;
        public string word;
        public Rect windowRect;
        public Action<BaseNode> OnRemoveNode;
        [NonSerialized] public bool isDragged;
        [NonSerialized] public bool isSelected;

        //public BaseNode(Rect rect, Action<BaseNode> OnClickRemoveNode, string title, GUID id)
        //{
        //    windowRect = rect;
        //    OnRemoveNode = OnClickRemoveNode;
        //    this.title = title;
        //    this.id = id;

        //    CreateStartEndNode();
        //}

        //public void Drag(Vector2 delta)
        //{
        //    windowRect.position += delta;
        //}

        //public void DrawWindow()
        //{
        //    //if (node != null)
        //    //{
        //    //    node = EditorGUILayout.ObjectField("Node", node, typeof(GameObject), true) as GameObject;
        //    //    node.transform.position = EditorGUILayout.Vector3Field("Position", node.transform.position);
        //    //}
        //}

        //private void CreateStartEndNode()
        //{
        //    //if (name == "") name = "Start-End Node";

        //    //if (node == null)
        //    //{
        //    //    node = new GameObject();
        //    //    node.AddComponent<CameraRotation>();
        //    //    node.name = name;
        //    //}
        //}

        //public bool ProcessEvents(Event e)
        //{
        //    switch (e.type)
        //    {
        //        case EventType.MouseDown:
        //            if (e.button == 0)
        //            {
        //                if (windowRect.Contains(e.mousePosition))
        //                {
        //                    isDragged = true;
        //                    GUI.changed = true;
        //                    isSelected = true;
        //                }
        //                else
        //                {
        //                    GUI.changed = true;
        //                    isSelected = false;
        //                }
        //            }

        //            if (e.button == 1 && isSelected && windowRect.Contains(e.mousePosition))
        //            {
        //                ProcessContextMenu();
        //                e.Use();
        //            }
        //            break;

        //        case EventType.MouseUp:
        //            isDragged = false;
        //            break;

        //        case EventType.MouseDrag:
        //            if (e.button == 0 && isDragged)
        //            {
        //                Drag(e.delta);
        //                e.Use();
        //                return true;
        //            }
        //            break;
        //    }

        //    return false;
        //}

        //private void ProcessContextMenu()
        //{
        //    GenericMenu genericMenu = new GenericMenu();
        //    genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        //    genericMenu.ShowAsContext();
        //}

        //private void OnClickRemoveNode()
        //{
        //    if (OnRemoveNode != null)
        //        OnRemoveNode(this);
        //}
    }
}