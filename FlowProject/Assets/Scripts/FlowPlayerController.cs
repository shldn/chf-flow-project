using UnityEngine;
using UnityEngine.Networking;

public class FlowPlayerController : NetworkBehaviour{

	public static FlowPlayerController local = null; public static FlowPlayerController Local { get { return local; } }


	public override void OnStartLocalPlayer(){
		local = this;
	} // End of OnStartLocalPlayer().


	void Update(){

        if (!isLocalPlayer)
            return;

        transform.position = CameraController.Inst.transform.position;
        transform.rotation = CameraController.Inst.transform.rotation;
    } // End of Update().

} // End of FlowPlayerController.