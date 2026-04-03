using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 cameraOffset;
    public float pitch = 2f;
    private float currentZoom = 5f;
    public float zoomSpeed = 1f;
    public float maxZoom = 15f;
    public float minZoom = 5f;
    public float yawSpeed = 100f;
    private float currentYaw;
    [Range(1f,8f)] public float currentFOV = 6f;
    public float changeFOVSpeed = 1f;
    private void Update()
    {
        if (!Camera.main.orthographic)
        {   
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        if (Input.GetMouseButton(2))
        { 
        currentYaw += Input.GetAxis("Mouse X") * yawSpeed * Time.deltaTime;
        }
        }
        else
        {
            Camera.main.orthographicSize = currentFOV;
            currentFOV -= Input.GetAxis("Mouse ScrollWheel") * changeFOVSpeed;
            currentFOV = Mathf.Clamp(currentFOV, 1f, 8f);
            
        }
    }


    void LateUpdate()
    {
        transform.position = target.position - cameraOffset * currentZoom;
        transform.LookAt(target.position + Vector3.up * pitch);

        transform.RotateAround(target.position, Vector3.up, currentYaw);
    }
}
