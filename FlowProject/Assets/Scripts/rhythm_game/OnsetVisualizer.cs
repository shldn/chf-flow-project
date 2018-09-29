using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnsetVisualizer : MonoBehaviour {
    public AudioSource audioSource;
    public GameObject markerPeak;
    public GameObject markerThresh;
    public GameObject markerFlux;
    public OnsetDetection onsetDetection;
    public AudioProcessor audioProcessor;
    private float[] spectrum = new float[1024];
    private float timeStart;
    // Use this for initialization
    void Start () {
        timeStart = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        onsetDetection.analyzeSpectrum(spectrum, audioSource.time);
        /*
        SpectralFluxInfo sample = onsetDetection.spectralFluxSamples[0][onsetDetection.indexToProcess - 2];
        Vector3 posPeak = new Vector3(2* (Time.time - timeStart), sample.spectralFlux, 0f);
        Vector3 posThresh = new Vector3(2*(Time.time - timeStart), sample.threshold, 0f);
        if (sample.isPeak)
        {
            Instantiate(markerPeak, posPeak, transform.rotation);
        }
        else
        {
            Instantiate(markerFlux, posPeak, transform.rotation);
        }
        Instantiate(markerThresh, posThresh, transform.rotation);
        */
    }
}
