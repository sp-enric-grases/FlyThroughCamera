using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TestNodeEditor
{
    //[CreateAssetMenu(fileName = "Data", menuName = "HELLLLLLLLLLO", order = 1)]
    //[System.Serializable]
    public class NodeList : ScriptableObject
    {
        public List<BaseNode> listOfNodes;
    }
}
