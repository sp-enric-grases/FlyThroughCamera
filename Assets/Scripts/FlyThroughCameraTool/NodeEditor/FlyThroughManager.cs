using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    public enum TypeOfNode
    {
        StartEnd,
        Path,
        Influencer,
        Trigger
    }

    public class FlyThroughManager : MonoBehaviour
    {
        public List<Node> nodes;
        public List<StartEndNode> startEndNodes;
        public List<PathNode> pathNodes;
        public List<Connection> connections;
    }
}