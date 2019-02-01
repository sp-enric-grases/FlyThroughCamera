using UnityEngine;
using UnityEngine.UI;

namespace SocialPoint.Tools
{
    public class CameraRotation : MonoBehaviour
    {
        const int MAX_ANGLE = 360;
        const int SENSIBILITY = 1000;

        public float offsetRotX = 0;
        public Vector2 limitX = new Vector2(0, 360);
        public Vector2 intLimitX;
        public float offsetRotY = 0;
        public Vector2 limitY = new Vector2(-85, 85);
        public Vector2 intLimitY;

        public float sensibility = 2;
        public bool invertDirection = false;
        public bool inertia = true;
        public float decelerationRate = 1f;

        public Text a, b;

        private float rotX = 0.0f;
        private float rotY = 0.0f;
        private float velRecover = 0f;

        void Start()
        {
            SetInitRotations(transform.eulerAngles);
        }

        public void SetInitRotations(Vector3 rotation)
        {
            if (transform.localEulerAngles.y >= 0 && transform.localEulerAngles.y < 180)
                rotX = transform.localEulerAngles.y;
            else
			        {
						if (offsetRotX > 180)
							rotX = transform.localEulerAngles.y;
						else
							rotX = -(MAX_ANGLE - transform.localEulerAngles.y);
					}  

            if (transform.localEulerAngles.x >= 0 && transform.localEulerAngles.x < 180)
                rotY = -transform.localEulerAngles.x;
            else
                rotY = MAX_ANGLE - transform.localEulerAngles.x;
        }

        void Update()
        {
            #if UNITY_STANDALONE || UNITY_EDITOR
            GetInput_StandAlone();
            #elif UNITY_IOS || UNITY_ANDROID
            GetInput_Devive();
            #endif
            transform.eulerAngles = new Vector3(-rotY, rotX, 0.0f);
        }

        private void GetInput_StandAlone()
        {
            if (Input.GetMouseButton(0))
            {
                rotX += Input.GetAxis("Mouse X") * (invertDirection ? 1 : -1);
                rotY += Input.GetAxis("Mouse Y") * (invertDirection ? 1 : -1);

                GetLimits();
            }
            else
                RecoverPosition();
        }

        private void GetInput_Devive()
        {
            if (Input.touchCount == 1)
            {
                rotX += Input.GetTouch(0).deltaPosition.x / GetComponent<Camera>().pixelWidth * Time.deltaTime * sensibility * SENSIBILITY;
                rotY += Input.GetTouch(0).deltaPosition.y / GetComponent<Camera>().pixelHeight * Time.deltaTime * sensibility * SENSIBILITY;

                GetLimits();
            }
            else
                RecoverPosition();
        }

        private void GetLimits()
        {
            if (!inertia)
            {
                rotX = Mathf.Clamp(rotX, intLimitX.x, intLimitX.y);
                rotY = Mathf.Clamp(rotY, intLimitY.x, intLimitY.y);
            }
        }

        private void RecoverPosition()
        {
            if (rotX > intLimitX.y) rotX = GetSmoothDamp(rotX, intLimitX.y);
            if (rotX < intLimitX.x) rotX = GetSmoothDamp(rotX, intLimitX.x);
            if (rotY > intLimitY.y) rotY = GetSmoothDamp(rotY, intLimitY.y);
            if (rotY < intLimitY.x) rotY = GetSmoothDamp(rotY, intLimitY.x);
        }

        private float GetSmoothDamp(float current, float target)
        {
            return Mathf.SmoothDamp(current, target, ref velRecover, Time.deltaTime * decelerationRate);
        }
    }
}