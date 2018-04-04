using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FlowPlayerController : NetworkBehaviour{

	public static FlowPlayerController local = null; public static FlowPlayerController Local { get { return local; } }
	[SerializeField] private FlowtrailController trailController = null;

	private float updateCooldown = 0f;

	// Synchronized Variables ============================================== //
	[SyncVar] private float eegConcent = 0f;
	private float eegConcent_last = 0f;

	[SyncVar] private float eegMellow = 0f;
	private float eegMellow_last = 0f;

	[SyncVar] private float hrv = 0f;
	private float hrv_last = 0f;

	[SyncVar] private Color playerColor = Color.white;




	public override void OnStartLocalPlayer(){
		local = this;
		GameObject.Find("slider_eeg_concent").GetComponentInChildren<Slider>().onValueChanged.AddListener(Cmd_SetEEGConcent);
		GameObject.Find("slider_eeg_mellow").GetComponentInChildren<Slider>().onValueChanged.AddListener(SetEEGMellow);
		GameObject.Find("slider_hrv").GetComponentInChildren<Slider>().onValueChanged.AddListener(SetHRV);

		GameObject.Find("button_color_blue").GetComponent<Button>().onClick.AddListener(() => Cmd_SetColor(Color.blue));
		GameObject.Find("button_color_red").GetComponent<Button>().onClick.AddListener(() => Cmd_SetColor(Color.red));
		GameObject.Find("button_color_green").GetComponent<Button>().onClick.AddListener(() => Cmd_SetColor(Color.green));
		GameObject.Find("button_color_random").GetComponent<Button>().onClick.AddListener(RandomColor);
	} // End of OnStartLocalPlayer().


	public override void OnStartServer(){
		base.OnStartServer();

		RandomColor();

	} // End of OnStartServer().


	private void Update(){

		trailController.SetCount(eegConcent);
		trailController.SetWidth(eegMellow);
		trailController.SetSpeed(hrv);
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
					eegConcent = MuseManager.Inst.LastConcentrationMeasure;
					GameObject.Find("slider_eeg_concent").GetComponentInChildren<Slider>().value = eegConcent;
					if(eegConcent != eegConcent_last){
						eegConcent_last = eegConcent;
						Cmd_SetEEGConcent(eegConcent);
					}

					eegMellow = MuseManager.Inst.LastMellowMeasure;
					GameObject.Find("slider_eeg_mellow").GetComponentInChildren<Slider>().value = eegMellow;
					if(eegMellow != eegMellow_last){
						eegMellow_last = eegMellow;
						Cmd_SetEEGMellow(eegMellow);
					}
				}

				if(hrv != hrv_last){
					hrv_last = hrv;
					Cmd_SetHRV(hrv);
				}
			}
			
			CameraController.Inst.transform.parent.position += transform.rotation * throttle * Time.deltaTime * 3f;
		}

    } // End of Update().

	

	// Networking ============================================================ //

	public void SetEEGConcent(float newVal){
		eegConcent = newVal;
		Cmd_SetEEGConcent(eegConcent);
	} // End of SetEEGConcent().
	
	[Command] private void Cmd_SetEEGConcent(float newVal){
		eegConcent = newVal;
	} // End of Cmd_SetEEGConcent().


	public void SetEEGMellow(float newVal){
		eegMellow = newVal;
		Cmd_SetEEGMellow(eegMellow);
	} // End of ToggleVal().
	
	[Command] private void Cmd_SetEEGMellow(float newVal){
		eegMellow = newVal;
	} // End of Cmd_SetEEGMellow().


	public void SetHRV(float newVal){
		hrv = newVal;
		Cmd_SetHRV(hrv);
	} // End of ToggleVal().
	
	[Command] private void Cmd_SetHRV(float newVal){
		hrv = newVal;
	} // End of Cmd_SetEEGMellow().
	


	// Player Color ---------------------------------------------------------- //
	public void RandomColor(){
		Cmd_SetColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
	} // End of ToggleVal().

	// This does the thing on the server, then relays it to all clients other than the sender.
	[Command] private void Cmd_SetColor(Color newColor){
		playerColor = newColor;
	} // End of Cmd_SetColor().

} // End of FlowPlayerController.
