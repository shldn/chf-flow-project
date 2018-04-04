using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowtrailController : MonoBehaviour {

	public static List<FlowtrailController> all = new List<FlowtrailController>();


	[SerializeField] private GameObject flowTrailPrefab = null;
	private List<Flowtrail> flowTrails = new List<Flowtrail>();
	[Range(1, 3000)] [SerializeField] private int trailCount = 5;

	[Range(0.1f, 1f)] [SerializeField] private float trailWidth = 0.2f; public float TrailWidth { get { return trailWidth; } }
	[Range(0.02f, 1f)] [SerializeField] private float trailSpeed = 0.1f; public float TrailSpeed { get { return trailSpeed; } }
	[Range(0.5f, 1f)] [SerializeField] private float trailOrbitDist = 0.5f; public float TrailOrbitDist { get { return trailOrbitDist; } }

	[SerializeField] private Color color = new Color(); public Color MyColor { get { return color; } }
	public void SetColor(Color color){ this.color = color; }

	private Renderer myRenderer = null;


	private void Awake(){
		all.Add(this);
		myRenderer = GetComponent<Renderer>();
		if(myRenderer)
			myRenderer.material.SetColor ("_EmissionColor", MyColor);

	} // End of Awake().


	private void Update() {
		while(flowTrails.Count < trailCount){
			Flowtrail newFlowtrail = Instantiate(flowTrailPrefab, transform.position, Quaternion.identity).GetComponent<Flowtrail>();
			flowTrails.Add(newFlowtrail);
			newFlowtrail.Create(this);
		}

		while(flowTrails.Count > trailCount){
			int removeIndex = Random.Range(0, flowTrails.Count);
			Destroy(flowTrails[removeIndex]);
			flowTrails.RemoveAt(removeIndex);
		}

		//trailCount = (int)Mathf.Lerp(10, 3000, MuseManager.Inst.LastConcentrationMeasure);
	} // End of Update().


	public void SetCount(float intensity){
		trailCount = Mathf.FloorToInt(Mathf.Lerp(30f, 1200f, intensity));
	} // End of SetCount().

	public void SetWidth(float intensity){
		trailWidth = Mathf.Lerp(0.03f, 0.7f, intensity);
	} // End of SetWidth().

	public void SetSpeed(float intensity){
		trailSpeed = Mathf.Lerp(0.02f, 0.1f, intensity);
	} // End of SetSpeed().

} // End of FlowlineController.
