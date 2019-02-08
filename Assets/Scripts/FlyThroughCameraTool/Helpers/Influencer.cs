using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QGM.FlyThrougCamera
{
    public class Influencer : MonoBehaviour
    {
        public Color orange = new Color(1, 0.5f, 0);
        public bool enableInfluencer = true;
        public float areaOfInfluence = 5;
        public bool useAlternativeTarget = false;
        public Vector3 targetPosition = Vector3.zero;
        public Transform alternativeTarget;
        public float timeToTargeting = 2;
        public AnimationCurve targetingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float timeToRelax = 2;
        public AnimationCurve relaxingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private void OnDrawGizmos()
        {
            Gizmos.color = orange;
            Gizmos.DrawWireSphere(transform.position, areaOfInfluence);
            Gizmos.DrawLine(transform.position, GetTargetPosition());
            Gizmos.DrawWireSphere(GetTargetPosition(), 1);
        }

        private Vector3 GetTargetPosition()
        {
            if (useAlternativeTarget)
                return alternativeTarget != null ? alternativeTarget.position : transform.position;
            else
                return transform.localPosition + targetPosition;
        }
    }
}
