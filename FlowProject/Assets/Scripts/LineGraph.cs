using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGraph : MonoBehaviour {


	private LineRenderer myLineRenderer = null;
	private List<float> dataPoints = new List<float>();
	[SerializeField] private int numDataPoints = 100;

	[SerializeField] private float takeDataRate = 10f;
	private float takeDataCooldown = 0f;

	public float sample = 0f;


	// Use this for initialization
	void Awake () {
		myLineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		takeDataCooldown -= Time.deltaTime;
		while(takeDataCooldown <= 0f){
			takeDataCooldown += (1f / takeDataRate);

			dataPoints.Add(sample);

			if(dataPoints.Count > numDataPoints)
				dataPoints.RemoveAt(0);
		}

		Vector3[] graphPositions = new Vector3[dataPoints.Count];
		for(int i = 0; i < dataPoints.Count; i++){
			graphPositions[i] = transform.position + Vector3.Scale(transform.localScale, (transform.right * i) + (transform.up * dataPoints[i] * 10f));
		}

		myLineRenderer.positionCount = graphPositions.Length;
		myLineRenderer.SetPositions(graphPositions);
		
	}
}
