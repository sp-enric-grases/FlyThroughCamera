using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialPoint.Tools
{
    public class PathManager : MonoBehaviour
    {
        public List<FlyThroughPath> paths = new List<FlyThroughPath>();

        private bool start = false;
        private bool loadPath = false;
        private int numPaths = 0;

        //void Start()
        //{
        //    foreach (var item in paths)
        //        item.splineFinishedEvent += EndOfPath;
        //}

        void Update()
        {

            if (loadPath && numPaths < paths.Count)
            {
                paths[numPaths].RunPath();
                numPaths++;
                loadPath = false;
            }
        }

        public void EndOfPath()
        {
            loadPath = true;
        }
    }
}