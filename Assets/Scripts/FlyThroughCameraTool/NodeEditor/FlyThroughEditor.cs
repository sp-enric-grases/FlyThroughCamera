using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    public class FlyThroughEditor : EditorWindow
    {
        private GameObject selectedObject;
        private FlyThroughManager ft;
        private Vector2 offset;
        private Vector2 drag;

        private ConnectionPoint selectedInPoint;
        private ConnectionPoint selectedOutPoint;

        void OnEnable()
        {
            selectedObject = (GameObject)Selection.activeObject;

            if (selectedObject != null)
            {
                ft = selectedObject.GetComponent<FlyThroughManager>();

                if (ft.nodes == null)
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
            ft.nodes = new List<Node>();
            ft.battleNodes = new List<BattleNode>();
            ft.pathNodes = new List<PathNode>();
            ft.connections = new List<Connection>();
        }

        private void RecoveringNodes()
        {
            for (int id = 0; id < ft.nodes.Count; id++)
            {
                switch (ft.nodes[id].typeOfNode)
                {
                    case TypeOfNode.Battle:
                        BattleNode sen = ft.battleNodes.Find(n => n.id == ft.nodes[id].id);
                        sen.CreateConnections(ft, OnClickInPoint, OnClickOutPoint, false);
                        sen.OnClickRemoveNodeEvent(OnClickRemoveNode);
                        break;
                    case TypeOfNode.Path:
                        PathNode pn = ft.pathNodes.Find(n => n.id == ft.nodes[id].id);
                        pn.CreateConnections(ft, OnClickInPoint, OnClickOutPoint, false);
                        pn.OnClickRemoveNodeEvent(OnClickRemoveNode);
                        break;
                }
            }
        }

        private void RecoveringConnections()
        {
            for (int i = 0; i < ft.connections.Count; i++)
            {
                ft.connections[i].CreateConnection
                (
                    GetConnection(ConnectionIO.Out, ft.connections[i].idOut),
                    GetConnection(ConnectionIO.In, ft.connections[i].idIn),
                    OnClickRemoveConnection, false
                );
            }
        }

        private ConnectionPoint GetConnection(ConnectionIO inOut, string idIn)
        {
            TypeOfNode connectionPoint = ft.nodes.Find(n => n.id == idIn).typeOfNode;

            if (inOut == ConnectionIO.In)
            {
                switch (connectionPoint)
                {
                    case TypeOfNode.Battle:   return ft.battleNodes.Find(n => n.id == idIn).inPoint;
                    case TypeOfNode.Path:       return ft.pathNodes.Find(n => n.id == idIn).inPoint;
                    default:                    return null;
                }
            }
            else
            {
                switch (connectionPoint)
                {
                    case TypeOfNode.Battle:   return ft.battleNodes.Find(n => n.id == idIn).outPoint;
                    case TypeOfNode.Path:       return ft.pathNodes.Find(n => n.id == idIn).outPoint;
                    default:                    return null;
                }
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

            for (int id = 0; id < ft.nodes.Count; id++)
            {
                switch (ft.nodes[id].typeOfNode)
                {
                    case TypeOfNode.Start:
                        StartNode sn = ft.startNode;
                        sn.windowRect = GUI.Window(id, sn.windowRect, DrawNodeList, sn.title);
                        sn.DrawNodes();
                        break;
                    case TypeOfNode.End:
                        EndNode en = ft.endNode;
                        en.windowRect = GUI.Window(id, en.windowRect, DrawNodeList, en.title);
                        en.DrawNodes();
                        break;
                    case TypeOfNode.Battle:
                        BattleNode sen = ft.battleNodes.Find(n => n.id == ft.nodes[id].id);
                        sen.windowRect = GUI.Window(id, sen.windowRect, DrawNodeList, sen.title);
                        sen.DrawNodes();
                        break;
                    case TypeOfNode.Path:
                        PathNode pn = ft.pathNodes.Find(n => n.id == ft.nodes[id].id);
                        pn.windowRect = GUI.Window(id, pn.windowRect, DrawNodeList, pn.title);
                        pn.DrawNodes();
                        break;
                }
            }

            EndWindows();
        }

        private void DrawConnections()
        {
            if (ft.connections == null) return;

            for (int i = 0; i < ft.connections.Count; i++)
                ft.connections[i].Draw();
        }

        private void DrawConnectionLine(Event e)
        {
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier (selectedInPoint.rect.center, e.mousePosition, selectedInPoint.rect.center - Vector2.left * 50f, e.mousePosition + Vector2.left * 50f, Color.black, null, 4f);
                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(selectedOutPoint.rect.center, e.mousePosition, selectedOutPoint.rect.center + Vector2.left * 50f, e.mousePosition - Vector2.left * 50f, Color.black, null, 4f);
                GUI.changed = true;
            }
        }

        void DrawNodeList(int id)
        {
            switch (ft.nodes[id].typeOfNode)
            {
                case TypeOfNode.Start:      ft.startNode.DrawWindow();                                          break;
                case TypeOfNode.End:        ft.endNode.DrawWindow();                                            break;
                case TypeOfNode.Battle:     ft.battleNodes.Find(n => n.id == ft.nodes[id].id).DrawWindow();   break;
                case TypeOfNode.Path:       ft.pathNodes.Find(n => n.id == ft.nodes[id].id).DrawWindow();       break;
            }

            GUI.DragWindow();
        }

        private void ProcessNodeEvents(Event e)
        {
            for (int id = 0; id < ft.nodes.Count; id++)
            {
                bool guiChanged = false;

                switch (ft.nodes[id].typeOfNode)
                {
                    case TypeOfNode.Start:      guiChanged = ft.startNode.ProcessEvents(e);                                         break;
                    case TypeOfNode.End:        guiChanged = ft.endNode.ProcessEvents(e);                                           break;
                    case TypeOfNode.Battle:     guiChanged = ft.battleNodes.Find(n => n.id == ft.nodes[id].id).ProcessEvents(e);  break;
                    case TypeOfNode.Path:       guiChanged = ft.pathNodes.Find(n => n.id == ft.nodes[id].id).ProcessEvents(e);      break;
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
                    if (e.button == 1) ContextMenu(e.mousePosition);
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0) OnDrag(e.delta);
                    break;
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;

            for (int id = 0; id < ft.nodes.Count; id++)
            {
                switch (ft.nodes[id].typeOfNode)
                {
                    case TypeOfNode.Start:      ft.startNode.Drag(delta);                                           break;
                    case TypeOfNode.End:        ft.endNode.Drag(delta);                                             break;
                    case TypeOfNode.Battle:     ft.battleNodes.Find(n => n.id == ft.nodes[id].id).Drag(delta);    break;
                    case TypeOfNode.Path:       ft.pathNodes.Find(n => n.id == ft.nodes[id].id).Drag(delta);        break;
                }
            }

            GUI.changed = true;
        }

        private void ContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();

            if (ft.nodes.Any(n => n.typeOfNode == TypeOfNode.Start))
                menu.AddDisabledItem(new GUIContent("Start Node"));
            else
                menu.AddItem(new GUIContent("Start Node"), false, () => OnClickAddStartNode(mousePosition));

            if (ft.nodes.Any(n => n.typeOfNode == TypeOfNode.End))
                menu.AddDisabledItem(new GUIContent("End Node"));
            else
                menu.AddItem(new GUIContent("End Node"), false, () => OnClickAddEndNode(mousePosition));

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Battle Node"), false, () => OnClickAddBattleNode(mousePosition));
            menu.AddItem(new GUIContent("Add Path"), false, () => OnClickAddPath(mousePosition));
            menu.ShowAsContext();
        }

        private void OnClickAddStartNode(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 205, 80);
            StartNode node = new StartNode(ft, rect, GUID.Generate().ToString(), "Start Node", TypeOfNode.Start, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint);

            AddNode(node.id, node.typeOfNode);
            ft.startNode = node;
        }

        private void OnClickAddEndNode(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 205, 80);
            EndNode node = new EndNode(ft, rect, GUID.Generate().ToString(), "End Node", TypeOfNode.End, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint);

            AddNode(node.id, node.typeOfNode);
            ft.endNode = node;
        }

        private void OnClickAddBattleNode(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 205, 80);
            BattleNode node = new BattleNode(ft, rect, GUID.Generate().ToString(), "Battle Node", TypeOfNode.Battle, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint);

            AddNode(node.id, node.typeOfNode);
            ft.battleNodes.Add(node);
        }

        private void OnClickAddPath(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 250, 150);
            PathNode node = new PathNode(ft, rect, GUID.Generate().ToString(), "Path", TypeOfNode.Path, OnClickRemoveNode, OnClickInPoint, OnClickOutPoint);

            AddNode(node.id, node.typeOfNode);
            ft.pathNodes.Add(node);
        }

        private void AddNode(string id, TypeOfNode typeOfNode)
        {
            ft.nodes.Add(new Node() { id = id, typeOfNode = typeOfNode });
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint;

            if (selectedOutPoint != null)
            {
                if (selectedInPoint.GetOppositeConnection() == selectedOutPoint.type && selectedInPoint.node.id != selectedOutPoint.node.id && ConnectionNotExist())
                    CreateConnection();

                ClearConnectionSelection();
            }
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint;

            if (selectedInPoint != null)
            {
                if (selectedOutPoint.GetOppositeConnection() == selectedInPoint.type && selectedInPoint.node.id != selectedOutPoint.node.id && ConnectionNotExist())
                    CreateConnection();

                ClearConnectionSelection();
            }
        }

        private bool ConnectionNotExist()
        {
            for (int c = 0; c < ft.connections.Count; c++)
            {
                if (ft.connections[c].idOut == selectedOutPoint.node.id && ft.connections[c].idIn == selectedInPoint.node.id ||
                    ft.connections[c].idIn == selectedOutPoint.node.id && ft.connections[c].idOut == selectedInPoint.node.id)
                    return false;
            }

            return true;
        }

        private void CreateConnection()
        {
            ft.connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection, true));
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            ft.connections.Remove(connection);
        }

        private void OnClickRemoveNode(BaseNode node)
        {
            switch (node.typeOfNode)
            {
                case TypeOfNode.Start:
                    DestroyImmediate(ft.startNode.node);
                    ft.startNode = null;
                    break;
                case TypeOfNode.Battle:
                    DestroyImmediate(ft.battleNodes.Find(n => n.id == node.id).node);
                    ft.battleNodes.Remove(ft.battleNodes.Find(n => n.id == node.id));
                    break;
                case TypeOfNode.Path:
                    DestroyImmediate(ft.pathNodes.Find(n => n.id == node.id).node);
                    ft.pathNodes.Remove(ft.pathNodes.Find(n => n.id == node.id));
                    break;
            }

            ft.nodes.Remove(ft.nodes.Find(n => n.id == node.id));
            RemoveRelatedConnections(node);
        }

        private void RemoveRelatedConnections(BaseNode node)
        {
            List<Connection> connections = ft.connections.ToList();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].idIn == node.id || connections[i].idOut == node.id)
                {
                    ft.connections[i].UnlinkConnections();
                    ft.connections.Remove(connections[i]);
                }
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