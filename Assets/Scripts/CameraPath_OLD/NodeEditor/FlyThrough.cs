using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SocialPoint.Tools.FlyThrough
{
    public class FlyThrough : ScriptableObject
    {
        public List<BaseNode> nodes;
        public List<StartEndNode> startEndNodes;
        public List<PathNode> pathNodes;
    }
}