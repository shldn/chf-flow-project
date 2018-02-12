using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowtrail : MonoBehaviour {

	[SerializeField] private TrailRenderer trail = null; public TrailRenderer Trail { get { return trail; } }
	private Quaternion rotation = Quaternion.identity;
	private float orbitSpeed = 0f;
	private float orbitDist = 0f;
	private float orbitDistVel = 0f;
	private float targetOrbitDist = 0f;

	private Vector3 rotationAxis = Vector3.zero;
	private Vector3 targetRotationAxis = Vector3.zero;
	private Vector3 targetRotAxisVel = Vector3.zero;

	private FlowtrailController controller = null;


	public void AssignToController(FlowtrailController controller){
		this.controller = controller;
	} // End of AssignToController().


	private void Awake() {
		targetRotationAxis = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
		orbitSpeed = Random.Range(90f, 720f);
		targetOrbitDist = Random.Range(4f, 5f);
		//orbitDist = 5f;


		Color trailColor = new Color(Random.Range(100f, 255f), Random.Range(100f, 255f), Random.Range(100f, 255f));

		/*
		Gradient trailGradient = new Gradient();
		trailGradient.SetKeys(
			new GradientColorKey[]{ new GradientColorKey(trailColor, 0f), new GradientColorKey(trailColor, 1f) }, new GradientAlphaKey[]{ new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
		);
		trail.colorGradient = trailGradient;
		*/

		//trail.material.SetColor("_TintColor", trailColor);

	} // End of Awake().


	private void Update() {
		rotation *= Quaternion.AngleAxis(Time.deltaTime * orbitSpeed * controller.TrailSpeed, rotationAxis);
		float smoothTime = 1f;
		rotationAxis.x = Mathf.SmoothDamp(rotationAxis.x, targetRotationAxis.x, ref targetRotAxisVel.x, smoothTime);
		rotationAxis.y = Mathf.SmoothDamp(rotationAxis.y, targetRotationAxis.y, ref targetRotAxisVel.y, smoothTime);
		rotationAxis.z = Mathf.SmoothDamp(rotationAxis.z, targetRotationAxis.z, ref targetRotAxisVel.z, smoothTime);
		orbitDist = Mathf.SmoothDamp(orbitDist, targetOrbitDist * controller.TrailOrbitDist, ref orbitDistVel, smoothTime);

		if(Vector3.Angle(rotationAxis, targetRotationAxis) < 1f)
			targetRotationAxis = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

		transform.position = controller.transform.position + (rotation * Vector3.forward * orbitDist);
		Trail.startWidth = controller.TrailWidth;
		Trail.endWidth = controller.TrailWidth;
	} // End of Update().

} // End of FlowTrail.
