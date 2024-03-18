using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 0, -12f);
    private float smoothTime = 0.25f; //Smoothness of the Camera Follow (Time for Camera to reach Target Position)
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //SmoothDamp used to smoothly interpolate the current camera position
        //'ref velocity' stores current velocity
    }
}
