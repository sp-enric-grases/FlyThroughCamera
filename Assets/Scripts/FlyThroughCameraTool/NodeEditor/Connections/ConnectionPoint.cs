using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    public enum TypeOfConnection { NodeIn, NodeOut, PathIn, PathOut }

    [Serializable]
    public class ConnectionPoint
    {
        [NonSerialized] public BaseNode node;
        [NonSerialized] public Rect rect;
        public TypeOfConnection type;
        public TypeOfConnection oppositeConnection;
        [NonSerialized] public GUIStyle style;
        [NonSerialized] public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(BaseNode node, TypeOfConnection type, TypeOfConnection oppositeConnection, Action<ConnectionPoint> OnClickConnectionPoint)
        {
            this.node = node;
            this.type = type;
            this.oppositeConnection = oppositeConnection;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 10f, 20f);

            DefineStyles();
        }

        private void DefineStyles()
        {
            style = new GUIStyle();

            switch (type)
            {
                case TypeOfConnection.NodeIn:
                case TypeOfConnection.PathIn:
                    style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
                    style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
                    break;

                case TypeOfConnection.NodeOut:
                case TypeOfConnection.PathOut:
                    style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
                    style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
                    break;
            }

            style.border = new RectOffset(4, 4, 12, 12);
        }

        public TypeOfConnection GetOppositeConnection()
        {
            return oppositeConnection;
        }

        public void LinkConnection(ConnectionIO connection, BaseNode id)
        {
            node.LinkConnection(connection, id);
        }

        public void UnlinkConnection(ConnectionIO connection, BaseNode id)
        {
            node.UnlinkConnection(connection, id);
        }

        public void Draw()
        {
            rect.y = node.windowRect.y + (node.windowRect.height * 0.5f) - rect.height * 0.5f;

            switch (type)
            {
                case TypeOfConnection.NodeIn:
                case TypeOfConnection.PathIn:
                    rect.x = node.windowRect.x - rect.width;
                    break;

                case TypeOfConnection.NodeOut:
                case TypeOfConnection.PathOut:
                    rect.x = node.windowRect.x + node.windowRect.width;
                    break;
            }

            if (GUI.Button(rect, "", style))
            {
                if (OnClickConnectionPoint != null)
                    OnClickConnectionPoint(this);
                //else
                //    Debug.Log("<color=red>[FLY-TROUGH]</color> OnClickConnectionPoint == NULL");
            }
        }
    }
}