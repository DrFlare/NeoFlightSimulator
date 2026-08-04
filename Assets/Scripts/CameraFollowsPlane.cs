using System;
using UnityEngine;

public class CameraFollowsPlane : MonoBehaviour
{
    public GameObject plane;
    public float distance = 6;
    public float elevation = 3;
    public float angle = 10;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cam.transform.position = plane.transform.position - plane.transform.forward * distance;

        cam.transform.Translate(elevation * Vector3.up);
        
        cam.transform.rotation = plane.transform.rotation;
                                 
        cam.transform.Rotate(Vector3.right, angle);        

        // print("Camera position: " + cam.transform.position);
        // print("Camera rotation: " + cam.transform.rotation.eulerAngles);
        
        // print("Plane position: " + plane.transform.position);
        // print("Plane rotation: " + plane.transform.rotation.eulerAngles);
    }
}