using UnityEngine;

namespace SocialPoint.Tools
{
    public class InfluencerDetection : MonoBehaviour
    {
        private Influencer inf;
        private Transform target;
        private FlyThroughPath path;

        private bool targetIsDetected = false;
        private bool targetIsExiting = false;
        private float counterEnter = 0;
        private float counterExit = 0;
        private GameObject targetReference;

        public void Init(FlyThroughPath path, GameObject reference)
        {
            this.path = path;
            targetReference = reference;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Influencer>())
            {
                inf = other.gameObject.GetComponent<Influencer>();

                if (inf.useAlternativeTarget)
                    target = inf.alternativeTarget.transform;
                else
                {
                    GameObject go = new GameObject();
                    go.transform.position = inf.targetPosition;
                    target = go.transform;
                }

                counterEnter = 0;
                targetIsDetected = true;
                path.GetComponent<FlyThroughPath>().follow = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Influencer>())
            {
                counterExit = 0;
                targetIsDetected = false;
                targetIsExiting = true;
                path.GetComponent<FlyThroughPath>().refFollow = true;
            }
        }

        private void Update()
        {
            targetReference.transform.position = gameObject.transform.position;

            if (targetIsDetected)
            {
                targetReference.transform.LookAt(target);
                counterEnter += Time.deltaTime / inf.timeToTargeting;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetReference.transform.rotation, inf.targetingCurve.Evaluate(counterEnter));
            }

            if (targetIsExiting)
            {
                counterExit += Time.deltaTime / inf.timeToRelax;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetReference.transform.rotation, inf.relaxingCurve.Evaluate(counterExit));
            }
        }
    }
}