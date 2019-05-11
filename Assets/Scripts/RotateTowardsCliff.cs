using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCliff : MonoBehaviour {

	// Use this for initialization
	// public float lowerRotationLimit = 120f;
	// public float higherRotationLimit = 240f;
	public bool TEST_BOOL = false;
	public GameObject vrCameraRig;

	private void Start ()
    {
        HandleLevel2Rotation();
    }

	private void Update() {
		if (TEST_BOOL) {
            HandleLevel2Rotation();
		}
	}
    private void HandleLevel2Rotation()
    {
        TEST_BOOL = false;
        vrCameraRig.transform.Rotate(Vector3.up, -transform.rotation.eulerAngles.y);
    }
}
