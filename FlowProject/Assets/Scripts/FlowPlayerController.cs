using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.Networking;

public class FlowPlayerController : NetworkBehaviour{

	public static FlowPlayerController local = null; public static FlowPlayerController Local { get { return local; } }
	[SerializeField] private FlowtrailController trailController = null;
	[SerializeField] private GameObject pivotIndicator = null;

	[SerializeField] private Renderer graphicRenderer = null;


	private float updateCooldown = 0f;

	// Synchronized Variables ============================================== //
	[SyncVar] private float eegConcent = 0f;
	private float eegConcent_last = 0f;

	[SyncVar] private float eegMellow = 0f;
	private float eegMellow_last = 0f;

	[SyncVar] private float hrv = 0f;
	private float hrv_last = 0f;

	[SyncVar] private Color playerColor = Color.white;

    // HRV
    HRVListener hrvListener = null;

	public override void OnStartLocalPlayer(){
		local = this;
		GameObject.Find("slider_eeg_concent").GetComponentInChildren<Slider>().onValueChanged.AddListener(Cmd_SetEEGConcent);
		GameObject.Find("slider_eeg_mellow").GetComponentInChildren<Slider>().onValueChanged.AddListener(SetEEGMellow);
		GameObject.Find("slider_hrv").GetComponentInChildren<Slider>().onValueChanged.AddListener(SetHRV);

		GameObject.Find("button_color_blue").GetComponent<Button>().onClick.AddListener(() => Cmd_SetColor(Color.blue));
		GameObject.Find("button_color_red").GetComponent<Button>().onClick.AddListener(() => Cmd_SetColor(Color.red));
		GameObject.Find("button_color_green").GetComponent<Button>().onClick.AddListener(() => Cmd_SetColor(Color.green));
		GameObject.Find("button_color_random").GetComponent<Button>().onClick.AddListener(RandomColor);

        // Setup HRVReceiver
        hrvListener = new HRVListener();

		// Set up initial position based on player prefs, if they've been set.
		if(PlayerPrefs.HasKey("anchor_posX")){
			CameraController.Inst.transform.parent.position = new Vector3(PlayerPrefs.GetFloat("anchor_posX"), PlayerPrefs.GetFloat("anchor_posY"), PlayerPrefs.GetFloat("anchor_posZ"));
			CameraController.Inst.transform.parent.rotation = new Quaternion(PlayerPrefs.GetFloat("anchor_rotX"), PlayerPrefs.GetFloat("anchor_rotY"), PlayerPrefs.GetFloat("anchor_rotZ"), PlayerPrefs.GetFloat("anchor_rotW"));
		}

		pivotIndicator.SetActive(false);

		Destroy(graphicRenderer);

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

		if(graphicRenderer)
			graphicRenderer.material.SetColor("_TintColor", Color.Lerp(graphicRenderer.material.GetColor("_TintColor"), Color.Lerp(playerColor, Color.black, 0.5f), Time.deltaTime * 0.1f));

		if(Input.GetKeyDown(KeyCode.Space))
			pivotIndicator.SetActive(false);

        if (isLocalPlayer){

			transform.position = CameraController.Inst.transform.position;
			transform.rotation = CameraController.Inst.transform.rotation;

			// Set origin to headset position with SPACEBAR
			if(Input.GetKeyDown(KeyCode.Space)){
				CameraController.Inst.transform.parent.rotation = Quaternion.Inverse(CameraController.Inst.transform.localRotation);
				Vector3 tempEulers = CameraController.Inst.transform.parent.eulerAngles;
				tempEulers.x = 0f;
				tempEulers.z = 0f;
				CameraController.Inst.transform.parent.eulerAngles = tempEulers;
				CameraController.Inst.transform.parent.position -= CameraController.Inst.transform.position;

				PlayerPrefs.SetFloat("anchor_posX", CameraController.Inst.transform.parent.position.x);
				PlayerPrefs.SetFloat("anchor_posY", CameraController.Inst.transform.parent.position.y);
				PlayerPrefs.SetFloat("anchor_posZ", CameraController.Inst.transform.parent.position.z);

				PlayerPrefs.SetFloat("anchor_rotX", CameraController.Inst.transform.parent.rotation.x);
				PlayerPrefs.SetFloat("anchor_rotY", CameraController.Inst.transform.parent.rotation.y);
				PlayerPrefs.SetFloat("anchor_rotZ", CameraController.Inst.transform.parent.rotation.z);
				PlayerPrefs.SetFloat("anchor_rotW", CameraController.Inst.transform.parent.rotation.w);
			}

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

                if (hrvListener.Active) {
                    hrv = hrvListener.LastNormalizedSample;
                    if (hrv != hrv_last) {
                        hrv_last = hrv;
                        Cmd_SetHRV(hrv);
                    }
                }
			}
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
