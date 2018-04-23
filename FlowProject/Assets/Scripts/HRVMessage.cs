using System.Collections.Generic;

[System.Serializable]
public class HRVMeasure
{
    public long ts;
    public int rr;
}

[System.Serializable]
public class HRVMessage
{
    public string userid;
    public string session;
    public List<HRVMeasure> logs;
}