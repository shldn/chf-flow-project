using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDeflectorBehavior : MonoBehaviour {

    public float velocitySensitivity;
    private Vector3 initialPosDeflector;
    public Vector3 velocityDeflector { get; private set; }
    private Rigidbody rb;
    private Renderer rend;
    public float handSize;

    // Use this for initialization
    void Start () {
        initialPosDeflector = transform.position;
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        // velocity is calculated to affect the enemy draft on collision
        velocityDeflector = (transform.position - initialPosDeflector) / Time.deltaTime * velocitySensitivity;
        
        rb.velocity = velocityDeflector;
        Debug.Log(rb.velocity);
        initialPosDeflector = transform.position;

        //transform.localPosition = Vector3.zero;
        // Color the speed for debugging
        rend.material.SetColor("_EmissionColor", Color.Lerp(Color.green, Color.red, Mathf.Clamp(rb.velocity.magnitude, 0f, 10f)));
    }
}
