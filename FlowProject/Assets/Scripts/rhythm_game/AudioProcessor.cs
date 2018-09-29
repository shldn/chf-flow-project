using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioProcessor : MonoBehaviour
{
    AudioSource _audioSource;
    public int sampleSize = 1024;
    public int bandSize = 8;
    public float initDecrease = 0.005f;
    public float[] _samplesLeft { get; private set; }

    // buffers hold the 'smoothed' values, which aren't as volatile and decrease
    // smoothly
    public float[] _samplesRight { get; private set; }
    public float[] _samplesLeftBuffer { get; private set; }
    public float[] _samplesRightBuffer { get; private set; }
    public float[] _audioLeftBuffer { get; private set; }
    public float[] _audioRightBuffer { get; private set; }

    // holds the speed at which the buffers should decrease at
    float[] _samplesLeftBufferDecrease;
    float[] _samplesRighBufferDecrease;
    float[] _samplesLeftHighest;
    float[] _samplesRightHighest;

    float[] _bandHighest;
    float[] _band;
    float[] _bandBuffer;
    float[] _bufferDecrease;

    // holds the banded spectrum
    public float[] _audioBand { get; private set; }
    public float[] _audioBandBuffer { get; private set; }

    // the average volume
    public float _amplitude { get; private set; }
    public float _amplitudeBuffer { get; private set; }

    // initial settings
    float _amplitudeHighest;
    public float _audioProfile;
    public float _bufferDecreaseSpeed = 1.005f;

    public enum _channel { Stereo, Left, Right };
    public _channel channel = new _channel();

    // Use this for initialization
    void Start()
    {
        // allocate new objects for each array
        _samplesLeft = new float[sampleSize];
        _samplesRight = new float[sampleSize];

        _samplesLeftBuffer = new float[sampleSize];
        _samplesRightBuffer = new float[sampleSize];

        _audioLeftBuffer = new float[sampleSize];
        _audioRightBuffer = new float[sampleSize];

        _samplesLeftBufferDecrease = new float[sampleSize];
        _samplesRighBufferDecrease = new float[sampleSize];
        _samplesLeftHighest = new float[sampleSize];
        _samplesRightHighest = new float[sampleSize];

        _bandHighest = new float[bandSize];
        _band = new float[bandSize];
        _bandBuffer = new float[bandSize];
        _bufferDecrease = new float[bandSize];

        _audioBand = new float[bandSize];
        _audioBandBuffer = new float[bandSize];

        _audioSource = GetComponent<AudioSource>();

        // initialize max/min values 
        AudioProfile(_audioProfile);
    }

    // Update is called once per frame
    void Update()
    {
        // get the spectrum data
        GetSpectrumAudioSource();

        // simplify them into lower granularity bands
        MakeFrequencyBands();

        // change their min/max values to reflected the widened range of values
        CreateAudioBands();

        // smooth the decreases
        BandBuffer();

        GetAmplitude();
    }

    void CreateAudioBands()
    {
        // loop through each band and set a new maximum
        for (int i = 0; i < bandSize; i++)
        {
            if (_band[i] > _bandHighest[i])
            {
                _bandHighest[i] = _band[i];
            }
            _audioBand[i] = (_band[i] / _bandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _bandHighest[i]);
        }

        for (int i = 0; i < sampleSize; i++)
        {
            if (_samplesLeft[i] > _samplesLeftHighest[i])
            {
                _samplesLeftHighest[i] = _samplesLeft[i];
            }
            if (_samplesLeftHighest[i] != 0)
            {
                _samplesLeft[i] = (_samplesLeft[i] / _samplesLeftHighest[i]);
                //_audioLeftBuffer[i] = (_samplesLeftBuffer[i] / _samplesLeftHighest[i]);
            }
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        _audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
    }

    // decrease the band values according to the specified speeds
    void BandBuffer()
    {
        for (int i = 0; i < bandSize; i++)
        {
            if (_band[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _band[i];
                _bufferDecrease[i] = initDecrease;
            }

            if (_band[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= _bufferDecreaseSpeed;
            }
        }

        for (int i = 0; i < sampleSize; i++)
        {
            if (_samplesLeft[i] > _samplesLeftBuffer[i])
            {
                _samplesLeftBuffer[i] = _samplesLeft[i];
                _samplesLeftBufferDecrease[i] = initDecrease;
            }
            else if (_samplesLeft[i] < _samplesLeftBuffer[i])
            {
                _samplesLeftBuffer[i] -= _samplesLeftBufferDecrease[i];
                _samplesLeftBuffer[i] = Mathf.Max(0, _samplesLeftBuffer[i]);
                _samplesLeftBufferDecrease[i] *= _bufferDecreaseSpeed;
            }
        }
    }

    // set the initial max value
    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _bandHighest[i] = audioProfile;
        }

    }

    // group multiple frequencies into one 'band' to aggregate different sound levels
    void MakeFrequencyBands()
    {
        int count = 0;

        // loop through each band
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            // determine the number of frequencies to fit in the band
            int sampleCount = (int)Mathf.Pow(2, i + 1);
            if (i == 7)
            {
                sampleCount += 2;
            }

            // average their values to obtain the 'band' value
            for (int j = 0; j < sampleCount; j++)
            {
                if (channel == _channel.Stereo)
                {
                    average += (_samplesLeft[count] + _samplesRight[count]) * (count + 1);
                }
                else if (channel == _channel.Left)
                {
                    average += _samplesLeft[count] * (count + 1);
                }
                else
                    average += _samplesRight[count] * (count + 1);
                count++;
            }
            average /= count;
            _band[i] = average * 10;
        }
    }

    void GetAmplitude()
    {
        // sum up th values in each band, then divide by highest amplitude
        float _currentAmplitude = 0;
        float _currentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _currentAmplitude += _audioBand[i];
            _currentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if (_currentAmplitude > _amplitudeHighest)
            _amplitudeHighest = _currentAmplitude;

        _amplitude = _currentAmplitude / _amplitudeHighest;
        _amplitudeBuffer = _currentAmplitudeBuffer / _amplitudeHighest;

    }
}
