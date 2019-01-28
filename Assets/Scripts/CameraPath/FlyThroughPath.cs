using System.Collections.Generic;
using UnityEngine;

namespace SocialPoint.Tools
{
    public delegate void SplineFinishedHandler();

    [RequireComponent(typeof(BezierSpline))]
    public class FlyThroughPath : MonoBehaviour
    {
        public event SplineFinishedHandler splineFinishedEvent;

        public Camera cam;
        public float timeToRelocate = 1;
        public AnimationCurve curveRelocation = AnimationCurve.EaseInOut(0, 0, 1, 1);


        public float pathDuration;
        public AnimationCurve curvePath = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public bool follow;
        public bool refFollow = true;
        public List<Influencer> inf = new List<Influencer>();

        private List<Influencer> influencers = new List<Influencer>();
        private float progress;
        private bool startMovement = false;
        private BezierSpline spline;
        private GameObject reference;
        private float counterRelocation = 0;
        private bool cameraIsRelocated = false;

        private void Start()
        {
            spline = gameObject.GetComponent<BezierSpline>();
            //reference = new GameObject();
            
            foreach (var item in inf)
            {
                if (item != null && item.enableInfluencer)
                    influencers.Add(item);
            }

            foreach (var item in influencers)
            {
                SphereCollider collider = item.gameObject.AddComponent<SphereCollider>();
                collider.radius = item.areaOfInfluence;
                collider.isTrigger = true;
            }

            //CreateCameraCollider();
        }

        public void RunPath()
        {
            startMovement = true;
            cam.GetComponent<CameraRotation>().enabled = false;
            reference = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cam.gameObject.AddComponent<InfluencerDetection>().Init(this, reference);
        }

        private void Update()
        {
            if (startMovement)
            {
                progress += Time.deltaTime / pathDuration;

                Vector3 position = spline.GetPoint(curvePath.Evaluate(progress));
                cam.transform.localPosition = position;
                Vector3 lookAt = position + spline.GetDirection(curvePath.Evaluate(progress));
                
                if (refFollow) reference.transform.LookAt(lookAt);

                RelocateCamera(lookAt);

                if (progress > 1f) EndOfPath();
            }
        }

        private void EndOfPath()
        {
            startMovement = false;
            //splineFinishedEvent();
            cam.GetComponent<CameraRotation>().enabled = true;
            Destroy(reference);
            Destroy(cam.GetComponent<InfluencerDetection>());
        }

        private void RelocateCamera(Vector3 lookAt)
        {
            float angleDifference = Quaternion.Angle(cam.transform.rotation, reference.transform.rotation);

            if (angleDifference > 0.1f && !cameraIsRelocated)
            {
                counterRelocation += Time.deltaTime / timeToRelocate;
                cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, reference.transform.rotation, curveRelocation.Evaluate(counterRelocation));
            }
            else
            {
                cameraIsRelocated = true;
                if (follow) cam.transform.LookAt(lookAt);
            }
        }

        private void CreateCameraCollider()
        {
            float COLLIDER_DETECTION_SIZE = 0.1f;
            SphereCollider c = cam.gameObject.GetComponent<SphereCollider>() ? cam.gameObject.GetComponent<SphereCollider>() : c = cam.gameObject.AddComponent<SphereCollider>();
            Rigidbody r = cam.gameObject.GetComponent<Rigidbody>() ? cam.gameObject.GetComponent<Rigidbody>() : cam.gameObject.AddComponent<Rigidbody>() ;

            c.radius = COLLIDER_DETECTION_SIZE;
            c.isTrigger = true;
            r.useGravity = false;
            r.isKinematic = true;
        }
    }
}