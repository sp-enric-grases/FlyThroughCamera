﻿using System;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    public enum ConnectionPointType { NodeIn, NodeOut, PathIn, PathOut }

    [Serializable]
    public class ConnectionPoint
    {
        [NonSerialized] public BaseNode node;
        public Rect rect;
        public ConnectionPointType type;
        public ConnectionPointType oppositeConnection;
        [NonSerialized] public GUIStyle style;
        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(BaseNode node, ConnectionPointType type, ConnectionPointType typeOut, Action<ConnectionPoint> OnClickConnectionPoint)
        {
            this.node = node;
            this.type = type;
            this.oppositeConnection = typeOut;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 10f, 20f);

            DefineStyles();
        }

        private void DefineStyles()
        {
            style = new GUIStyle();

            switch (type)
            {
                case ConnectionPointType.NodeIn:
                case ConnectionPointType.PathIn:
                    style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
                    style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
                    break;

                case ConnectionPointType.NodeOut:
                case ConnectionPointType.PathOut:
                    style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
                    style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
                    break;
            }

            style.border = new RectOffset(4, 4, 12, 12);
        }

        public ConnectionPointType GetOppositeConnection()
        {
            return oppositeConnection;
        }

        public void Draw()
        {
            rect.y = node.windowRect.y + (node.windowRect.height * 0.5f) - rect.height * 0.5f;

            switch (type)
            {
                case ConnectionPointType.NodeIn:
                case ConnectionPointType.PathIn:
                    rect.x = node.windowRect.x - rect.width;
                    break;

                case ConnectionPointType.NodeOut:
                case ConnectionPointType.PathOut:
                    rect.x = node.windowRect.x + node.windowRect.width;
                    break;
            }

            if (GUI.Button(rect, "", style))
            {
                if (OnClickConnectionPoint != null)
                    OnClickConnectionPoint(this);
                else
                    Debug.Log("<color=red>[FLY-TROUGH]</color> OnClickConnectionPoint == NULL");
            }
        }
    }
}