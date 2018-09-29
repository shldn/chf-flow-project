using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumstickController : MonoBehaviour {

    public float gripThreshold;
    public float regripSpeed;
    public GameObject drumstickLeft;
    public GameObject drumstickRight;
    public GameObject handAnchorPrim;
    public GameObject handAnchorSecond;

    private bool holdingDrumstickLeft;
    private bool holdingDrumstickRight;
	
	// Update is called once per frame
	void Update () {
        // determine whether the buttons on the controller are pressed or not
        float indexPrim = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float handPrim = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        float indexSecond = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        float handSecond = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);

        // check if the grip on the right is tight enough
        if (indexSecond > gripThreshold || handSecond > gripThreshold)
        {
            drumstickRight.GetComponent<Rigidbody>().isKinematic = true;

            // calculate the position for the drumsticks to move to
            Vector3 initialPos = drumstickRight.transform.position;
            Vector3 finalPos = handAnchorSecond.transform.position;
            Vector3 initialRot = drumstickRight.transform.rotation.eulerAngles;
            
            // calculate the rotation
            Quaternion finalRot = handAnchorSecond.transform.rotation;

            // move towards it
            drumstickRight.transform.position = Vector3.MoveTowards(initialPos, finalPos, regripSpeed * Time.deltaTime);
            drumstickRight.transform.rotation = finalRot;
        }
        else
            drumstickRight.GetComponent<Rigidbody>().isKinematic = false;

        // check if the grip on the left is tight enough
        if (indexPrim > gripThreshold || handPrim > gripThreshold)
        {
            drumstickLeft.GetComponent<Rigidbody>().isKinematic = true;

            // calculate the position for the drumsticks to move to
            Vector3 initialPos = drumstickLeft.transform.position;
            Vector3 finalPos = handAnchorPrim.transform.position;
            Vector3 initialRot = drumstickLeft.transform.rotation.eulerAngles;

            // calculate the rotation
            Quaternion finalRot = handAnchorPrim.transform.rotation;

            // move towards it
            drumstickLeft.transform.position = Vector3.MoveTowards(initialPos, finalPos, regripSpeed * Time.deltaTime);
            drumstickLeft.transform.rotation = finalRot;
        }
        else
            drumstickLeft.GetComponent<Rigidbody>().isKinematic = false;

    }
}
