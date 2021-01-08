using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour {
    public Vector3 cameraOffset;
    public GameObject objectToFollow;
    public float rotateSensitivity = 10f;
    public float RotationSmoothFactor = 5f;

    private void LateUpdate() {
        Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotateSensitivity, Vector3.up);
        // if (Input.GetKeyDown("e")) camTurnAngle *= Quaternion.Euler(0, -90, 0);
        // if (Input.GetKeyDown("q")) camTurnAngle *= Quaternion.Euler(0, 90, 0);
        
        cameraOffset = camTurnAngle * cameraOffset;
        var newPos = objectToFollow.transform.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, RotationSmoothFactor);
        transform.LookAt(objectToFollow.transform);
    }
}