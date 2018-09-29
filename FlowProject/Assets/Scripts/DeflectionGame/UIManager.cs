using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Text textDebug;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float concentration = 0;
        float mellow = 0;

        if (MuseManager.Inst.MuseDetected) {
            concentration = MuseManager.Inst.LastConcentrationMeasure;
            mellow = MuseManager.Inst.LastMellowMeasure;
        }

        textDebug.text = "Concentration: " + concentration + "\n" + "Mellow: " + mellow;
    }
}
