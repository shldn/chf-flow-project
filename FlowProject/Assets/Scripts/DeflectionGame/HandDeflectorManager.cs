using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDeflectorManager : MonoBehaviour {

    public Vector2 sizeRange;
    public GameObject handDeflectorRight;
    public GameObject handDeflectorLeft;

    private Transform transformDeflectorRight;
    private Transform transformDeflectorLeft;
    
    

	// Use this for initialization
	void Start () {
        transformDeflectorRight = handDeflectorRight.GetComponent<Transform>();
        transformDeflectorLeft = handDeflectorLeft.GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        
        // get the current button values
        float indexRight = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        float handRight = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);

        float indexLeft = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float handLeft = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);

        // change the size of the deflector
        float scaleFactorRight = (sizeRange.y - sizeRange.x) * (1f - handRight) + sizeRange.x;
        float scaleFactorLeft = (sizeRange.y - sizeRange.x) * (1f - handLeft) + sizeRange.x;    

        // move the deflector to the hand position
        transformDeflectorRight.localScale = scaleFactorRight * Vector3.one;
        transformDeflectorLeft.localScale = scaleFactorLeft * Vector3.one;

        handDeflectorRight.GetComponent<HandDeflectorBehavior>().handSize = handRight;
        handDeflectorLeft.GetComponent<HandDeflectorBehavior>().handSize = handLeft;
    }
}
