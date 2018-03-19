using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FlowPlayerController : NetworkBehaviour{

	public static FlowPlayerController local = null; public static FlowPlayerController Local { get { return local; } }
	[SerializeField] private FlowtrailController trailController = null;

	[SyncVar] private float eegVal = 0f;


	public override void OnStartLocalPlayer(){
		local = this;
		RandomColor();
		GameObject.Find("Button").GetComponent<Button>().onClick.AddListener(ToggleVal);
		GameObject.Find("ColorButton").GetComponent<Button>().onClick.AddListener(RandomColor);
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

	public void RandomColor(){
		Local_SetColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
	} // End of ToggleVal().



	// Networking ============================================================ //

	// EEG Val --------------------------------------------------------------- //
	public void ToggleVal(){
		eegVal = 1 - eegVal;
		Cmd_SetEEGVal(eegVal);
	} // End of ToggleVal().
	
	[Command] private void Cmd_SetEEGVal(float newEEGVal){
		eegVal = newEEGVal;
	} // End of UpdateEEGVal().



	// Player Color ---------------------------------------------------------- //
	// This actually does the thing locally--no networking.
	private void SetColor(Color newColor){
		trailController.SetColor(newColor);
	} // End of SetColor().

	// This is how you access doing the thing, either as a client or the server.
	private void Local_SetColor(Color newColor){
		// If we're not the server, let's do it now before sending the request such that it happens instantly for us.
		if(!isServer)
			SetColor(newColor);
		// We call this regardless of whether or not we're the server... it'll happen on both.
		Cmd_SetColor(newColor);
	} // End of SetColor().

	// On the server, this just happens instantly. A client 'requests' this but it actually happens on the server.
	// We assume the client already called the method locally before informing us... cheeky lil cunt.
	// 
	// This does the thing on the server, then relays it to all clients other than the sender.
	[Command] private void Cmd_SetColor(Color newColor){
		SetColor(newColor);
		for(int i = 0; i < NetworkServer.connections.Count; i++)
			if(NetworkServer.connections[i] != connectionToClient)
				Target_SetColor(NetworkServer.connections[i], newColor);
	} // End of Cmd_SetColor().

	[TargetRpc] private void Target_SetColor(NetworkConnection target, Color newColor){
		SetColor(newColor);
	} // End of Rpc_SetColor().

} // End of FlowPlayerController.
