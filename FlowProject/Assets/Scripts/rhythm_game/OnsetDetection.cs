using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnsetDetection : MonoBehaviour {

    public AudioSource audioSource;
    public static int numSamples = 1024;
    public int thresholdWindowSize = 30;
    public float thresholdMultiplier;
    public float fluxMultiplier;
    public float thresholdMin = 0.1f;
    public int frequencyBands = 8;
    public bool readyToDetect { get; private set; }

    float[] curSpectrum = new float[numSamples];
    float[] prevSpectrum = new float[numSamples];
    public List<SpectralFluxInfo>[] spectralFluxSamples;
    public int indexToProcess = 1;

    void Start()
    {
        // create a list for each band
        spectralFluxSamples = new List<SpectralFluxInfo>[frequencyBands];
        for (int i = 0; i < frequencyBands; i++)
        {
            spectralFluxSamples[i] = new List<SpectralFluxInfo>();
        }
        readyToDetect = false;
        indexToProcess = thresholdWindowSize / 2;
    }

    public void setCurSpectrum(float[] spectrum)
    {
        curSpectrum.CopyTo(prevSpectrum, 0);
        spectrum.CopyTo(curSpectrum, 0);
    }


    float[] calculateRectifiedSpectralFlux()
    {
        float[] sums = new float[frequencyBands];
        float sum = 0f;
        int indexPrev = 0;

        // find the differences between the current and previous spectrums
        for (int i = 0; i < frequencyBands; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i + 2);
            for (int j = indexPrev; j < sampleCount + indexPrev; j++)
            {
                sums[i] += Mathf.Max(0f, curSpectrum[j] - prevSpectrum[j]);
            }
            indexPrev = sampleCount + indexPrev;

        }

        return sums;
    }

    // calculate the threshold for how high the flux must be to be considered a beat
    float getFluxThreshold(int spectralFluxIndex, int bandIndex)
    {
        // How many samples in the past and future we include in our average
        int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
        int windowEndIndex = Mathf.Min(spectralFluxSamples[bandIndex].Count - 1, spectralFluxIndex + thresholdWindowSize / 2);

        // Add up our spectral flux over the window
        float sum = 0;
        for (int j = windowStartIndex; j < windowEndIndex; j++)
        {
            sum += spectralFluxSamples[bandIndex][j].spectralFlux;
        }
        float avg = sum / (windowEndIndex - windowStartIndex);
        return Mathf.Max(thresholdMin, avg * thresholdMultiplier);
    }

    // difference between flux and threshold
    float getPrunedSpectralFlux(int spectralFluxIndex, int bandIndex)
    {
        return Mathf.Max(0f, spectralFluxSamples[bandIndex][spectralFluxIndex].spectralFlux - spectralFluxSamples[bandIndex][spectralFluxIndex].threshold);
    }

    // check if it is a beat
    bool isPeak(int spectralFluxIndex, int bandIndex)
    {
        return (spectralFluxSamples[bandIndex][spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[bandIndex][spectralFluxIndex + 1].prunedSpectralFlux &&
            spectralFluxSamples[bandIndex][spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[bandIndex][spectralFluxIndex - 1].prunedSpectralFlux);
        
    }

    // analyze a new spectrum and determine where the beats are
    public void analyzeSpectrum(float[] spectrum, float time)
    {
        // Set spectrum
        setCurSpectrum(spectrum);

        // Get current spectral flux from spectrum
        
        float[] newFlux = calculateRectifiedSpectralFlux();
        for (int i = 0; i < frequencyBands; i++)
        {
            SpectralFluxInfo curInfo = new SpectralFluxInfo();
            curInfo.time = time;
            curInfo.spectralFlux = newFlux[i];
            spectralFluxSamples[i].Add(curInfo);
        }

        // We have enough samples to detect a peak
        if (spectralFluxSamples[0].Count > thresholdWindowSize)
        {
            readyToDetect = true;
            // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
            int indexToDetectPeak = indexToProcess - 1;
            for (int i = 0; i < frequencyBands; i++)
            {
                spectralFluxSamples[i].RemoveAt(0);

                // Get Flux threshold of time window surrounding index to process
                spectralFluxSamples[i][indexToProcess].threshold = getFluxThreshold(indexToProcess, i); 

                // Only keep amp amount above threshold to allow peak filtering
                spectralFluxSamples[i][indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(indexToProcess, i);

                

                bool curPeak = isPeak(indexToDetectPeak, i);

                if (curPeak)
                {
                    spectralFluxSamples[i][indexToDetectPeak].isPeak = true;
                }
            }
            
        }
        else
        {
            Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", spectralFluxSamples[0].Count, thresholdWindowSize));
        }
    }
}

// container object to hold info
public class SpectralFluxInfo
{
    public float time;
    public float spectralFlux;
    public float threshold;
    public float prunedSpectralFlux;
    public bool isPeak;
}