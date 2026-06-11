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

    private bool inCinematic;
    private Transform cinematicFocusTarget;
    private Vector3 cinematicFollowPos;
    private float cinematicZoomTarget;
    private float cinematicFOVTarget;
    private float cinematicSpeed;

    public void StartCinematic(Transform focusTarget, float zoomTarget, float fovTarget, float speed)
    {
        cinematicFocusTarget = focusTarget;
        cinematicFollowPos = target.position;
        cinematicZoomTarget = zoomTarget;
        cinematicFOVTarget = fovTarget;
        cinematicSpeed = speed;
        inCinematic = true;
    }

    private void Update()
    {
        if (inCinematic) return;

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
            currentFOV -= Input.GetAxis("Mouse ScrollWheel") * changeFOVSpeed;
            currentFOV = Mathf.Clamp(currentFOV, 1f, 8f);
        }
    }


    void LateUpdate()
    {
        Vector3 followPos = target.position;

        if (inCinematic)
        {
            float t = cinematicSpeed * Time.unscaledDeltaTime;
            cinematicFollowPos = Vector3.Lerp(cinematicFollowPos, cinematicFocusTarget.position, t);
            currentZoom = Mathf.Lerp(currentZoom, cinematicZoomTarget, t);
            currentFOV = Mathf.Lerp(currentFOV, cinematicFOVTarget, t);
            followPos = cinematicFollowPos;
        }

        if (Camera.main.orthographic)
            Camera.main.orthographicSize = currentFOV;

        transform.position = followPos - cameraOffset * currentZoom;
        transform.LookAt(followPos + Vector3.up * pitch);
        transform.RotateAround(followPos, Vector3.up, currentYaw);
    }
}
