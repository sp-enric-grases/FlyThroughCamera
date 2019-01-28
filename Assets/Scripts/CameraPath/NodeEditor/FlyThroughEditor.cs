using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SocialPoint.Tools.FlyThrough
{
    public class FlyThroughEditor : EditorWindow
    {
        private GameObject ftmObject;
        private FlyThroughManager ftm;
        private FlyThrough ft;
        
        private List<BaseNode> nodeList;
        private List<StartEndNode> startEndNodes;
        private List<PathNode> pathNodes;
        private List<Connection> connections;
        private ConnectionPoint selectedInPoint;
        private ConnectionPoint selectedOutPoint;
        private GUIStyle inPointStyle;
        private GUIStyle outPointStyle;
        private Vector2 offset;
        private Vector2 drag;

        [MenuItem("Window/Fly Through Editor", false)]
        static void Init()
        {
            FlyThroughEditor window = GetWindow<FlyThroughEditor>();
            window.titleContent = new GUIContent("Fly Through");
        }

        [MenuItem("Window/Fly Through Editor", true)]
        static bool InitValidator()
        {
            try
            {
                FlyThrough ft = (FlyThrough)Selection.activeObject;
                return true;
            }
            catch { return false; }
        }

        void OnEnable()
        {
            ftmObject = (GameObject)Selection.activeObject;
            ftm = ftmObject.GetComponent<FlyThroughManager>();
            ft = ftm.flyThrough;
            
            if (ft.nodes == null)           ft.nodes = new List<BaseNode>();
            if (ft.startEndNodes == null)   ft.startEndNodes = new List<StartEndNode>();
            if (ft.pathNodes == null)       ft.pathNodes = new List<PathNode>();

            nodeList = ft.nodes;
            startEndNodes = ft.startEndNodes;
            pathNodes = ft.pathNodes;

            DefineStyles();
        }

        private void DefineStyles()
        {
            inPointStyle = new GUIStyle();
            inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inPointStyle.border = new RectOffset(4, 4, 12, 12);

            outPointStyle = new GUIStyle();
            outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outPointStyle.border = new RectOffset(4, 4, 12, 12);
        }

        private void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            DrawConnections();
            DrawConnectionLine(Event.current);

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            BeginWindows();

            for (int i = 0; i < nodeList.Count; i++)
                nodeList[i].windowRect = GUI.Window(i, nodeList[i].windowRect, DrawNodeList, nodeList[i].title);

            EndWindows();

            if (GUI.changed) Repaint();
        }

        void DrawNodeList(int id)
        {
            switch (nodeList[id].typeOfNode)
            {
                case TypeOfNode.StartEnd: startEndNodes.Find(n => n.id == nodeList[id].id).DrawWindow(); break;
                case TypeOfNode.Path: pathNodes.Find(n => n.id == nodeList[id].id).DrawWindow(); break;
            }

            GUI.DragWindow();
        }

        private void DrawConnectionLine(Event e)
        {
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(
                    selectedOutPoint.rect.center,
                    e.mousePosition,
                    selectedOutPoint.rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        private void DrawConnections()
        {
            if (connections != null)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].Draw();
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

            if (nodeList != null)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    nodeList[i].Drag(delta);
                }
            }

            GUI.changed = true;
        }

        private void ProcessNodeEvents(Event e)
        {
            if (nodeList != null)
            {
                for (int i = nodeList.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodeList[i].ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Start-End Node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.AddItem(new GUIContent("Add Path"), false, () => OnClickAddPath(mousePosition));
            genericMenu.AddItem(new GUIContent("Add Influencer"), false, () => OnClickAddInfluencer(mousePosition));
            genericMenu.AddItem(new GUIContent("Add Trigger"), false, () => OnClickAddTrigger(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 250, 100);
            StartEndNode node = new StartEndNode(rect, TypeOfNode.StartEnd, OnClickRemoveNode, "Start-End Node", name, GUID.Generate(), ftm);

            nodeList.Add(node);
            ft.startEndNodes.Add(node);
        }

        private void OnClickAddPath(Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition.x, mousePosition.y, 250, 150);
            PathNode node = new PathNode(rect, TypeOfNode.Path, OnClickRemoveNode, "Path");

            nodeList.Add(node);
            ft.pathNodes.Add(node);
        }

        private void OnClickAddInfluencer(Vector2 mousePosition)
        {
            //nodeList.nodes.Add(new BaseNode(mousePosition, 60, 70, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
        }

        private void OnClickAddTrigger(Vector2 mousePosition)
        {
            //nodeList.nodes.Add(new BaseNode(mousePosition, 100, 30, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint;

            if (selectedOutPoint != null)
            {
                if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint;

            if (selectedInPoint != null)
            {
                if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
        }

        private void CreateConnection()
        {
            if (connections == null)
            {
                connections = new List<Connection>();
            }

            connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        private void OnClickRemoveNode(BaseNode node)
        {
            //if (connections != null)
            //{
            //    List<Connection> connectionsToRemove = new List<Connection>();

            //    for (int i = 0; i < connections.Count; i++)
            //    {
            //        if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
            //        {
            //            connectionsToRemove.Add(connections[i]);
            //        }
            //    }

            //    for (int i = 0; i < connectionsToRemove.Count; i++)
            //    {
            //        connections.Remove(connectionsToRemove[i]);
            //    }

            //    connectionsToRemove = null;
            //}

            switch (node.typeOfNode)
            {
                case TypeOfNode.StartEnd: startEndNodes.Remove(startEndNodes.Find(n => n.id == node.id)); break;
                case TypeOfNode.Path: pathNodes.Remove(pathNodes.Find(n => n.id == node.id)); break;
            }

            nodeList.Remove(node);

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