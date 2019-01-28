using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TestNodeEditor
{
    //[CreateAssetMenu(fileName = "Data", menuName = "ASAAAAAAA", order = 1)]
    [System.Serializable]
    public class BaseNode// : ScriptableObject
    {
        public Rect windowRect;
        public bool hasInputs = false;
        public string windowTitle = "";

        public virtual void DrawWindow()
        {
            windowTitle = EditorGUILayout.TextField("Title", windowTitle);
        }

        public virtual void DrawCurves() { }

        public virtual void SetInput(BaseInputNode input, Vector2 clickPos)
        {

        }

        public virtual void NodeDeleted(BaseNode node)
        {

        }

        public virtual BaseInputNode CLickedOnIput(Vector2 pos)
        {
            return null;
        }
    }
}