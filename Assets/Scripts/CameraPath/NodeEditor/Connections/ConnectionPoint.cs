using System;
using UnityEngine;

namespace SocialPoint.Tools.FlyThrough
{
    public enum ConnectionPointType { In, Out}

    [Serializable]
    public class ConnectionPoint
    {
        public Rect rect;
        public ConnectionPointType type;
        public BaseNode node;
        [HideInInspector]
        public GUIStyle style;
        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(BaseNode node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
        {
            this.node = node;
            this.type = type;
            this.style = style;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 10f, 20f);
        }

        public void Draw()
        {
            rect.y = node.windowRect.y + (node.windowRect.height * 0.5f) - rect.height * 0.5f;

            switch (type)
            {
                case ConnectionPointType.In:
                    rect.x = node.windowRect.x - rect.width + 8f;
                    break;

                case ConnectionPointType.Out:
                    rect.x = node.windowRect.x + node.windowRect.width - 8f;
                    break;
            }

            if (GUI.Button(rect, "", style))
            {
                if (OnClickConnectionPoint != null)
                {
                    OnClickConnectionPoint(this);
                }
            }
        }
    }
}