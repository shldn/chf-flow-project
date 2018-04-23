using UnityEngine;

public class HRVGrapher : MonoBehaviour {

    public GameObject graphPrefab;

    int rrMin = int.MaxValue;
    int rrMax = int.MinValue;
    int lastRR = 0;
    LineGraph graph = null;

    float LastNormalizedSample { get { return (float)(lastRR - rrMin) / (float)(rrMax - rrMin); } }
    void Awake () {
        HRVServer.Inst.MessageReceived += HandleHRVMessage;
        graph = CreateGraph("hrv");
    }

    private void Update() {
        if (rrMin < rrMax) {
            graph.sample = LastNormalizedSample;
        }
    }

    void HandleHRVMessage(HRVMessage msg) {
        string msgStr = "Handling hrv message\n";
        for (int i = 0; i < msg.logs.Count; ++i) {
            msgStr += "\t" + msg.logs[i].ts + " " + msg.logs[i].rr + "\n";
            HandleRange(msg.logs[i].rr);
            lastRR = msg.logs[i].rr;
        }
        //Debug.LogError(msgStr + " " + lastRR + " min: " + rrMin + " max: " + rrMax);
    }

    void HandleRange(int rr) {
        if (rr < rrMin)
            rrMin = rr;
        if (rr > rrMax)
            rrMax = rr;
    }
    LineGraph CreateGraph(string name) {
        GameObject newGraphGO = GameObject.Instantiate(graphPrefab) as GameObject;
        newGraphGO.name = "HRVGraph " + name;
        newGraphGO.transform.parent = transform;
        newGraphGO.transform.localPosition = Vector3.up;
        newGraphGO.transform.rotation = transform.rotation;
        newGraphGO.transform.localScale = Vector3.one;

        return newGraphGO.GetComponent<LineGraph>();
    }

}
