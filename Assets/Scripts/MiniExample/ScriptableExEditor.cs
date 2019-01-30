using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    public class ScriptableExEditor : EditorWindow
    {
        private GameObject selectedObject;
        private ScriptableExManager sm;
        private List<BaseNode> nodes;
        private List<Connection> connections;

        private Vector2 offset;
        private Vector2 drag;

        private ConnectionPoint selectedInPoint;
        private ConnectionPoint selectedOutPoint;

        [MenuItem("Window/FlyTC Editor")]
        private static void OpenWindow()
        {
            ScriptableExEditor window = GetWindow<ScriptableExEditor>();
            window.titleContent = new GUIContent("Node Based Editor");
        }

        void OnEnable()
        {
            selectedObject = (GameObject)Selection.activeObject;

            if (selectedObject != null)
            {
                sm = selectedObject.GetComponent<ScriptableExManager>();

                if (sm.nodes == null)
                {
                    sm.nodes = new List<BaseNode>();
                    sm.connections = new List<Connection>();
                }
                else
                {
                    for (int i = 0; i < sm.nodes.Count; i++)
                    {
                        sm.nodes[i].OnClickRemoveNodeEvent(OnClickRemoveNode);
                        sm.nodes[i].CreateConnections(OnClickInPoint, OnClickOutPoint, false);
                    }

                    for (int i = 0; i < sm.connections.Count; i++)
                    {
                        sm.connections[i].CreateConnection
                            (
                            GetConnection(ConnectionPointType.NodeIn, sm.connections[i].idIn),
                            GetConnection(ConnectionPointType.NodeOut, sm.connections[i].idOut),
                            OnClickRemoveConnection, false
                            );
                    }
                }

                connections = sm.connections;
                nodes = sm.nodes;
            }
        }

        private ConnectionPoint GetConnection(ConnectionPointType type, string id)
        {
            switch (type)
            {
                case ConnectionPointType.NodeIn: return sm.nodes.Find(i => i.id == id).inPoint;
                case ConnectionPointType.NodeOut: return sm.nodes.Find(i => i.id == id).outPoint;
                default: return null;
            }
            
        }

        private void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            BeginWindows();

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].windowRect = GUI.Window(i, nodes[i].windowRect, DrawNodeList, nodes[i].title);
                    nodes[i].DrawNodes();
                }
            }

            EndWindows();

            DrawConnections();

            DrawConnectionLine(Event.current);
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawConnections()
        {
            if (connections == null) return;

            for (int i = 0; i < connections.Count; i++)
                connections[i].Draw();
        }

        private void DrawConnectionLine(Event e)
        {
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier (selectedInPoint.rect.center, e.mousePosition, selectedInPoint.rect.center + Vector2.left * 50f, e.mousePosition - Vector2.left * 50f, Color.white, null, 3f);
                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(selectedOutPoint.rect.center, e.mousePosition, selectedOutPoint.rect.center - Vector2.left * 50f, e.mousePosition + Vector2.left * 50f, Color.white, null, 3f);
                GUI.changed = true;
            }
        }

        void DrawNodeList(int id)
        {
            //switch (nodeList[id].typeOfNode)
            //{
            //    case TypeOfNode.StartEnd: startEndNodes.Find(n => n.id == nodeList[id].id).DrawWindow(); break;
            //    case TypeOfNode.Path: pathNodes.Find(n => n.id == nodeList[id].id).DrawWindow(); break;
            //}
            nodes[id].DrawWindow();
            GUI.DragWindow();
        }

        private void ProcessNodeEvents(Event e)
        {
            if (nodes != null)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodes[i].ProcessEvents(e);

                    Debug.Log(guiChanged);

                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        ClearConnectionSelection();
                    }

                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }
                    break;
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Drag(delta);
                }
            }

            GUI.changed = true;
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Start-End Node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 250, 100);
            BaseNode node = new BaseNode(rect, OnClickRemoveNode, "Start-End Node", OnClickInPoint, OnClickOutPoint, GUID.Generate().ToString());

            nodes.Add(node);
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            //Debug.Log("Click In Point: " + inPoint.rect);
            selectedInPoint = inPoint;
            //Debug.Log("Click In Point: " + selectedInPoint.rect);

            if (selectedOutPoint != null)
            {
                if (selectedInPoint.GetOppositeConnection() == selectedOutPoint.type && selectedInPoint.node.id != selectedOutPoint.node.id)
                //if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
            else
                Debug.Log("<color=red>[FLY-TROUGH]</color> selectedOutPoint == NULL");
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            //Debug.Log("Click Out Point: " + outPoint.rect);
            selectedOutPoint = outPoint;
            //Debug.Log("Click Out Point: " + selectedOutPoint.rect);

            if (selectedInPoint != null)
            {
                if (selectedInPoint.GetOppositeConnection() == selectedOutPoint.type && selectedInPoint.node.id != selectedOutPoint.node.id)
                //if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
            else
                Debug.Log("<color=red>[FLY-TROUGH]</color> selectedInPoint == NULL");
        }

        private void CreateConnection()
        {
            //if (connections == null) connections = new List<Connection>();
            connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection, true));
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
        }

        private void OnClickRemoveNode(BaseNode node)
        {
            //switch (node.typeOfNode)
            //{
            //    case TypeOfNode.StartEnd: startEndNodes.Remove(startEndNodes.Find(n => n.id == node.id)); break;
            //    case TypeOfNode.Path: pathNodes.Remove(pathNodes.Find(n => n.id == node.id)); break;
            //}

            nodes.Remove(node);
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);

            for (int j = 0; j < heightDivs; j++)
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}