using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {

        #region Fields

        private Camera _cam;

        #endregion

        #region Serialize Fields
        
        [SerializeField] private GameObject plane;
        [SerializeField] private float distance = 6;
        [SerializeField] private float elevation = 3;
        [SerializeField] private float angle = 10;

        #endregion

        #region Unity Functions

        private void Start()
        {
            _cam = Camera.main;
        }

        private void FixedUpdate()
        {
            var camTransform = _cam.transform;
            
            // TODO: do this better
            camTransform.position = plane.transform.position - plane.transform.forward * distance;
            camTransform.Translate(elevation * Vector3.up);
            camTransform.rotation = plane.transform.rotation;
            camTransform.Rotate(Vector3.right, angle);        

            // print("Camera position: " + cam.transform.position);
            // print("Camera rotation: " + cam.transform.rotation.eulerAngles);
        
            // print("Plane position: " + plane.transform.position);
            // print("Plane rotation: " + plane.transform.rotation.eulerAngles);
        }

        #endregion
    }
}