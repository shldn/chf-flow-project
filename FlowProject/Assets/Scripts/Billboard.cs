using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
	
	void LateUpdate () {
		transform.LookAt(Camera.main.transform);
	} // End of LateUpdate().

} // End of Billboard class.
