using UnityEngine;
using System;
using System.Collections.Generic;

namespace QGM.Bezier
{
    public enum BezierControlPointMode { Free, Aligned, Mirrored }

    public class BezierSpline : MonoBehaviour
    {
        public Vector3 startNodePosition = new Vector3(1,1,1);
        public Vector3 endNodePosition = new Vector3(3,3,3);

        public List<Vector3> points;
        public List<Vector3> newPoints = new List<Vector3>();

        public int CurveCount
        {
            get { return (points.Count - 1) / 3; }
        }

        public int ControlPointCount
        {
            get { return points.Count; }
        }

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];

                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Count)
                {
                    points[index + 1] += delta;
                }
            }
            points[index] = point;
            EnforceMode(index);
        }

        private void EnforceMode(int index)
        {
            if (index == 0 || index == 1 || index == points.Count - 2 || index == points.Count - 1) return;

            int modeIndex = (index + 1) / 3;
            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0) fixedIndex = points.Count - 2;

                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Count) enforcedIndex = 1;
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Count) fixedIndex = 1;

                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0) enforcedIndex = points.Count - 2;
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            points[enforcedIndex] = middle + enforcedTangent;
        }

        public Vector3 GetPoint(float t)
        {
            int i = GetPointValue(ref t);
            return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }

        public Vector3 GetLocalPoint(float t)
        {
            return GetPoint(t) - transform.position;
        }

        public Vector3 GetVelocity(float t)
        {
            int i = GetPointValue(ref t);
            return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        private int GetPointValue(ref float t)
        {
            int i;

            if (t >= 1f)
            {
                t = 1f;
                i = points.Count - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }

            return i;
        }

        public void AddNewPoint(int p)
        {
            points.InsertRange(p, new List<Vector3>() { GetLocalPoint(0.4f), GetLocalPoint(0.5f), GetLocalPoint(0.6f)});
            newPoints.Add(GetLocalPoint(0.5f));
            EnforceMode(p+1);
            
        }

        public void DeletePoint(int p)
        {
            points.RemoveRange(p * 3 + 2, 3);
            newPoints.RemoveAt(p);
        }

        public void Reset()
        {
            Vector3 n = ((endNodePosition - startNodePosition) * 0.25f) + startNodePosition;
            Vector3 m = ((endNodePosition - startNodePosition) * 0.75f) + startNodePosition;
            points = new List<Vector3>() { startNodePosition, n, m, endNodePosition };
        }
    }
}
