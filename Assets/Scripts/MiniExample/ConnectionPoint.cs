using System;
using UnityEditor;
using UnityEngine;

namespace QGM.ScriptableExample
{
    public enum ConnectionPointType { In, Out }

    [Serializable]
    public class ConnectionPoint
    {
        [NonSerialized] public BaseNode node;
        public string id;
        public Rect rect;
        public ConnectionPointType type;
        public GUIStyle style;
        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(BaseNode node, ConnectionPointType type, Action<ConnectionPoint> OnClickConnectionPoint)
        {
            this.node = node;
            this.type = type;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 10f, 20f);

            DefineStyles();
        }

        private void DefineStyles()
        {
            style = new GUIStyle();

            if (type == ConnectionPointType.In)
            {
                style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
                style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            }
            else
            {
                style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
                style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            }
            style.border = new RectOffset(4, 4, 12, 12);
        }

        public void Draw()
        {
            rect.y = node.windowRect.y + (node.windowRect.height * 0.5f) - rect.height * 0.5f;

            switch (type)
            {
                case ConnectionPointType.In:
                    rect.x = node.windowRect.x - rect.width;
                    break;

                case ConnectionPointType.Out:
                    rect.x = node.windowRect.x + node.windowRect.width;
                    break;
            }

            if (GUI.Button(rect, "", style))
            {
                if (OnClickConnectionPoint != null)
                    OnClickConnectionPoint(this);
            }
        }
    }
}