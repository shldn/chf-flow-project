using UnityEngine;

public class HRVGrapher : MonoBehaviour {

	void Awake () {
        HRVServer.Initialize();
        HRVServer.MessageReceived += HandleHRVMessage;
	}
	
    void HandleHRVMessage(HRVMessage msg) {
        string msgStr = "Handling hrv message\n";
        for (int i = 0; i < msg.logs.Count; ++i) {
            msgStr += "\t" + msg.logs[i].ts + " " + msg.logs[i].rr + "\n";
        }
        Debug.LogError(msgStr);
    }
}
