
public class HRVListener {

    int rrMin = int.MaxValue;
    int rrMax = int.MinValue;
    int lastRR = 0;
    bool gotMessages = false;
    public bool Active { get { return gotMessages; } }
    public float LastNormalizedSample { get { return (float)(lastRR - rrMin) / (float)(rrMax - rrMin); } }

    public HRVListener() {
        //HRVServer.Inst.MessageReceived += HandleHRVMessage;
    }

    void HandleHRVMessage(HRVMessage msg) {
        gotMessages = true;
        string msgStr = "Handling hrv message\n";
        for (int i = 0; i < msg.logs.Count; ++i) {
            msgStr += "\t" + msg.logs[i].ts + " " + msg.logs[i].rr + "\n";
            HandleRange(msg.logs[i].rr);
            lastRR = msg.logs[i].rr;
        }
        UnityEngine.Debug.LogError(msgStr + " " + lastRR + " min: " + rrMin + " max: " + rrMax);
    }

    void HandleRange(int rr) {
        if (rr < rrMin)
            rrMin = rr;
        if (rr > rrMax)
            rrMax = rr;
    }
}
