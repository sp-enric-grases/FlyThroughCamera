using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QGM.ScriptableExample
{
    public enum TypeOfNode
    {
        StartEnd,
        Path,
        Influencer,
        Trigger
    }

    public class ScriptableExManager : MonoBehaviour
    {
        public List<BaseNode> nodes;
        public List<StartEndNode> startEndNodes;
        public List<PathNode> pathNodes;
        public List<Connection> connections;
    }
}