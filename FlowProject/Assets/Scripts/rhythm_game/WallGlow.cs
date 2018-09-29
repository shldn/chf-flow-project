using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallGlow : MonoBehaviour {

    public AudioProcessor audioProcessor;
    public enum Effect { Amp, Band, Const };
    public Effect effectHue = new Effect();
    public Effect effectSat = new Effect();
    public Effect effectVal = new Effect();

    public float pulseSpeed = 1.0f;
    public Vector2 rangeHue;
    public Vector2 rangeSat;
    public Vector2 rangeVal;
    public int audioBand;
    [SerializeField]
    private Color baseColor;

    private Renderer rend;
    private float rad;

    
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        rad = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        // extract color values
        float H, S, V;
        Color.RGBToHSV(baseColor, out H, out S, out V);
        float amp = audioProcessor._amplitudeBuffer;
        float band = audioProcessor._audioBandBuffer[audioBand];
        
        if (!float.IsNaN(amp) && !float.IsNaN(band))
        {
            // calculate new HSV values given the audio data
            if (effectHue == Effect.Amp)
                H = amp * (rangeHue.y - rangeHue.x) + rangeHue.x;
            else if (effectHue == Effect.Band)
                H = band * (rangeHue.y - rangeHue.x) + rangeHue.x;

            if (effectSat == Effect.Amp)
                S = amp * (rangeSat.y - rangeSat.x) + rangeSat.x;
            else if (effectSat == Effect.Band)
                S = band * (rangeSat.y - rangeSat.x) + rangeSat.x;

            if (effectVal == Effect.Amp)
                V = amp * (rangeVal.y - rangeVal.x) + rangeVal.x;
            else if (effectVal == Effect.Band)
                V = band * (rangeVal.y - rangeVal.x) + rangeVal.x;
        }

        // set the new color
        Color newColor = Color.HSVToRGB(H, S, V);
        rend.material.SetColor("_EmissionColor", newColor);
        DynamicGI.SetEmissive(rend, newColor);
    }
}
