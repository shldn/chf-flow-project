using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowtrail : MonoBehaviour {

	[SerializeField] private TrailRenderer trail = null; public TrailRenderer Trail { get { return trail; } }

	private Vector3 position = Vector3.zero;
	private Vector3 targetPosition = Vector3.zero;
	private Vector3 positionVel = Vector3.zero;

	private Vector3 linkedPosition = Vector3.zero;
	private Vector3 targetLinkedPosition = Vector3.zero;
	private Vector3 linkedPositionVel = Vector3.zero;

	private float linkedSmoothTime = 0f;

	private Quaternion rotation = Quaternion.identity;
	private float orbitSpeed = 0f;
	private float orbitDist = 0f;
	private float orbitDistVel = 0f;
	private float targetOrbitDist = 0f;

	private float width = 0f;
	private float widthMult = 0f;
	private float widthVel = 0f;

	private Vector3 rotationAxis = Vector3.zero;
	private Vector3 targetRotationAxis = Vector3.zero;
	private Vector3 targetRotAxisVel = Vector3.zero;

	private List<FlowtrailController> controllers = new List<FlowtrailController>();

	private Color trailColor = Color.black;
	private Color tintColor = Color.black;
	private Color targetTrailColor = Color.black;
	private Color trailColorVel = Color.black;

	float smoothTime = 1f;


	float transferCooldown = 0f;

	
	public void Create(FlowtrailController controller){
		AssignToController(controller);
		position = controller.transform.position;
		targetPosition = position;
	} // End of Create().


	public void AssignToController(FlowtrailController controller){
		if(!controllers.Contains(controller))
			controllers.Add(controller);
	} // End of AssignToController().


	private void Start() {
		rotation = Random.rotationUniform;
		rotationAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		targetRotationAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		orbitSpeed = Random.Range(45f, 720f);
		targetOrbitDist = Random.Range(2f, 10f);
		widthMult = Random.Range(0.05f, 0.2f);
		
		smoothTime = Random.Range(0.5f, 5f);

		linkedSmoothTime = Random.Range(0.05f, 0.5f);

		tintColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));

		//transferCooldown = Random.Range(0f, 60f);
	} // End of Awake().


	private void Update() {

		Vector3 orbitPosAverage = Vector3.zero;
		float trailOrbitSpeedAverage = 0f;
		float trailOrbitDistAverage = 0f;
		float trailWidthAverage = 0f;
		Color trailColorAverage = Color.black;
		for(int i = 0; i < controllers.Count; i++){
			orbitPosAverage += controllers[i].transform.position;
			trailOrbitSpeedAverage += controllers[i].TrailSpeed;
			trailOrbitDistAverage += controllers[i].TrailOrbitDist;
			trailWidthAverage += controllers[i].TrailWidth;
			trailColorAverage += controllers[i].MyColor;
		}
		orbitPosAverage /= (float)controllers.Count;
		//trailOrbitSpeedAverage /= (float)controllers.Count;
		trailOrbitDistAverage /= Mathf.Pow((float)controllers.Count, 2f);
		trailWidthAverage /= (float)controllers.Count;
		trailColorAverage /= (float)controllers.Count;

		rotation *= Quaternion.AngleAxis(Time.deltaTime * orbitSpeed * trailOrbitSpeedAverage, rotationAxis);
		rotationAxis.x = Mathf.SmoothDamp(rotationAxis.x, targetRotationAxis.x, ref targetRotAxisVel.x, smoothTime);
		rotationAxis.y = Mathf.SmoothDamp(rotationAxis.y, targetRotationAxis.y, ref targetRotAxisVel.y, smoothTime);
		rotationAxis.z = Mathf.SmoothDamp(rotationAxis.z, targetRotationAxis.z, ref targetRotAxisVel.z, smoothTime);
		orbitDist = Mathf.SmoothDamp(orbitDist, targetOrbitDist * trailOrbitDistAverage, ref orbitDistVel, smoothTime);

		if(Vector3.Angle(rotationAxis, targetRotationAxis) < 1f)
			targetRotationAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));


		targetPosition = /*orbitPosAverage + */(rotation * Vector3.forward * orbitDist);
		position = Vector3.SmoothDamp(position, targetPosition, ref positionVel, smoothTime);
		Vector3 localPosition = (controllers[0].transform.rotation * position);
		targetLinkedPosition = controllers[0].transform.position + (localPosition.normalized * Mathf.Clamp(localPosition.magnitude, 0.5f, Mathf.Infinity));
		linkedPosition = Vector3.SmoothDamp(linkedPosition, targetLinkedPosition, ref linkedPositionVel, linkedSmoothTime);
		transform.position = linkedPosition;



		width = Mathf.SmoothDamp(width, widthMult * trailWidthAverage, ref widthVel, smoothTime);
		Trail.startWidth = width;
		Trail.endWidth = width;


		targetTrailColor = Color.Lerp(trailColorAverage, tintColor, 0.35f);
		trailColor.r = Mathf.SmoothDamp(trailColor.r, targetTrailColor.r, ref trailColorVel.r, smoothTime);
		trailColor.g = Mathf.SmoothDamp(trailColor.g, targetTrailColor.g, ref trailColorVel.g, smoothTime);
		trailColor.b = Mathf.SmoothDamp(trailColor.b, targetTrailColor.b, ref trailColorVel.b, smoothTime);
		Gradient trailGradient = new Gradient();
		trailGradient.SetKeys(
			//new GradientColorKey[]{ new GradientColorKey(trailColor, 0f), new GradientColorKey(trailColor, 1f) }, new GradientAlphaKey[]{ new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.5f), new GradientAlphaKey(0f, 1f) }
			new GradientColorKey[]{ new GradientColorKey(trailColor, 0f), new GradientColorKey(Color.black, 1f) }, new GradientAlphaKey[]{ new GradientAlphaKey(1f, 1f), new GradientAlphaKey(0f, 1f) }
		);
		trail.colorGradient = trailGradient;


		/*
		transferCooldown = Mathf.MoveTowards(transferCooldown, 0f, Time.deltaTime);
		if(transferCooldown == 0f){
			AssignToController(FlowtrailController.all[Random.Range(0, FlowtrailController.all.Count)]);
			transferCooldown = Random.Range(0f, 60f);
		}
		*/

	} // End of Update().

} // End of FlowTrail.
