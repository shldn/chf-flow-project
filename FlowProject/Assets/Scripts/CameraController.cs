using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public static CameraController Inst = null;


	private void Awake(){
		Inst = this;
	} // End of Awake().

} // End of CameraController.
