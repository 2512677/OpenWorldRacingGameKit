using UnityEngine;
using System.Collections;

namespace RGSK {
    
    [RequireComponent(typeof(Camera))]
    public class MinimapCamera : MonoBehaviour
    {
        public Transform target;
        public bool followPosition = true;
        public bool followRotation = true;
        private float height = 100;

        private void Start()
        {
            height = GetComponent<Camera>().farClipPlane / 2;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }



        void LateUpdate()
        {
            if (!target)
                return;

            if (followPosition)
                transform.position = new Vector3(target.position.x, height, target.position.z);

            if (followRotation)         
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z);                     
        }



        public void SetTarget(Transform t)
        {
            target = t;
        }
    }
}
