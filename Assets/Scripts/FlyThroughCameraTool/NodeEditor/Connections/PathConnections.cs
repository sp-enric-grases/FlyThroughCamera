using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    [ExecuteInEditMode]
    public class PathConnections : MonoBehaviour
    {
        public List<BezierSpline> pathsIn = new List<BezierSpline>();
        public List<BezierSpline> pathsOut = new List<BezierSpline>();

        private void Update()
        {
            if (transform.hasChanged)
                UpdateConnections();
        }

        private void UpdateConnections()
        {
            foreach (var item in pathsIn)
                item.SetControlPoint(item.points.Count - 1, transform.position - item.transform.position);

            foreach (var item in pathsOut)
                item.SetControlPoint(0, transform.position-item.transform.position);
        }

        public void MoveNode()
        {
            Vector3 pos = transform.position;
            transform.position = Vector3.zero;
            transform.position = pos;
        }
    }
}
