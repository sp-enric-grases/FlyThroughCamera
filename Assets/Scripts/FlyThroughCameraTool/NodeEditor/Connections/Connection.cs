using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    public enum ConnectionIO { In, Out}

    [Serializable]
    public class Connection
    {
        public string idOut;
        public string idIn;
        [HideInInspector] public ConnectionPoint outPoint;
        [HideInInspector] public ConnectionPoint inPoint;
        [NonSerialized] public Action<Connection> OnClickRemoveConnection;

        public Connection(ConnectionPoint outPoint, ConnectionPoint inPoint, Action<Connection> OnClickRemoveConnection, bool isNewConnection)
        {
            idOut = outPoint.node.id;
            idIn = inPoint.node.id;

            CreateConnection(outPoint, inPoint, OnClickRemoveConnection, isNewConnection);
        }

        public void CreateConnection(ConnectionPoint outPoint, ConnectionPoint inPoint, Action<Connection> OnClickRemoveConnection, bool isNewConnection)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;

            if (isNewConnection)
            {
                outPoint.LinkConnection(ConnectionIO.Out, inPoint.node);
                inPoint.LinkConnection(ConnectionIO.In, outPoint.node);
            }
        }

        public void Draw()
        {
            Handles.DrawBezier (outPoint.rect.center, inPoint.rect.center, outPoint.rect.center - Vector2.left * 50f, inPoint.rect.center + Vector2.left * 50f, Color.white, null, 2f);

            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                if (OnClickRemoveConnection != null)
                {
                    OnClickRemoveConnection(this);
                    UnlinkConnections();
                }
            }
        }

        public void UnlinkConnections()
        {
            outPoint.UnlinkConnection(ConnectionIO.Out, inPoint.node);
            inPoint.UnlinkConnection(ConnectionIO.In, outPoint.node);
        }
    }
}
