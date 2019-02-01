using System.Collections.Generic;
using UnityEngine;

namespace SocialPoint.Tools
{
    public delegate void SplineFinishedHandler();

    [RequireComponent(typeof(BezierSpline))]
    public class FlyThroughPath : MonoBehaviour
    {
        public event SplineFinishedHandler SplineFinishedEvent;

        public Camera cam;
        public float pathDuration;
        public AnimationCurve curvePath = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public Collider trigger;

        public float timeToInitRelocatation = 1;
        public AnimationCurve curveInitRelocation = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public Vector3 finalRotation = Vector3.zero;
        public float timeToFinalRelocation = 1;
        public AnimationCurve curveFinalRelocation = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public bool follow = true;
        public bool refFollow = true;
        public List<Influencer> inf = new List<Influencer>();

        private List<Influencer> influencers = new List<Influencer>();
        private float progress;
        private bool startMovement = false;
        private BezierSpline spline;
        private GameObject reference;
        private GameObject finalReference;
        private float counterInitRelocation = 0;
        private float counterFinalRelocation = 0;
        private bool cameraIsRelocated = false;
        private float timeToEnd;

        private bool mouseHasBeenDrag = false;

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

            timeToEnd = 1 - timeToFinalRelocation/pathDuration;
            //CreateCameraCollider();

            CreateFinalReference();
        }

        private void  CreateFinalReference()
        {
            finalReference = new GameObject();//  GameObject.CreatePrimitive(PrimitiveType.Cube);
            finalReference.transform.position = spline.GetControlPoint(spline.ControlPointCount - 1) + spline.transform.position;
            finalReference.transform.rotation = spline.transform.rotation * Quaternion.Euler(finalRotation);
        }

        public void RunPath()
        {
            startMovement = true;
            cam.GetComponent<CameraRotation>().enabled = false;
            reference = new GameObject();//  GameObject.CreatePrimitive(PrimitiveType.Cube);
            cam.gameObject.AddComponent<InfluencerDetection>().Init(this, reference);
        }

        private void Update()
        {
            if (startMovement)
                MoveCameraAlongThePath();
            else
                WaitForTrigger();
        }

        private void MoveCameraAlongThePath()
        {
            progress += Time.deltaTime / pathDuration;

            Vector3 position = spline.GetPoint(curvePath.Evaluate(progress));
            cam.transform.localPosition = position;
            Vector3 lookAt = position + spline.GetDirection(curvePath.Evaluate(progress));

            if (refFollow) reference.transform.LookAt(lookAt);

            if (progress > timeToEnd)
                RelocateFinalCamera();
            else
                RelocateInitCamera(lookAt);

            if (progress > 1f) EndOfPath();
        }

        private void WaitForTrigger()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.blue);

                if (Input.GetMouseButtonUp(0) && !mouseHasBeenDrag && hit.collider == trigger)
                {
                    RunPath();
                    startMovement = true;
                }
            }

            if (Input.GetMouseButtonUp(0)) mouseHasBeenDrag = false;
        }

        void OnGUI()
        {
            if (Event.current.type == EventType.MouseDrag) mouseHasBeenDrag = true;
        }

        private void EndOfPath()
        {
            startMovement = false;

            if (SplineFinishedEvent != null)
                SplineFinishedEvent();

            SetCameraFinalPosition();
            Destroy(reference);
            Destroy(finalReference);
            Destroy(cam.GetComponent<InfluencerDetection>());
        }

        private void SetCameraFinalPosition()
        {
            Quaternion rot = cam.transform.rotation;
            CameraRotation rotation = cam.GetComponent<CameraRotation>();

            rotation.enabled = true;
            rotation.SetInitRotations(rot.eulerAngles);
            rotation.offsetRotX += rot.eulerAngles.y;
        }

        private void RelocateInitCamera(Vector3 lookAt)
        {
            float angleDifference = Quaternion.Angle(cam.transform.rotation, reference.transform.rotation);

            if (angleDifference > 0.1f && !cameraIsRelocated)
            {
                counterInitRelocation += Time.deltaTime / timeToInitRelocatation;
                cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, reference.transform.rotation, curveInitRelocation.Evaluate(counterInitRelocation));
            }
            else
            {
                cameraIsRelocated = true;
                cam.transform.LookAt(lookAt);
            }
        }

        private void RelocateFinalCamera()
        {
            counterFinalRelocation += Time.deltaTime / timeToFinalRelocation;
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, finalReference.transform.rotation, curveFinalRelocation.Evaluate(counterFinalRelocation));
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