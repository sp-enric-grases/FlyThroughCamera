using System;
using UnityEditor;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    public enum ConnectionIO { In, Out}

    [Serializable]
    public class Connection
    {
        public string idIn;
        public string idOut;
        [HideInInspector]
        public ConnectionPoint inPoint;
        [HideInInspector]
        public ConnectionPoint outPoint;
        [NonSerialized] public Action<Connection> OnClickRemoveConnection;

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection, bool isNew)
        {
            idIn = inPoint.node.id;
            idOut = outPoint.node.id;

            CreateConnection(inPoint, outPoint, OnClickRemoveConnection, true);
        }

        public void CreateConnection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection, bool isNew)
        {
            //if (isNew)
            //    Debug.Log("<color=green>[FLY-TROUGH]</color> Creating connection between nodes");
            //else
            //    Debug.Log("<color=green>[FLY-TROUGH]</color> Recovering connection between nodes");

            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;

            inPoint.LinkConnection(ConnectionIO.In, idIn);
            outPoint.LinkConnection(ConnectionIO.Out, idOut);
        }

        public void Draw()
        {
            Handles.DrawBezier(
                inPoint.rect.center,
                outPoint.rect.center,
                inPoint.rect.center + Vector2.left * 50f,
                outPoint.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                if (OnClickRemoveConnection != null)
                {
                    OnClickRemoveConnection(this);
                    inPoint.UnlinkConnection(ConnectionIO.In);
                    outPoint.UnlinkConnection(ConnectionIO.Out);
                }
            }
        }
    }
}
