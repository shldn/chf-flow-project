using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour {

    public static float moveSpeed = 1f;
    public Vector2 rangeSize;
    public string laneID;

    private Vector3 initialScale;
    private AudioProcessor audioProcessor;
    private bool initialized;


    // Use this for initialization
    void Start () {
        initialScale = transform.localScale;
	}
	
    public void Initialize(AudioProcessor audioProcessor, string laneID)
    {
        this.audioProcessor = audioProcessor;
        initialized = true;
        this.laneID = laneID;
    }

	// Update is called once per frame
	void Update () {
        if (!initialized)
            return;
        
        MoveForward();
        UpdateSize();
	}

    public void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    public void UpdateSize()
    {
        transform.localScale = initialScale * ((rangeSize.y - rangeSize.x) * audioProcessor._audioBandBuffer[1] + rangeSize.x);
    }

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}
