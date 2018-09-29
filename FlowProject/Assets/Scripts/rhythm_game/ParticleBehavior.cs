using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehavior : MonoBehaviour {

    public float duration;
    private float durationEnd;

	// Use this for initialization
	void Start () {
        durationEnd = Time.time + duration;	
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > durationEnd)
            Destroy(gameObject);
	}
}
