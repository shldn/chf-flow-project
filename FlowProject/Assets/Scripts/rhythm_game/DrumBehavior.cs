using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehavior : MonoBehaviour {

    public Material hitMat;
    public Material idleMat;
    public AudioClip hitSound;
    public AudioSource audioSource;
    public GameObject particleDestroy;
    public string laneID;

    private Renderer rend;
    private bool currentlyHit;

	// Use this for initialization
	void Start () {
        rend = GetComponentInChildren<Renderer>();
        currentlyHit = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        // drumsticks collide with the drum
        if (other.gameObject.tag == "drumstick")
        {
            audioSource.clip = hitSound;
            audioSource.Play();
            rend.material = hitMat;
            currentlyHit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // change back to non-hit state
        if (other.gameObject.tag == "drumstick")
        {
            rend.material = idleMat;
            currentlyHit = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // destroy the note that is in the drum as the player hits it
        if (other.tag == "note"
            && currentlyHit)
        {
            NoteBehavior nb = other.GetComponent<NoteBehavior>();
            if (nb.laneID == this.laneID)
            {
                Instantiate(particleDestroy, transform.position, transform.rotation);
                nb.OnDestroy();
            }
            
        }
    }
}
