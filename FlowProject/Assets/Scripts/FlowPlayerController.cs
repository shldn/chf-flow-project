using UnityEngine;
using UnityEngine.Networking;

public class FlowPlayerController : NetworkBehaviour{

	public static FlowPlayerController local = null; public static FlowPlayerController Local { get { return local; } }
	[SerializeField] private FlowtrailController trailController = null;

	[SyncVar] private float eegVal = 0f;


	public override void OnStartLocalPlayer(){
		local = this;
	} // End of OnStartLocalPlayer().


	void Update(){

		trailController.SetIntensity(eegVal);


        if (isLocalPlayer){

			transform.position = CameraController.Inst.transform.position;
			transform.rotation = CameraController.Inst.transform.rotation;


			// Manual origin movement
			Vector3 throttle = Vector3.zero;
			if(Input.GetKey(KeyCode.W))
				throttle.z += 1f;
			if(Input.GetKey(KeyCode.S))
				throttle.z -= 1f;
			if(Input.GetKey(KeyCode.D))
				throttle.x += 1f;
			if(Input.GetKey(KeyCode.A))
				throttle.x -= 1f;
			if(Input.GetKey(KeyCode.R))
				throttle.y += 1f;
			if(Input.GetKey(KeyCode.F))
				throttle.y -= 1f;


			if(MuseManager.Inst.MuseDetected)
				eegVal = MuseManager.Inst.LastConcentrationMeasure;
		

			CameraController.Inst.transform.parent.position += transform.rotation * throttle * Time.deltaTime * 3f;
		}

    } // End of Update().

} // End of FlowPlayerController.