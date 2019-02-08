using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialPoint.Tools.FlyThrough
{
    public enum TypeOfNode
    {
        StartEnd,
        Path,
        Influencer,
        Trigger
    }

    public class StartEndNodeGO
    {
        public GameObject node;
        public StartEndNode startEndNode;
    }

    public class FlyThroughManager_OLD : MonoBehaviour
    {
        public FlyThrough flyThrough;
        public List<GameObject> startEndNode;
        public List<BaseNode> baseNodes;
        //public List<StartEndNodeGO> startEndNodes;
        public List<PathNode> pathNodes;

        void Start()
        {
            foreach (var item in flyThrough.nodes)
            {
                Debug.Log(item.id);
            }
        }

        void Update()
        {
        }

        public void CreateNode(StartEndNode sen)
        {
            Debug.Log("HELLLO");
            //StartEndNodeGO startEndNode = new StartEndNodeGO { node = new GameObject(), startEndNode = sen };
        }
    }
}