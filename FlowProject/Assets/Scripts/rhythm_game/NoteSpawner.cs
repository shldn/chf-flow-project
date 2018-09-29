using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour {

    public GameObject pf_note;
    public AudioProcessor audioProcessor;
    public string laneID;

    public void SpawnNote(float distanceFromTarget)
    {
        GameObject newNote = Instantiate(pf_note, transform);
        newNote.transform.localPosition = -Vector3.forward * distanceFromTarget;
        newNote.GetComponent<NoteBehavior>().Initialize(audioProcessor, laneID);
    }
}
