using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField] private float orbitDist = 50f;
	[SerializeField] private float orbitSpeed = 2f;


	void Update () {
		transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * orbitSpeed, Vector3.up);
		transform.position = -transform.forward * orbitDist;
	} // End of Update().

} // End of CameraController.
