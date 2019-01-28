using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace SocialPoint.Tools
{
    //[Serializable]
    //public class BaseNode
    //{
    //    public Rect windowRect;
    //    public bool hasInputs = false;
    //    public string windowTitle = "";

    //    public virtual void DrawWindow()
    //    {
    //        windowTitle = EditorGUILayout.TextField("Title", windowTitle);
    //    }

    //    public virtual void DrawCurves() { }

    //    public virtual void SetInput(BaseInputNode input, Vector2 clickPos) {}

    //    public virtual void NodeDeleted(BaseNode node) { }

    //    public virtual BaseInputNode CLickedOnIput(Vector2 pos) { return null; }
    //}

    //[Serializable]
    //public class BaseInputNode : BaseNode
    //{
    //    public virtual string GetResult()
    //    {
    //        return "None";
    //    }

    //    public override void DrawCurves()
    //    {

    //    }
    //}

    //[Serializable]
    //public class StartEndNode : BaseInputNode
    //{
    //    public List<PathNode> inputNodes;
    //    public List<PathNode> outputNodes;
    //}

    //[Serializable]
    //public class PathNode : BaseInputNode
    //{
    //    public StartEndNode inputNode;
    //    public StartEndNode outputNode;
    //    public List<InfluencerNode> influencerNodes;
    //}

    //[Serializable]
    //public class InfluencerNode : BaseInputNode
    //{
    //    public PathNode pathNode;
    //}
}