using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SocialPoint.Tools.FlyThrough
{
    [Serializable]
    public class PathNode : BaseNode
    {
        public GameObject node;

        public Camera cam;
        public float timeToRelocate = 1;
        public AnimationCurve curveRelocation = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float pathDuration;
        public AnimationCurve curvePath = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public PathNode(Rect rect, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, string title)
        {
            this.typeOfNode = typeOfNode;
            windowRect = rect;
            OnRemoveNode = OnClickRemoveNode;
            this.title = title;

            CreatePathNode();
        }

        public override void DrawWindow()
        {
            base.DrawWindow();
            name = EditorGUILayout.TextField("Name", name);
            node = EditorGUILayout.ObjectField("Node", node, typeof(GameObject), true) as GameObject;
            cam = EditorGUILayout.ObjectField("Node", cam, typeof(Camera), true) as Camera;
            timeToRelocate = EditorGUILayout.FloatField("Time to Recolate", timeToRelocate);
            curveRelocation = EditorGUILayout.CurveField("Relocation ", curveRelocation);
            pathDuration = EditorGUILayout.FloatField("Path duration time", pathDuration);
            curvePath = EditorGUILayout.CurveField("Curve path", curvePath);
        }

        private void CreatePathNode()
        {
            if (name == "") name = "Path Node";

            if (node == null)
            {
                node = new GameObject();
                //node.AddComponent<FlyThroughPath>();
                node.name = name;
            }
        }
    }
}