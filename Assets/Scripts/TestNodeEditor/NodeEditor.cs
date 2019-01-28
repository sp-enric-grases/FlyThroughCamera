﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TestNodeEditor
{
    public class NodeEditor : EditorWindow
    {
        private List<BaseNode> windows = new List<BaseNode>();
        private Vector2 mousePos;
        private BaseNode selectedNode;
        private bool makeTransitionMode = false;

        [MenuItem("Window/Node Editor", false)]
        static void Init()
        {
            NodeEditor window = GetWindow<NodeEditor>();
            window.titleContent = new GUIContent("Fly Through");
        }

        [MenuItem("Window/Node Editor", true)]
        static bool InitValidator()
        {
            try
            {
                NodeList flyThrough = (NodeList)Selection.activeObject;
                return true;
            }
            catch { return false; }
        }

        void OnEnable()
        {
            NodeList flyThrough = (NodeList)Selection.activeObject;
            if (flyThrough.listOfNodes == null) flyThrough.listOfNodes = new List<BaseNode>();

            windows = flyThrough.listOfNodes;
        }

        private void OnGUI()
        {
            Event e = Event.current;
            mousePos = e.mousePosition;

            if (e.button == 1 && !makeTransitionMode)
            {
                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    int selectIndex = -1;

                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (windows[i].windowRect.Contains(mousePos))
                        {
                            selectIndex = i;
                            clickedOnWindow = true;
                            break;
                        }
                    }

                    if (!clickedOnWindow)
                    {
                        GenericMenu menu = new GenericMenu();

                        menu.AddItem(new GUIContent("Add Input Node"), false, ContextCallback, "inputNode");
                        menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallback, "outputNode");
                        menu.AddItem(new GUIContent("Calculation Node"), false, ContextCallback, "calcNode");
                        menu.AddItem(new GUIContent("Comparison Node"), false, ContextCallback, "compNode");

                        menu.ShowAsContext();
                        e.Use();
                    }
                    else
                    {
                        GenericMenu menu = new GenericMenu();

                        menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

                        menu.ShowAsContext();
                        e.Use();
                    }
                }
            }
            else if (e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode)
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow && !windows[selectIndex].Equals(selectedNode))
                {
                    windows[selectIndex].SetInput((BaseInputNode)selectedNode, mousePos);
                    makeTransitionMode = false;
                    selectedNode = null;
                }

                if (!clickedOnWindow)
                {
                    makeTransitionMode = false;
                    selectedNode = null;
                }

                e.Use();
            }
            else if (e.button == 0 && e.type == EventType.MouseDown && !makeTransitionMode)
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    BaseInputNode nodeToChange = windows[selectIndex].CLickedOnIput(mousePos);

                    if (nodeToChange != null)
                    {
                        selectedNode = nodeToChange;
                        makeTransitionMode = true;
                    }
                }
            }

            if (makeTransitionMode && selectedNode != null)
            {
                Rect mouseRect = new Rect(e.mousePosition.x, mousePos.y, 10, 10);
                DrawNodeCurve(selectedNode.windowRect, mouseRect);
                Repaint();
            }

            foreach (BaseNode n in windows)
            {
                n.DrawCurves();
            }

            BeginWindows();

            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }

            EndWindows();
        }

        void DrawNodeWindow(int id)
        {
            windows[id].DrawWindow();
            GUI.DragWindow();
        }

        private void ContextCallback(object obj)
        {
            string clb = obj.ToString();

            if (clb.Equals("inputNode"))
            {
                InputNode inputNode = new InputNode();
                inputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 150);

                windows.Add(inputNode);
            }
            //else if (clb.Equals("outputNode"))
            //{
            //    OutputNode outputNode = new OutputNode();
            //    outputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            //    windows.Add(outputNode);
            //}
            //else if (clb.Equals("calcNode"))
            //{
            //    CalcNode calcNode = new CalcNode();
            //    calcNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            //    windows.Add(calcNode);
            //}
            //else if (clb.Equals("compNode"))
            //{
            //    ComparisonNode compNode = new ComparisonNode();
            //    compNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            //    windows.Add(compNode);
            //}
            else if (clb.Equals("makeTransition"))
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    selectedNode = windows[selectIndex];
                    makeTransitionMode = true;
                }
            }
            else if (clb.Equals("deleteNode"))
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    BaseNode selNode = windows[selectIndex];
                    windows.RemoveAt(selectIndex);

                    foreach (BaseNode n in windows)
                    {
                        n.NodeDeleted(selNode);
                    }
                }
            }
        }

        public static void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.6f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, (i + 1) * 2);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }
    }
}