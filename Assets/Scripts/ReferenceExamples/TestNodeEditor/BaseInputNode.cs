using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestNodeEditor
{
    [System.Serializable]
    public class BaseInputNode : BaseNode
    {
        public virtual string GetResult()
        {
            return "None";
        }

        public override void DrawCurves()
        {

        }
    }
}
