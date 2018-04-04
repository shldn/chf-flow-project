using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FlowPlayerController : NetworkBehaviour{

	public static FlowPlayerController local = null; public static FlowPlayerController Local { get { return local; } }
	[SerializeField] private FlowtrailController trailController = null;

	[SyncVar] private float eegVal = 0f;
	private float lastEegVal = 0f;
	[SyncVar] private Color playerColor = Color.white;

	private float updateCooldown = 0f;


	public override void OnStartLocalPlayer(){
		local = this;
		GameObject.Find("Button").GetComponent<Button>().onClick.AddListener(ToggleVal);
		GameObject.Find("ColorButton").GetComponent<Button>().onClick.AddListener(RandomColor);
	} // End of OnStartLocalPlayer().


	public override void OnStartServer(){
		base.OnStartServer();

		RandomColor();

	} // End of OnStartServer().


	void Update(){

		trailController.SetIntensity(eegVal);
		trailController.SetColor(playerColor);

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


			updateCooldown = Mathf.MoveTowards(updateCooldown, 0f, Time.deltaTime);
			if(updateCooldown == 0f){
				updateCooldown = 0.5f;

				if(MuseManager.Inst.MuseDetected){
					eegVal = MuseManager.Inst.LastConcentrationMeasure;
					if(eegVal != lastEegVal){
						lastEegVal = eegVal;
						Cmd_SetEEGVal(eegVal);
					}
				}
			}
			
			CameraController.Inst.transform.parent.position += transform.rotation * throttle * Time.deltaTime * 3f;
		}

    } // End of Update().

	

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
	public void RandomColor(){
		Cmd_SetColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
	} // End of ToggleVal().

	// This does the thing on the server, then relays it to all clients other than the sender.
	[Command] private void Cmd_SetColor(Color newColor){
		playerColor = newColor;
	} // End of Cmd_SetColor().

} // End of FlowPlayerController.
