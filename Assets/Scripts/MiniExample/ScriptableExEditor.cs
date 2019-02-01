using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace QGM.ScriptableExample
{
    public class ScriptableExEditor : EditorWindow
    {
        private GameObject selectedObject;
        private ScriptableExManager sm;
        private List<BaseNode> nodes;
        private List<StartEndNode> startEndNodes;
        private List<PathNode> pathNodes;
        private List<Connection> connections;

        private Vector2 offset;
        private Vector2 drag;

        private ConnectionPoint selectedInPoint;
        private ConnectionPoint selectedOutPoint;

        void OnEnable()
        {
            selectedObject = (GameObject)Selection.activeObject;

            if (selectedObject != null)
            {
                sm = selectedObject.GetComponent<ScriptableExManager>();

                if (sm.nodes == null)
                    InitNodeLists();
                else
                {
                    RecoveringNodes();
                    //RecoveringConnections();
                }

                SetNodeLists();
            }
        }

        private void InitNodeLists()
        {
            sm.nodes = new List<BaseNode>();
            sm.startEndNodes = new List<StartEndNode>();
            sm.pathNodes = new List<PathNode>();
            sm.connections = new List<Connection>();
        }

        private void SetNodeLists()
        {
            nodes = sm.nodes;
            startEndNodes = sm.startEndNodes;
            pathNodes = sm.pathNodes;
            connections = sm.connections;
        }

        private void RecoveringNodes()
        {
            for (int i = 0; i < sm.nodes.Count; i++)
            {
                sm.nodes[i].OnClickRemoveNodeEvent(OnClickRemoveNode);
                //sm.startEndNodes[i].CreateConnections(OnClickInPoint, OnClickOutPoint, false);
            }
        }

        private void RecoveringConnections()
        {
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

        private ConnectionPoint GetConnection(ConnectionPointType type, string id)
        {
            switch (type)
            {
                case ConnectionPointType.NodeIn: return sm.startEndNodes.Find(i => i.id == id).inPoint;
                case ConnectionPointType.NodeOut: return sm.startEndNodes.Find(i => i.id == id).outPoint;
                default: return null;
            }
        }

        private void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            ProcessNodeEvents(Event.current);
            DrawNodes();

            DrawConnectionLine(Event.current);
            DrawConnections();

            ProcessEvents(Event.current);

            if (GUI.changed)
            {
                Repaint();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        private void DrawNodes()
        {
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
            switch (nodes[id].typeOfNode)
            {
                case TypeOfNode.StartEnd: startEndNodes.Find(n => n.id == nodes[id].id).DrawWindow(); break;
                case TypeOfNode.Path: pathNodes.Find(n => n.id == nodes[id].id).DrawWindow(); break;
            }

            GUI.DragWindow();
        }

        private void ProcessNodeEvents(Event e)
        {
            if (nodes != null)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodes[i].ProcessEvents(e);
                    if (guiChanged) GUI.changed = true;
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
            genericMenu.AddItem(new GUIContent("Add Start-End Node"), false, () => OnClickAddStartEndNode(mousePosition));
            genericMenu.AddItem(new GUIContent("Add Path"), false, () => OnClickAddPath(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddStartEndNode(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 205, 80);
            StartEndNode node = new StartEndNode(rect, GUID.Generate().ToString(), "Start-End Node", TypeOfNode.StartEnd, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint);

            nodes.Add(node);
            sm.startEndNodes.Add(node);
        }

        private void OnClickAddPath(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 250, 150);
            PathNode node = new PathNode(rect, GUID.Generate().ToString(), "Path", TypeOfNode.Path, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint);

            nodes.Add(node);
            sm.pathNodes.Add(node);
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint;

            if (selectedOutPoint != null)
            {
                if (selectedInPoint.GetOppositeConnection() == selectedOutPoint.type && selectedInPoint.node.id != selectedOutPoint.node.id)
                    CreateConnection();

                ClearConnectionSelection();
            }
            else
                Debug.Log("<color=red>[FLY-TROUGH]</color> selectedOutPoint == NULL");
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint;

            if (selectedInPoint != null)
            {
                if (selectedOutPoint.GetOppositeConnection() == selectedInPoint.type && selectedInPoint.node.id != selectedOutPoint.node.id)
                    CreateConnection();

                ClearConnectionSelection();
            }
            else
                Debug.Log("<color=red>[FLY-TROUGH]</color> selectedInPoint == NULL");
        }

        private void CreateConnection()
        {
            // We need to check first if the connection exists (this feature is missing!!)
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
            switch (node.typeOfNode)
            {
                case TypeOfNode.StartEnd: startEndNodes.Remove(startEndNodes.Find(n => n.id == node.id)); break;
                case TypeOfNode.Path: pathNodes.Remove(pathNodes.Find(n => n.id == node.id)); break;
            }

            nodes.Remove(node);
            //RemoveRelatedConnections(node);
        }

        private void RemoveRelatedConnections(BaseNode node)
        {
            foreach (var item in connections.ToList())
            {
                if (item.idIn == node.id || item.idOut == node.id)
                    connections.Remove(item);
            }
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