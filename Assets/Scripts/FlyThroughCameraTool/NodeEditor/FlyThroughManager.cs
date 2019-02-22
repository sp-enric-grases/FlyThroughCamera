using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    public enum TypeOfNode
    {
        Start,
        End,
        Battle,
        Path,
        Influencer,
        Trigger
    }

    public class FlyThroughManager : MonoBehaviour
    {
        public Camera cam;
        public List<Node> nodes;
        public StartNode startNode;
        public EndNode endNode;
        public List<BattleNode> battleNodes;
        public List<PathNode> pathNodes;
        public List<Connection> connections;
    }
}