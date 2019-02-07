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
                    RecoveringConnections();
                }
            }
        }

        private void InitNodeLists()
        {
            sm.nodes = new List<Node>();
            sm.startEndNodes = new List<StartEndNode>();
            sm.pathNodes = new List<PathNode>();
            sm.connections = new List<Connection>();
        }

        private void RecoveringNodes()
        {
            for (int id = 0; id < sm.nodes.Count; id++)
            {
                switch (sm.nodes[id].typeOfNode)
                {
                    case TypeOfNode.StartEnd:
                        StartEndNode sen = sm.startEndNodes.Find(n => n.id == sm.nodes[id].id);
                        sen.CreateConnections(OnClickInPoint, OnClickOutPoint, false);
                        sen.OnClickRemoveNodeEvent(OnClickRemoveNode);
                        break;
                    case TypeOfNode.Path:
                        PathNode pn = sm.pathNodes.Find(n => n.id == sm.nodes[id].id);
                        pn.CreateConnections(OnClickInPoint, OnClickOutPoint, false);
                        pn.OnClickRemoveNodeEvent(OnClickRemoveNode);
                        break;
                }
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
                case ConnectionPointType.PathIn: return sm.startEndNodes.Find(i => i.id == id).inPoint;
                case ConnectionPointType.PathOut: return sm.startEndNodes.Find(i => i.id == id).outPoint;
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

            for (int id = 0; id < sm.nodes.Count; id++)
            {
                switch (sm.nodes[id].typeOfNode)
                {
                    case TypeOfNode.StartEnd:
                        StartEndNode sen = sm.startEndNodes.Find(n => n.id == sm.nodes[id].id);
                        sen.windowRect = GUI.Window(id, sen.windowRect, DrawNodeList, sen.title);
                        sen.DrawNodes();
                        break;
                    case TypeOfNode.Path:
                        PathNode pn = sm.pathNodes.Find(n => n.id == sm.nodes[id].id);
                        pn.windowRect = GUI.Window(id, pn.windowRect, DrawNodeList, pn.title);
                        pn.DrawNodes();
                        break;
                }
            }

            EndWindows();
        }

        private void DrawConnections()
        {
            if (sm.connections == null) return;

            for (int i = 0; i < sm.connections.Count; i++)
                sm.connections[i].Draw();
        }

        private void DrawConnectionLine(Event e)
        {
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier (selectedInPoint.rect.center, e.mousePosition, selectedInPoint.rect.center + Vector2.left * 50f, e.mousePosition - Vector2.left * 50f, Color.white, null, 4f);
                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(selectedOutPoint.rect.center, e.mousePosition, selectedOutPoint.rect.center - Vector2.left * 50f, e.mousePosition + Vector2.left * 50f, Color.white, null, 4f);
                GUI.changed = true;
            }
        }

        void DrawNodeList(int id)
        {
            switch (sm.nodes[id].typeOfNode)
            {
                case TypeOfNode.StartEnd:   sm.startEndNodes.Find(n => n.id == sm.nodes[id].id).DrawWindow();   break;
                case TypeOfNode.Path:       sm.pathNodes.Find(n => n.id == sm.nodes[id].id).DrawWindow();       break;
            }

            GUI.DragWindow();
        }

        private void ProcessNodeEvents(Event e)
        {
            for (int id = 0; id < sm.nodes.Count; id++)
            {
                bool guiChanged = false;

                switch (sm.nodes[id].typeOfNode)
                {
                    case TypeOfNode.StartEnd:   guiChanged = sm.startEndNodes.Find(n => n.id == sm.nodes[id].id).ProcessEvents(e); break;
                    case TypeOfNode.Path:       guiChanged = sm.pathNodes.Find(n => n.id == sm.nodes[id].id).ProcessEvents(e);     break;
                }

                if (guiChanged) GUI.changed = true;
            }
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0) ClearConnectionSelection();
                    if (e.button == 1) ProcessContextMenu(e.mousePosition);
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0) OnDrag(e.delta);
                    break;
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;

            for (int id = 0; id < sm.nodes.Count; id++)
            {
                switch (sm.nodes[id].typeOfNode)
                {
                    case TypeOfNode.StartEnd:   sm.startEndNodes.Find(n => n.id == sm.nodes[id].id).Drag(delta);    break;
                    case TypeOfNode.Path:       sm.pathNodes.Find(n => n.id == sm.nodes[id].id).Drag(delta);        break;
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

            AddNode(node.id, node.typeOfNode);
            sm.startEndNodes.Add(node);
        }

        private void OnClickAddPath(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 250, 150);
            PathNode node = new PathNode(rect, GUID.Generate().ToString(), "Path", TypeOfNode.Path, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint);

            AddNode(node.id, node.typeOfNode);
            sm.pathNodes.Add(node);
        }

        private void AddNode(string id, TypeOfNode typeOfNode)
        {
            sm.nodes.Add(new Node() { id = id, typeOfNode = typeOfNode });
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
            sm.connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection, true));
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            sm.connections.Remove(connection);
        }

        private void OnClickRemoveNode(BaseNode node)
        {
            switch (node.typeOfNode)
            {
                case TypeOfNode.StartEnd:   sm.startEndNodes.Remove(sm.startEndNodes.Find(n => n.id == node.id));   break;
                case TypeOfNode.Path:       sm.pathNodes.Remove(sm.pathNodes.Find(n => n.id == node.id));           break;
            }

            sm.nodes.Remove(sm.nodes.Find(n => n.id == node.id));
            RemoveRelatedConnections(node);
        }

        private void RemoveRelatedConnections(BaseNode node)
        {
            foreach (var item in sm.connections.ToList())
            {
                if (item.idIn == node.id || item.idOut == node.id)
                    sm.connections.Remove(item);
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