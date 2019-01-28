using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SocialPoint.Tools.FlyThrough
{
    [Serializable]
    public class StartEndNode : BaseNode
    {
        private FlyThroughManager ftm;

        public StartEndNode(Rect rect, TypeOfNode typeOfNode, Action<BaseNode> OnClickRemoveNode, string title, string name, GUID id, FlyThroughManager ftm)
        {
            this.typeOfNode = typeOfNode;
            windowRect = rect;
            OnRemoveNode = OnClickRemoveNode;
            this.title = title;
            this.name = name;
            this.id = id;
            this.ftm = ftm;

            //CreateStartEndNode();
        }

        public override void DrawWindow()
        {
            base.DrawWindow();
            name = EditorGUILayout.TextField("Name", name);

            //if (node != null)
            //{
            //    node = EditorGUILayout.ObjectField("Node", node, typeof(GameObject), true) as GameObject;
            //    node.transform.position = EditorGUILayout.Vector3Field("Position", node.transform.position);
            //}
        }

        private void CreateStartEndNode()
        {
            ftm.CreateNode(this);
            //if (name == "") name = "Start-End Node";

            //if (node == null)
            //{
            //    node = new GameObject();
            //    node.AddComponent<CameraRotation>();
            //    node.name = name;
            //}
        }
    }
}