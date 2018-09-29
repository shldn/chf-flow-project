using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBehavior : MonoBehaviour {

    public GameObject[] laneSpawns = new GameObject[7];
   
    public GameObject bgmAudible;
    public GameObject bgmDetection;
    public OnsetDetection onsetDetection;

    private AudioProcessor processorAudible;
    private AudioProcessor processorDetection;

    // the delay is necessary because of the flux calculations in determining beats
    public float preprocessDelay;
    public float noteSpeed;
    public float spawnrate = 2f;

    public float[] thresholds = new float[7];

    private float spawnDistance;
    private float[] timeNextSpawn;
    private float timeStartAudible;
    private bool AudibleStarted = false;
    private float[] spectrum = new float[1024];
    private AudioSource audioSourceBgm;

    // Use this for initialization
    void Start()
    {
        // calculate the distance from the drums to spawn
        spawnDistance = noteSpeed * preprocessDelay * 4.7f;
        
        processorAudible = bgmAudible.GetComponent<AudioProcessor>();
        processorDetection = bgmDetection.GetComponent<AudioProcessor>();

        NoteBehavior.moveSpeed = noteSpeed;

        audioSourceBgm = bgmDetection.GetComponent<AudioSource>();
        audioSourceBgm.Play();

        timeStartAudible = Time.time + preprocessDelay;
        AudibleStarted = false;
        timeNextSpawn = new float[laneSpawns.Length];
    }

    // Update is called once per frame
    void Update () {
        
        if (!AudibleStarted && Time.time > timeStartAudible)
        {
            AudibleStarted = true;
            bgmAudible.GetComponent<AudioSource>().Play();
        }
        //audioSourceBgm.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        //onsetDetection.analyzeSpectrum(spectrum, audioSourceBgm.time);
        SpawnNotes();
	}

    private void SpawnNotes()
    {
        // spawn the notes only if the onset detector is ready
        if (onsetDetection.readyToDetect)
        {
            // bass
            if (Time.time > timeNextSpawn[3])
            {
                SpectralFluxInfo sample = onsetDetection.spectralFluxSamples[0][onsetDetection.indexToProcess - 2];
                if (sample.isPeak)
                {
                    GameObject selectedLane = laneSpawns[3];
                    selectedLane.GetComponent<NoteSpawner>().SpawnNote(spawnDistance);
                    timeNextSpawn[3] = 1f / spawnrate + Time.time;
                }
            }

            // snare
            if (Time.time > timeNextSpawn[1])
            {
                SpectralFluxInfo sample = onsetDetection.spectralFluxSamples[2][onsetDetection.indexToProcess - 2];
                if (sample.isPeak)
                {
                    GameObject selectedLane = laneSpawns[1];
                    selectedLane.GetComponent<NoteSpawner>().SpawnNote(spawnDistance);
                    timeNextSpawn[1] = 1f / spawnrate + Time.time;
                }
            }

            // tomf
            if (Time.time > timeNextSpawn[5])
            {
                SpectralFluxInfo sample = onsetDetection.spectralFluxSamples[5][onsetDetection.indexToProcess - 2];
                if (sample.isPeak)
                {
                    GameObject selectedLane = laneSpawns[5];
                    selectedLane.GetComponent<NoteSpawner>().SpawnNote(spawnDistance);
                    timeNextSpawn[5] = 1f / spawnrate + Time.time;
                }
            }

            // crash
            if (Time.time > timeNextSpawn[0])
            {
                SpectralFluxInfo sample = onsetDetection.spectralFluxSamples[7][onsetDetection.indexToProcess - 2];
                if (sample.isPeak)
                {
                    GameObject selectedLane = laneSpawns[0];
                    selectedLane.GetComponent<NoteSpawner>().SpawnNote(spawnDistance);
                    timeNextSpawn[0] = 1f / spawnrate + Time.time;
                }
            }

            /*
            for (int i = 0; i < 1; i++)
            {
                SpectralFluxInfo sample = onsetDetection.spectralFluxSamples[i][onsetDetection.indexToProcess - 2];
                if (sample.isPeak)
                {
                    GameObject selectedLane = laneSpawns[i];
                    selectedLane.GetComponent<NoteSpawner>().SpawnNote(spawnDistance);
                    timeNextSpawn = 1f / spawnrate + Time.time;
                }
            }*/

        }
    }

    void onOnbeatDetected()
    {
        Debug.Log("Beat!!!");
    }

    //This event will be called every frame while music is playing
    void onSpectrum(float[] spectrum)
    {
        //The spectrum is logarithmically averaged
        //to 12 bands
        for (int i = 0; i < spectrum.Length; ++i)
        {
            Vector3 start = new Vector3(i, 0, 0);
            Vector3 end = new Vector3(i, spectrum[i], 0);
            Debug.DrawLine(start, end);
        }
    }
}
